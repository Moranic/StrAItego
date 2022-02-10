using System;
using System.Collections.Generic;
using StrAItego.Game.Agents.SetupProviders;

namespace StrAItego.Game.Agents.RandomAgent
{
    public class RandomAgent : IAgent
    {
        protected Random r;
        protected string name = "Random Agent";
        ISetupProvider setupProvider;
        public RandomAgent() {
        }

        public RandomAgent(int seed) {
            r = new Random(seed);
        }

        public virtual Move? GetMove(Board board, GameLogger gameLogger) {
            List<Move> moves = board.GetValidMoves(Team.Red);
            if (moves.Count == 0)
                return null;
            return moves[r.Next(moves.Count)];
        }

        public virtual IAgentParameters GetParameters() {
            return new RandomAgentParameters();
        }

        public virtual Rank[] GetSetup(Board board) {
            return setupProvider.GetSetup(r);
        }

        public bool IsAI() {
            return true;
        }

        public virtual void SetParameters(IAgentParameters agentParameters) {
            RandomAgentParameters parameters = agentParameters as RandomAgentParameters;
            r = new Random(parameters.GetSeed());
            name = parameters.ToString();
            setupProvider = parameters.GetSetupProvider;
        }

        public override string ToString() {
            return name;
        }

        public virtual void Dispose() {
            r = null;
            setupProvider?.Dispose();
        }
    }
}
