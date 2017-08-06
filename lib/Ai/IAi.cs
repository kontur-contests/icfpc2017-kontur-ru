using lib.StateImpl;

namespace lib.Ai
{
    public interface IAi
    {
        string Name { get; }
        string Version { get; }
        AiSetupDecision Setup(State state, IServices services);
        AiMoveDecision GetNextMove(State state, IServices services);
    }
}