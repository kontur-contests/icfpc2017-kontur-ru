using System;
using System.Collections.Generic;
using System.IO;
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
            return metas
                .OrderBy("timestamp")
                .LimitToLast(limit)
                .OnceAsync<ReplayMeta>()
                .ConfigureAwait(false).GetAwaiter()
                .GetResult()
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

    [TestFixture]
    public class ReplayRepoTests
    {
        [Test]
        public void SaveReplay_ShouldSave()
        {
            var repo = new ReplayRepo(true);
            
            var meta = new ReplayMeta(
                DateTime.UtcNow,
                "player",
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
            var map = MapLoader.LoadMap(Path.Combine(TestContext.CurrentContext.TestDirectory, @"..\..\..\..\maps\circle.json")).Map;

            var data = new ReplayData(
                map, new Move[]
                {
                    new ClaimMove(0, 15, 16),
                    new ClaimMove(0, 16, 17),
                    new ClaimMove(0, 17, 18),
                }, new[] {new Future(18, 15)});

            repo.SaveReplay(meta, data);

            var savedData = repo.GetData(meta.DataId);
            
            Assert.NotNull(savedData);
        }
        
        [Test]
        public void GetRecentMetas_Should()
        {
            var repo = new ReplayRepo(true);
            
            var metas = repo.GetRecentMetas();
            
            Assert.That(metas[0].Timestamp > metas[1].Timestamp);
        }
    }
}