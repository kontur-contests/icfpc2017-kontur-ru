using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using lib.Structures;
using Newtonsoft.Json;
using NUnit.Framework;

namespace lib.Replays
{
    [TestFixture]
    [Explicit]
    public class ReplayRepoTests
    {

        [Test]
        //[Ignore("Danger Area!!!!")]
        [Explicit]
        public void Delete()
        {
            new ReplayRepo(true).DeleteAll();
        }
        [Test]
        public void SaveReplay_ShouldSave()
        {
            var repo = new ReplayRepo(true);
            var meta = CreateReplayMeta();
            var map = MapLoader.LoadMap(Path.Combine(TestContext.CurrentContext.TestDirectory, @"..\..\..\..\maps\oxford2.json")).Map;
            JsonConvert.SerializeObject(map).CalculateMd5();
            var data = new ReplayData(map, Enumerable.Range(0, map.Rivers.Length).Select(i => Move.Claim(0, i, i + 1)).ToArray(), new Future[0]);
            var sw = Stopwatch.StartNew();
            repo.SaveReplay(meta, data);
            Console.WriteLine(sw.Elapsed);
            sw.Restart();
            var savedData = repo.GetData(meta);
            Console.WriteLine(sw.Elapsed);
            Assert.NotNull(savedData);
        }

        private static ReplayMeta CreateReplayMeta()
        {
            var meta = new ReplayMeta(
                DateTime.UtcNow,
                "player",
                "0.1",
                "aabbcc",
                0,
                1,
                new[]
                {
                    new Score
                    {
                        punter = 0,
                        score = 42
                    }
                }
            );
            return meta;
        }

        [Test, Explicit]
        public void GetRecentMetas_Should()
        {
            var repo = new ReplayRepo(true);
            
            var metas = repo.GetRecentMetas();
            
            Assert.That(metas[0].Timestamp > metas[1].Timestamp);
        }
    }
}