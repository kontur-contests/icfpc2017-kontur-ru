using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Database.Query;
using lib.Firebase;
using Newtonsoft.Json.Linq;

namespace lib.Replays
{
    public interface IReplayRepo
    {
        void SaveReplay(ReplayMeta meta, ReplayData data);

        ReplayMeta[] GetRecentMetas(int limit = 10);
        ReplayData GetData(string dataId);
    }

    public class ReplayRepo : IReplayRepo
    {
        private readonly FirebaseClient fb;

        private ChildQuery metas;
        private ChildQuery datas;

        public ReplayRepo(bool test = false)
        {
            fb = Connect().ConfigureAwait(false).GetAwaiter().GetResult();
            metas = test ? fb.Child("test").Child("replays").Child("metas") : fb.Child("replays").Child("metas");
            datas = test ? fb.Child("test").Child("replays").Child("datas") : fb.Child("replays").Child("datas");
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
            var result = datas
                .PostAsync(data)
                .ConfigureAwait(false).GetAwaiter()
                .GetResult();

            meta.DataId = result.Key;
            
            metas
                .PostAsync(meta)
                .ConfigureAwait(false).GetAwaiter()
                .GetResult();
        }

        public ReplayMeta[] GetRecentMetas(int limit = 10)
        {
            var items = metas
                .OrderBy("timestamp")
                .LimitToLast(limit)
                .OnceAsync<ReplayMeta>()
                .ConfigureAwait(false).GetAwaiter()
                .GetResult();
            return items
                .Select(x => x.Object)
                .OrderByDescending(x => x.Timestamp)
                .ToArray();
        }

        public ReplayData GetData(string dataId)
        {
            return datas
                .Child(dataId)
                .OnceSingleAsync<ReplayData>()
                .ConfigureAwait(false).GetAwaiter()
                .GetResult();
        }
    }
}