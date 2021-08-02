using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrAItego.Game.Agents.MCTSAgents.BoardEvaluators
{
    class NaiveUnitValueCountEvaluator : IBoardEvaluator
    {
        public float EvaluateNode(Node n, Random r = null) {
            if (n.Winner == Team.Red)
                return 1;
            if (n.Winner == Team.Blue)
                return 0;

            bool redHasSpy = n.Board.UnitCount(Rank.Spy, Team.Red) > 0;
            bool redHasMarshal = n.Board.UnitCount(Rank.Marshal, Team.Red) > 0;
            int redBombs = n.Board.UnitCount(Rank.Bomb, Team.Red);

            bool blueHasSpy = n.Board.UnitCount(Rank.Spy, Team.Blue) > 0;
            bool blueHasMarshal = n.Board.UnitCount(Rank.Marshal, Team.Blue) > 0;
            int blueBombs = n.Board.UnitCount(Rank.Bomb, Team.Blue);


            float friendlies = 0;
            for (Rank i = Rank.Flag; i <= Rank.Bomb; i++) {
                friendlies += GetValue(i, n.Board.UnitCount(i, Team.Red), blueHasMarshal, blueHasSpy, blueBombs);
            }

            float enemies = 0;
            for (Rank i = Rank.Flag; i <= Rank.Bomb; i++) {
                enemies += GetValue(i, n.Board.UnitCount(i, Team.Blue), redHasMarshal, redHasSpy, redBombs);
            }
            float score = (friendlies / enemies) / 324;
            return score;
        }

        public int GetValue(Rank i, int count, bool enemyHasMarshal, bool enemyHasSpy, int enemyBombCount) {
            switch (i) {
                case Rank.Flag: return 0;
                case Rank.Spy: if (enemyHasMarshal) return 10 * count; else return 1;
                case Rank.Scout: return count;
                case Rank.Miner: if (enemyBombCount >= 2) return 6 * count; else return 3 * count;
                case Rank.Sergeant: return 4 * count;
                case Rank.Lieutenant: return 5 * count;
                case Rank.Captain: return 12 * count;
                case Rank.Major: return 14 * count;
                case Rank.Colonel: return 16 * count;
                case Rank.General: return 18 * count;
                case Rank.Marshal: if (enemyHasSpy) return 20 * count; else return 40 * count;
                case Rank.Bomb: return 10 * count;
                default: return 0;
            }
        }


        public override string ToString() {
            return "Naive Unit Value Count";
        }
    }
}
