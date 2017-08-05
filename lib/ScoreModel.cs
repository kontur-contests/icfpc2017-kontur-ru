using Newtonsoft.Json;

namespace lib
{
	public class ScoreModel
	{
		[JsonProperty("punter")]
		public int Punter { get; set; }
		[JsonProperty("score")]
		public long Score { get; set; }

		public override string ToString()
		{
			return $"{nameof(Punter)}: {Punter}, {nameof(Score)}: {Score}";
		}
	}
}
