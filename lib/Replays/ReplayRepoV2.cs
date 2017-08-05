using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Database.Query;
using Newtonsoft.Json;

namespace lib.Replays
{

    public class EncodedData
    {
        public EncodedData(string d)
        {
            D = d;
        }

        public string D;
    }
    public class ReplayRepo : IReplayRepo
    {
        private readonly FirebaseClient fb;

        private ChildQuery metas;
        private ChildQuery datas;
        private ChildQuery maps;
        private ChildQuery rootQuery;

        public ReplayRepo(bool test = false)
        {
            fb = Connect().ConfigureAwait(false).GetAwaiter().GetResult();
            var rootName = test ? "test" : "v2";
            rootQuery = fb.Child(rootName);
            metas = rootQuery.Child("replays").Child("metas");
            datas = rootQuery.Child("replays").Child("datas");
            maps = rootQuery.Child("maps");
        }

        private static async Task<FirebaseClient> Connect()
        {
            var auth = new FirebaseAuthProvider(new FirebaseConfig("AIzaSyBRYZvtAg1Vm5fZZ-r80vCISm0A8IhA7vM"));
            var link = await auth.SignInWithEmailAndPasswordAsync("pe@kontur.ru", "W1nnerzz");

            return new FirebaseClient("https://icfpc2017.firebaseio.com", new FirebaseOptions
            {
                AuthTokenAsyncFactory = () => Task.FromResult(link.FirebaseToken)
            });
        }

        public void SaveReplay(ReplayMeta meta, ReplayData data)
        {
            string encodedMap = Encode(data.Map);
            string mapHash = encodedMap.CalculateMd5();
            if (maps.Child("keys").Child(mapHash).OnceSingleAsync<EncodedData>().ConfigureAwait(false).GetAwaiter().GetResult()?.D != "1")
            {
                maps.Child("data").Child(mapHash).PutAsync(new EncodedData(encodedMap)).ConfigureAwait(false).GetAwaiter().GetResult();
                maps.Child("keys").Child(mapHash).PutAsync(new EncodedData("1")).ConfigureAwait(false).GetAwaiter().GetResult();
            }

            var result = datas
                .PostAsync(new EncodedData(data.Encode()))
                .ConfigureAwait(false).GetAwaiter()
                .GetResult();

            meta.DataId = result.Key;
            meta.MapHash = mapHash;

            metas.PostAsync(meta)
                .ConfigureAwait(false).GetAwaiter()
                .GetResult();
        }

        private static string Encode<T>(T data)
        {
            return JsonConvert.SerializeObject(data);
        }
        private static T Decode<T>(string data)
        {
            return JsonConvert.DeserializeObject<T>(data);
        }

        public ReplayMeta[] GetRecentMetas(int limit = 10)
        {
            var items = metas.OrderBy("timestamp")
                .LimitToLast(limit)
                .OnceAsync<ReplayMeta>()
                .ConfigureAwait(false).GetAwaiter()
                .GetResult();
            return items
                .Select(x => x.Object)
                .OrderByDescending(x => x.Timestamp)
                .ToArray();
        }

        public ReplayData GetData(ReplayMeta meta)
        {
            string data = datas
                .Child(meta.DataId)
                .OnceSingleAsync<EncodedData>()
                .ConfigureAwait(false).GetAwaiter()
                .GetResult().D;
            string mapData = maps.Child("data")
                .Child(meta.MapHash)
                .OnceSingleAsync<EncodedData>()
                .ConfigureAwait(false).GetAwaiter()
                .GetResult().D;
            var replayData = ReplayData.Decode(data);
            replayData.Map = Decode<Map>(mapData);
            return replayData;
        }

        public void DeleteAll()
        {
            rootQuery.DeleteAsync().ConfigureAwait(false).GetAwaiter().GetResult();
        }
    }
}