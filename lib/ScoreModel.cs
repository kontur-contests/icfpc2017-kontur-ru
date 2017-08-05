using Newtonsoft.Json;

namespace lib
{
	public class ScoreModel
	{
		[JsonProperty("punter")]
		public int Punter { get; set; }
		[JsonProperty("score")]
		public int Score { get; set; }
	}
}
