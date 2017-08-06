using System;

namespace lib.StateImpl
{
    public class InvalidDecisionException : Exception
    {
        public string Reason { get; }

        public InvalidDecisionException(string reason, string message, Exception innerException = null) : base(message, innerException)
        {
            Reason = reason;
        }
    }
}