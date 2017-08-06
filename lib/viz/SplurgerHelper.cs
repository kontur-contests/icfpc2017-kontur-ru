using lib.Structures;

namespace lib.viz
{
    public static class SplurgerHelper
    {
        public static void Update(this long[] spluregerPoints, Move move)
        {
            if (move.pass != null)
                spluregerPoints[move.pass.punter]++;
            else if (move.splurge != null)
                spluregerPoints[move.splurge.punter] -= move.splurge.route.Length - 2;
        }
    }
}