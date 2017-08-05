using lib.Structures;
using Newtonsoft.Json;
using NUnit.Framework;

namespace lib
{
    [TestFixture]
    public class Move_Should
    {
        [Test]
        public void BeClaim_WhenDeserializeFromClaim()
        {
            var json = "{\"claim\" : {\"punter\" : 123, \"source\" : 2, \"target\" : 1}}";

            JsonConvert.DeserializeObject<Move>(json);
        }

        [Test]
        public void BePass_WhenDeserializeFromPass()
        {
            var json = "{\"pass\" : {\"punter\" : 123}}";

            JsonConvert.DeserializeObject<Move>(json);
        }
    }
 }