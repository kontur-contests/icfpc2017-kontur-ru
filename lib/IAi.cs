using Newtonsoft.Json.Linq;

namespace lib
{
    public interface IAi
    {
        string Name { get; }
        void StartRound(int punterId, int puntersCount, Map map);
        Move GetNextMove(Move[] prevMoves, Map map);
        string SerializeGameState();
        void DeserializeGameState(string gameState);
    }
}