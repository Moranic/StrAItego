using System.Collections.Generic;
using System.Linq;

namespace StrAItego.Game.Agents.RandomAgent.RandomAvoidDefeats
{
    class RandomAvoidsUnitLossAgent : RandomAgent
    {
        public RandomAvoidsUnitLossAgent() : base("Random (Avoids Attacker Loss) Agent") { }
        public override Move? GetMove(Board board, GameLogger gameLogger) {
            List<Move> moves = board.GetValidMoves(Team.Red);
            if (moves.Count == 0)
                return null;
            List<Move> noDefeatMoves = moves.Where(x => (x.InfoOfDefender.IsDiscovered() &&
                                                        x.Attacker.Attacks(x.Defender) == Outcome.Victory) ||
                                                        !x.InfoOfDefender.IsDiscovered()).ToList();
            if(noDefeatMoves.Count == 0)
                return moves[r.Next(moves.Count)];
            return noDefeatMoves[r.Next(noDefeatMoves.Count)];
        }

        public override IAgentParameters GetParameters() {
            return new RandomAvoidsUnitLossAgentParameters();
        }

        public override void SetParameters(IAgentParameters agentParameters) {
            base.SetParameters(agentParameters);
            RandomAvoidsUnitLossAgentParameters parameters = agentParameters as RandomAvoidsUnitLossAgentParameters;
            name = parameters.ToString();
        }
    }
}
