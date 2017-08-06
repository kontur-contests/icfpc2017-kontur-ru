using lib.Structures;

namespace lib.viz
{
    public static class SplurgerHelper
    {
        public static void Update(this long[] spluregerPoints, Move move)
        {
            if (move.pass != null)
                spluregerPoints[move.pass.punter]++;
            else if (move.splurger != null)
                spluregerPoints[move.splurger.punter] -= move.splurger.route.Length - 2;
        }
    }
}