using System;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace lib
{
    public interface IStringIransport
    {
        void Write(string data);
        string Read();
    }

    public interface IGameTransport
    {
        void HandShake(string name);
        Setup RequestSetup();
        Move[] ReadMoves();
        void SendMove(Move move);
    }

	public class GameTransport : IGameTransport
	{
		public GameTransport(IStringIransport transport)
		{
			this.transport = transport;
		}

	    public void HandShake(string name)
	    {
	        transport.Write($"{{\"me\":\"{name}\"}}");
		    var answer = transport.Read();
			if (!answer.Contains($"\"{name}\""))
				throw new InvalidOperationException($"Incorrect server handsnake: {answer}");
	    }

	    public Setup RequestSetup()
	    {
		    var data = JsonConvert.DeserializeObject<Setup>(transport.Read());
			transport.Write($"{{\"ready\":\"{data.OurId}\"}}");
		    return data;
	    }

	    public Move[] ReadMoves()
	    {
		    var data = JsonConvert.DeserializeObject<MoveServerData>(transport.Read());
		    return data.Moves.Moves;
	    }

	    public void SendMove(Move move)
	    {
		    var data = JsonConvert.SerializeObject(move);
			transport.Write(data);
	    }

		private class MoveServerData
		{
			[JsonProperty("move")]
			public MoveDataData Moves { get; set; }

			public class MoveDataData
			{
				[JsonProperty("moves")]
				public Move[] Moves { get; set; }
			}
		}

		private class MoveClientDataClaim
		{
			
		}

		private readonly IStringIransport transport;
	}

	public class Setup
	{
		[JsonProperty("punter")]
		public string OurId { get; set; }
		[JsonProperty("punters")]
		public int PunterCount { get; set; }
		[JsonProperty("map")]
		public Map Map { get; set; }
	}
}