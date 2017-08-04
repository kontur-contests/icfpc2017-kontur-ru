using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace lib
{
	public class Score
	{
		[JsonProperty("punter")]
		public string Punter { get; set; }
		[JsonProperty("score")]
		public int Value { get; set; }
	}
}
