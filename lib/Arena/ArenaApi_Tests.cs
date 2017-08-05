using System.Threading.Tasks;
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
        }
    }
}