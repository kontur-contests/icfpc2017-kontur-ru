using Newtonsoft.Json;

namespace lib
{
    public interface IMove
    {
    }

    public class AbstractMove : IMove
    {
        [JsonProperty("punter")]
        public int PunterId;
    }

    public class Pass : AbstractMove
    {
    }

    public class Move : AbstractMove
    {
        [JsonProperty("source")]
        public int Source;

        [JsonProperty("target")]
        public int Target;

        public Move()
        {
        }

        public Move(int source, int target)
        {
            Source = source;
            Target = target;
        }
    }
}