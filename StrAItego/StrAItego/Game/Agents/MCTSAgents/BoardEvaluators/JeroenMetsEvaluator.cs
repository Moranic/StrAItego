using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrAItego.Game.Agents.MCTSAgents.BoardEvaluators
{
    class JeroenMetsEvaluator : IBoardEvaluator
    {
        

        public float EvaluateNode(Node n, Random r = null) {
            if (n.Winner == Team.Red)
                return 1;
            if (n.Winner == Team.Blue)
                return 0;

            Board b = n.Board;

            int redPoints = 11087;
            int bluePoints = 11087;

            for(Square i = Square.A1; i <= Square.K10; i++) {
                Piece p = b.OnSquare(i);
                if (p == null)
                    continue;
                
                // Determine points for this piece
                int points = 0;
                if (!p.HasMoved && !(p.Rank == Rank.Bomb || p.Rank == Rank.Flag))
                    points += 100;
                if (!Board.UnitKnown(p.PotentialRank))
                    points += discoveryValue[(int)p.Rank - 1];
                points += conquestValue[(int)p.Rank - 1];

                if (p.Team == Team.Red)
                    bluePoints -= points;
                else
                    redPoints -= points;
            }

            float score = (redPoints / (float)bluePoints) / 11087f;
            return score;
        }

        public override string ToString() {
            return "Jeroen Mets Evaluator";
        }

        static int[] discoveryValue = new int[] { 0, 0, 0, 20, 5, 10, 15, 20, 25, 50, 100, 200 };
        static int[] conquestValue = new int[] { 0, 100, 2, 50, 5, 10, 20, 50, 100, 250, 500, 750 };
    }
}
/*
 * In Monte Carlo Stratego (Mets 2008), Jeroen Mets describes an evaluation function. In it, he provides the following values:
 * Rank         Discovered      Conquered       Moved    Total per piece    Total
 * Bomb (B)            200            750           -                950     5700
 * Marshal (10)        100            500         100                700      700
 * General (9)          50            250         100                400      400
 * Colonel (8)          25            100         100                225      450
 * Major (7)            20             50         100                170      510
 * Captain (6)          15             20         100                135      540
 * Lieutenant (5)       10             10         100                120      480
 * Sergeant (4)         5               5         100                110      440
 * Miner (3)            20             50         100                170      850
 * Scout                 -              2         100                102      816
 * Spy                   -            100         100                200      200
 * Flag                  -           1000           -               1000     1000
 * 
 * Excluding the flag, the total amount of points on one side is 11086.
 * As every positive point for player A is a negative point for player B, -11086 should be 0, 0 should be 0.5 and +11086 should be 1.
 * Obtaining the flag also sets the score to 0 or 1. 
 */
