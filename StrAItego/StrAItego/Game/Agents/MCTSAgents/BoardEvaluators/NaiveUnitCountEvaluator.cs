using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrAItego.Game.Agents.MCTSAgents.BoardEvaluators
{
    class NaiveUnitCountEvaluator : IBoardEvaluator
    {
        public float EvaluateNode(Node n, Random r = null) {
            if (n.Winner == Team.Red)
                return 1;
            if (n.Winner == Team.Blue)
                return 0;

            float friendlies = 0;
            for (Rank i = Rank.Flag; i <= Rank.Bomb; i++)
                friendlies += n.Board.UnitCount(i, Team.Red);

            float enemies = 0;
            for (Rank i = Rank.Flag; i <= Rank.Bomb; i++)
                enemies += n.Board.UnitCount(i, Team.Blue);

            float score = 1f - (float)Math.Pow(1 - ((friendlies / enemies) / 40), 8);
            return score;
        }

        public override string ToString() {
            return "Naive Unit Count";
        }
    }
}
