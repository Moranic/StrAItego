using System;
using System.Collections.Generic;
using StrAItego.Game.Agents.SetupProviders;

namespace StrAItego.Game.Agents.RandomAgent
{
    public class RandomAgent : BaseAgent
    {
        protected Random r;
        ISetupProvider setupProvider;

        public RandomAgent() : base("Random Agent") { }

        public RandomAgent(string agentName = "Random Agent") : base(agentName) { }

        public RandomAgent(int seed, string agentName = "Random Agent") : base(agentName)
        {
            r = new Random(seed);
        }

        public override Move? GetMove(Board board, GameLogger gameLogger) {
            List<Move> moves = board.GetValidMoves(Team.Red);
            if (moves.Count == 0)
                return null;
            return moves[r.Next(moves.Count)];
        }

        public override IAgentParameters GetParameters() {
            return new RandomAgentParameters();
        }

        public override Rank[] GetSetup(Board board) {
            return setupProvider.GetSetup(r);
        }

        public override void SetParameters(IAgentParameters agentParameters) {
            RandomAgentParameters parameters = agentParameters as RandomAgentParameters;
            r = new Random(parameters.GetSeed());
            name = parameters.ToString();
            setupProvider = parameters.GetSetupProvider;
        }

        public override void Dispose() {
            r = null;
            setupProvider?.Dispose();
        }
    }
}
