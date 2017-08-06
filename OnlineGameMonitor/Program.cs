using System;
using lib.Arena;
using MoreLinq;

namespace OnlineGameMonitor
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var matches = new ArenaApi().GetArenaMatchesAsync()
                .ConfigureAwait(false).GetAwaiter()
                .GetResult();

            matches.ForEach(match =>
            {
                Console.WriteLine(match.Port);
            });
        }
    }
}