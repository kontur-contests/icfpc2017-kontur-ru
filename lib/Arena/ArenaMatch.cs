namespace lib.Arena
{
    public class ArenaMatch
    {
        public enum MatchStatus
        {
            Waiting,
            InProgress,
            Offline,
            Unknown
        }

        public MatchStatus Status;
        public int TakenSeats;
        public int TotalSeats;
        public string[] Players;
        public string[] Extensions;
        public int Port;
        public string MapName;

        public bool IsSuitableForReplayCollection()
        {
            return Status == MatchStatus.Waiting && TotalSeats - TakenSeats == 1; //&&  MapName.IsOneOf("sample.json", "circle.json", "lambda.json", "Sierpinski-triangle.json");
        }
    }
}