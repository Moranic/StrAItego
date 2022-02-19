using System;

namespace StrAItego.Game.Agents.MCTSAgents.BoardEvaluators
{
    class NaiveExplorerEvaluator : BoardEvaluator
    {
        NaiveUnitCountEvaluator nuce = new NaiveUnitCountEvaluator();

        public NaiveExplorerEvaluator() : base("Naive Explorer") { }

        public override float EvaluateNode(Node n, Random r = null) {
            if (n.Winner == Team.Red)
                return 1;
            if (n.Winner == Team.Blue)
                return 0;

            float unitValue = nuce.EvaluateNode(n, r);

            float friendlies = 0;
            float enemies = 0;
            for(Square i = Square.A1; i <= Square.K10; i++) {
                Team t = n.Board.OnSquare(i);
                if (t == Team.Neither)
                    continue;
                if (t == Team.Red)
                    friendlies += n.Board.CountPossibilitiesOnSquare(i);
                else
                    enemies += n.Board.CountPossibilitiesOnSquare(i);
            }

            float score = ((480 - friendlies) * (480 - friendlies) / 230400 + (enemies * enemies / 230400)) / 2;

            return (unitValue + (score / 20)) / 1.05f;
        }
    }
}
