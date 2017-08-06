namespace lib.Arena
{
    public class ArenaMatch
    {
        public static readonly ArenaMatch EmptyMatch = new ArenaMatch();
        
        public enum MatchStatus
        {
            Waiting = 1,
            InProgress = 2,
            Offline = 3,
            Unknown = 4
        }

        public MatchStatus Status;
        public int TakenSeats;
        public int TotalSeats;
        public string[] Players;
        public string[] Extensions;
        public int Port;
        public string MapName;
    }
}