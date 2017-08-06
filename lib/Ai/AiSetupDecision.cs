using lib.Structures;

namespace lib.Ai
{
    public class AiSetupDecision
    {
        public readonly Future[] futures;
        public readonly string reason;

        private AiSetupDecision(Future[] futures, string reason = null)
        {
            this.futures = futures;
            this.reason = reason;
        }

        public static AiSetupDecision Create(Future[] futures, string reason = null)
        {
            return new AiSetupDecision(futures, reason);
        }

        public static AiSetupDecision Empty(string reason = null)
        {
            return new AiSetupDecision(new Future[0], reason);
        }
    }
}