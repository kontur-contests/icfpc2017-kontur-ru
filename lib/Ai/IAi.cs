namespace lib.Ai
{
    public interface IAi
    {
        string Name { get; }
        string Version { get; }
        Future[] StartRound(int punterId, int puntersCount, Map map, Settings settings);
        Move GetNextMove(Move[] prevMoves, Map map);
        string SerializeGameState();
        void DeserializeGameState(string gameState);
    }
}