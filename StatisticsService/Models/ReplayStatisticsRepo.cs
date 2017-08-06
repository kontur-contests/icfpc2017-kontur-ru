using System;
using lib.Replays;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Database.Query;
using lib;
using Newtonsoft.Json;

namespace StatisticsService.Models
{
    public interface IReplayStatisticsRepo
    {
        IList<ReportModel> GetReportsStatistics();
    }

    public class ReplayStatisticsRepo : IReplayStatisticsRepo
    {
        private const int interval = 120 / ReplaysStatisticsConverter.minutesGroupSize;

        private readonly FirebaseClient fb;

        private ChildQuery metas;
        private ChildQuery maps;

        public ReplayStatisticsRepo()
        {
            fb = Connect().ConfigureAwait(false).GetAwaiter().GetResult();
            metas = fb.Child("v2").Child("replays").Child("metas");
            maps = fb.Child("v2").Child("maps");
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

        public IList<ReportModel> GetReportsStatistics()
        {
            var replayMetas = GetAllMeta()
                .Select(e => e.Object)
                .ToArray();

            var maxTime = replayMetas.Max(e => ((DateTimeOffset) e.Timestamp).ToUnixTimeSeconds() / (60 * ReplaysStatisticsConverter.minutesGroupSize));

            var sizes = new Dictionary<string, int>();

            return replayMetas
                .GroupBy(e => ((DateTimeOffset) e.Timestamp).ToUnixTimeSeconds() / (60 * ReplaysStatisticsConverter.minutesGroupSize))
                .Where(e => e.Key > maxTime - interval)
                .Select(
                    e =>
                    {
                        return new ReportModel(
                            e.Key * ReplaysStatisticsConverter.minutesGroupSize, e.Count(),
                            e.GroupBy(g => g.AiName).ToDictionary(g => g.Key, g => g.Count()),
                            e.GroupBy(
                                    g =>
                                    {
                                        if (!sizes.ContainsKey(g.MapHash))
                                        {
                                            var mapData = maps.Child("data")
                                                .Child(g.MapHash)
                                                .OnceSingleAsync<EncodedData>()
                                                .ConfigureAwait(false).GetAwaiter()
                                                .GetResult().D;

                                            var map = JsonConvert.DeserializeObject<Map>(mapData);

                                            var array = map.Mines.OrderBy(ee => ee).ToArray();

                                            sizes[g.MapHash] = MapMapper.Hashes.Any(ee => ee.Key.OrderBy(gg => gg).SequenceEqual(array))
                                                ? MapMapper.Hashes.FirstOrDefault(ee => ee.Key.OrderBy(gg => gg).SequenceEqual(array)).Value
                                                : 0;
                                        }

                                        return sizes[g.MapHash];
                                    })
                                .ToDictionary(g => g.Key, g => g.Count()),
                            e.Count(g => g.Scores.OrderByDescending(t => t.score).IndexOf(t => t.punter == g.OurPunter) == 0),
                            e.GroupBy(g => g.AiName).ToDictionary(
                                g => g.Key,
                                g => g.Count(gg => gg.Scores.OrderByDescending(t => t.score).IndexOf(t => t.punter == gg.OurPunter) == 0)),
                            e.GroupBy(g => sizes[g.MapHash]).ToDictionary(
                                g => g.Key,
                                g => g.Count(gg => gg.Scores.OrderByDescending(t => t.score).IndexOf(t => t.punter == gg.OurPunter) == 0)));
                    })
                .ToArray();
        }
        
        private IReadOnlyCollection<FirebaseObject<ReplayMeta>> GetAllMeta()
        {
            return metas
                .OnceAsync<ReplayMeta>()
                .ConfigureAwait(false).GetAwaiter()
                .GetResult();
        }
    }

    public static class MapMapper
    {
        private static readonly IDictionary<string, int> maps = new Dictionary<string, int>
        {
            ["boston-sparse"] = 8,
            //["boston"] = 1,
            ["circle"] = 4,
            //["edge"] = 1,
            //["edinburgh-scaled"] = 1,
            ["edinburgh-sparse"] = 16,
            //["edinburgh-sparse2"] = 1,
            //["edinburgh"] = 1,
            //["gen1"] = 1,
            //["gen2"] = 1,
            //["gen3"] = 1,
            //["gen4"] = 1,
            ["gothenburg-sparse"] = 16,
            ["lambda"] = 4,
            //["nara-scaled"] = 1,
            ["nara-sparse"] = 16,
            //["nara"] = 1,
            //["oxford-3000-nodes"] = 1,
            //["oxford-center-sparse"] = 1,
            //["oxford-center"] = 1,
            ["oxford-sparse"] = 16,
            //["oxford-sparse2"] = 1,
            //["oxford"] = 1,
            //["oxford2-scaled"] = 1,
            //["oxford2-sparse-2"] = 1,
            //["oxford2-sparse"] = 1,
            //["oxford2"] = 1,
            ["randomMedium"] = 4,
            ["randomSparse"] = 4,
            ["sample"] = 2,
            ["Sierpinski-triangle"] = 3,
            //["spider_web"] = 1,
            //["t1"] = 1,
            ["tube"] = 8,
            //["van-city-sparse"] = 1
        };

        private static  IDictionary<int[], int> hashes;
        public static IDictionary<int[], int> Hashes => hashes ?? (hashes = LoadHashes());

        private static Dictionary<int[], int> LoadHashes()
            => MapLoader.LoadDefaultMaps()
                .Join(maps, map => map.Name, pair => pair.Key, (map, pair) => new {NamedMap = map, Size = pair.Value})
                .ToDictionary(e => e.NamedMap.Map.Mines, e => e.Size);
    }
}