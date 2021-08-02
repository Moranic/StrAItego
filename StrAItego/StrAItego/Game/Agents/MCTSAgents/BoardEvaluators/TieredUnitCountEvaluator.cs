using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrAItego.Game.Agents.MCTSAgents.BoardEvaluators
{
    class TieredUnitCountEvaluator : IBoardEvaluator
    {
        public float EvaluateNode(Node n, Random r = null) {
            if (n.Winner == Team.Red)
                return 1;
            if (n.Winner == Team.Blue)
                return 0;

            float score = 0;

            for(Rank i = Rank.Spy; i <= Rank.Bomb; i++) {
                int cr = n.Board.UnitCount(i, Team.Red);
                int cb = n.Board.UnitCount(i, Team.Blue);
                switch (i) {
                    case Rank.Spy:
                        score += (1 + (cr - cb)) / 200000000000f;
                        break;
                    case Rank.Scout:
                        score += (1 + (cr - cb) / 8f) / 20000000000f;
                        break;
                    case Rank.Miner:
                        score += (1 + (cr - cb) / 5f) / 2000000000f;
                        break;
                    case Rank.Sergeant:
                        score += (1 + (cr - cb) / 4f) / 20000000f;
                        break;
                    case Rank.Lieutenant:
                        score += (1 + (cr - cb) / 4f) / 2000000f;
                        break;
                    case Rank.Captain:
                        score += (1 + (cr - cb) / 4f) / 200000f;
                        break;
                    case Rank.Major:
                        score += (1 + (cr - cb) / 3f) / 20000f;
                        break;
                    case Rank.Colonel:
                        score += (1 + (cr - cb) / 2f) / 2000f;
                        break;
                    case Rank.General:
                        score += (1 + (cr - cb)) / 20f;
                        break;
                    case Rank.Marshal:
                        score += (1 + (cr - cb)) / 2f;
                        break;
                    case Rank.Bomb:
                        score += (1 + (cr - cb)) / 200000000f;
                        break;
                    default: throw new Exception("Unknown rank");
                }
            }
            
            return score / 2f;
        }

        public override string ToString() {
            return "Tiered Unit Count";
        }
    }
}
