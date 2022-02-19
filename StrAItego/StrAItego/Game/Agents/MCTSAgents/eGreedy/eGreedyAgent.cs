using System.Collections.Generic;

namespace StrAItego.Game.Agents.MCTSAgents.eGreedy
{
    class eGreedyAgent : MCTSAgent
    {
        float e;
        public eGreedyAgent() : base("eGreedy Agent") { }

        public override IAgentParameters GetParameters() {
            return new eGreedyAgentParameters();
        }

        protected override Move SelectMove(Node n) {
            List<Move> moves = n.GetMoves();
            if(r.NextDouble() <= e) {   // Pick random
                return moves[r.Next(moves.Count)];
            }
            else {                      // Pick best
                (Move, float) bestMove = (moves[0], n.TurnOfTeam == Team.Red ? 0f : 1f);
                foreach (Move m in moves) {
                    (float, int) result = n.GetScoreOfMove(m);
                    float score = result.Item1 / result.Item2;
                    if (n.TurnOfTeam == Team.Red ? score > bestMove.Item2 : score < bestMove.Item2) {
                        bestMove = (m, score);
                    }
                }

                return bestMove.Item1;
            }
        }

        public override void SetParameters(IAgentParameters agentParameters) {
            base.SetParameters(agentParameters);
            eGreedyAgentParameters parameters = (eGreedyAgentParameters)agentParameters;
            e = parameters.Epsilon;
            name = parameters.ToString();
        }
    }
}
