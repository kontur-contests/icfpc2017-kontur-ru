using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Database.Query;
using lib.Firebase;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace lib.Replays
{
    public interface IReplayRepo
    {
        void SaveReplay(ReplayMeta meta, ReplayData data);

        IList<ReplayMeta> GetRecentMetas(int limit = 10);
        ReplayData GetData(string dataId);
    }

    public class ReplayRepo : IReplayRepo
    {
        private readonly FirebaseClient fb;

        public ReplayRepo()
        {
            fb = Connect().Result;
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
            var result = fb.Child("replays").Child("datas")
                .PostAsync(data)
                .ConfigureAwait(false).GetAwaiter()
                .GetResult();

            meta.DataId = result.Key;
            
            fb.Child("replays").Child("metas")
                .PostAsync(meta)
                .ConfigureAwait(false).GetAwaiter()
                .GetResult();
        }

        public IList<ReplayMeta> GetRecentMetas(int limit = 10)
        {
            throw new System.NotImplementedException();
        }

        public ReplayData GetData(string dataId)
        {
            return fb.Child("replays").Child("datas")
                .Child(dataId)
                .OnceSingleAsync<ReplayData>()
                .ConfigureAwait(false).GetAwaiter()
                .GetResult();
        }
    }

    [TestFixture]
    public class ReplayRepoTests
    {
        [Test]
        public void SaveReplay_ShouldSave()
        {
            var repo = new ReplayRepo();
            
            var meta = new ReplayMeta(
                DateTime.UtcNow,
                "testAi",
                0,
                1,
                new[]
                {
                    new ScoreModel
                    {
                        Punter = 0,
                        Score = 42
                    }
                }
            );
            
            var data = new ReplayData(new Map(), new[]
            {
                new PassMove(0)
            });
            
            repo.SaveReplay(meta, data);

            var savedData = repo.GetData(meta.DataId);
            
            Assert.NotNull(savedData);
        }
    }
}