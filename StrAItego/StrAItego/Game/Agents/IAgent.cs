namespace StrAItego.Game.Agents
{
    public interface IAgent {
        Move? GetMove(Board board, GameLogger gameLogger);

        Rank[] GetSetup(Board board);

        string ToString();

        IAgentParameters GetParameters();

        void SetParameters(IAgentParameters agentParameters);

        bool IsAI();

        void Dispose();
    }
}
