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
        private static IDictionary<string, int[]> mapsDictionary;
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
            return new FirebaseClient("https://icfpc2017.firebaseio.com", new FirebaseOptions
            {
            });
        }

        public IList<ReportModel> GetReportsStatistics()
        {
            var dictionary = mapsDictionary ?? (mapsDictionary = GetMapsInner());

            var replayMetas = GetAllMeta()
                .Select(e => e.Object)
                .ToArray();

            var maxTime = ((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds() / (60 * ReplaysStatisticsConverter.minutesGroupSize);

            var sizes = new Dictionary<string, int>();

            return replayMetas
                .GroupBy(e => ((DateTimeOffset) e.Timestamp).ToUnixTimeSeconds() / (60 * ReplaysStatisticsConverter.minutesGroupSize))
                .Where(e => e.Key >= maxTime - interval)
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
                                            var mines = dictionary[g.MapHash];

                                            sizes[g.MapHash] = MapMapper.Hashes.Any(ee => ee.Key.OrderBy(gg => gg).SequenceEqual(mines))
                                                ? MapMapper.Hashes.FirstOrDefault(ee => ee.Key.OrderBy(gg => gg).SequenceEqual(mines)).Value
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

        private IDictionary<string, int[]> GetMapsInner()
        {
            return maps.Child("data")
                .OnceAsync<EncodedData>()
                .ConfigureAwait(false).GetAwaiter()
                .GetResult()
                .ToDictionary(e => e.Key, e => JsonConvert.DeserializeObject<Map>(e.Object.D).Mines.OrderBy(g => g).ToArray());
        }
    }

    public static class MapMapper
    {
        private static IDictionary<int[], int> hashes;
        public static IDictionary<int[], int> Hashes => hashes ?? (hashes = LoadHashes());

        private static Dictionary<int[], int> LoadHashes()
            => MapLoader.LoadDefaultMaps()
                .Join(MapLoader.LoadOnlineMapSizes(), map => $"{map.Name}.json", pair => pair.Key, (map, pair) => new {NamedMap = map, Size = pair.Value})
                .ToDictionary(e => e.NamedMap.Map.Mines, e => e.Size);
    }
}