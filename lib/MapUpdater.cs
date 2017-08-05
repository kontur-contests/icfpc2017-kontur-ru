namespace lib
{
    public class MapUpdater
    {
        public static void ApplyMove(Map map, Move nextMove)
        {
            if (!(nextMove is ClaimMove move)) return;

            foreach (var river in map.Rivers)
                if ((river.Source == move.Source && river.Target == move.Target
                     || river.Target == move.Source && river.Source == move.Target) && river.Owner == -1)
                {
                    river.Owner = move.PunterId;
                    return;
                }
        }
    }
}