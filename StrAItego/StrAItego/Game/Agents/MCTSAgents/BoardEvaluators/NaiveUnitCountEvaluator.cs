using System;

namespace StrAItego.Game.Agents.MCTSAgents.BoardEvaluators
{
    class NaiveUnitCountEvaluator : BoardEvaluator
    {
        public NaiveUnitCountEvaluator() : base("Naive Unit Count") { }

        public override float EvaluateNode(Node n, Random r = null) {
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
    }
}
