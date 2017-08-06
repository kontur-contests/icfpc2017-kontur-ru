using System;
using System.Linq;
using System.Threading.Tasks;
using MoreLinq;
using Newtonsoft.Json;
using NUnit.Framework;

namespace lib.Arena
{
    [TestFixture]
    public class ArenaApi_Tests
    {
        [Test, Explicit]
        public async Task GetArenaMatchesSmokeTest()
        {
            var matches = await new ArenaApi().GetArenaMatchesAsync();
            var mapSizes = matches.Where(m => m.TotalSeats > 0)
                .GroupBy(m => m.MapName)
                .Select(
                    g => new
                    {
                        map = g.Key,
                        seats = g.Max(m => m.TotalSeats)
                    })
                    .OrderBy(t => t.seats)
                    .ThenBy(t => t.map)
                    .ToDictionary(t => t.map, t => t.seats);
            
            Console.WriteLine(JsonConvert.SerializeObject(mapSizes, Formatting.Indented));
        }
    }
}