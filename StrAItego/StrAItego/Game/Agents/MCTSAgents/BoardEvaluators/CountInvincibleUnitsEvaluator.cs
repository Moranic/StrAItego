using System;

namespace StrAItego.Game.Agents.MCTSAgents.BoardEvaluators
{
    class CountInvincibleUnitsEvaluator : IBoardEvaluator
    {
        int[][] remainingPieces = new int[2][];

        public CountInvincibleUnitsEvaluator() {
            for (int i = 0; i <= 1; i++)
                remainingPieces[i] = new int[12];
        }

        public float EvaluateNode(Node n, Random r = null) {
            if (n.Winner == Team.Red)
                return 1;
            if (n.Winner == Team.Blue)
                return 0;

            for(Rank u = Rank.Flag; u <= Rank.Bomb; u++) {
                remainingPieces[1][(int)u - 1] = n.Board.UnitCount(u, Team.Blue);
                remainingPieces[0][(int)u - 1] = n.Board.UnitCount(u, Team.Red);
            }

            int friendlyInvincibles = 1; //Prevent div by 0
            for(int i = 1; i < 10; i++) {
                if (remainingPieces[0][i] > 0) {
                    bool couldKill = false;
                    for (int j = i; j < 11; j++) {
                        if (remainingPieces[1][j] > 0) {
                            couldKill = true;
                            break;
                        }
                    }
                    friendlyInvincibles += couldKill ? 0 : remainingPieces[0][i];
                }
            }

            if (remainingPieces[0][10] > 0 && remainingPieces[1][1] == 0)
                friendlyInvincibles++;

            if (remainingPieces[0][11] > 0 && remainingPieces[1][3] == 0)
                friendlyInvincibles += remainingPieces[0][11];

            int enemyInvincibles = 1;   //Prevent div by 0
            for (int i = 1; i < 10; i++) {
                if (remainingPieces[1][i] > 0) {
                    bool couldKill = false;
                    for (int j = i; j < 11; j++) {
                        if (remainingPieces[0][j] > 0) {
                            couldKill = true;
                            break;
                        }
                    }
                    enemyInvincibles += couldKill ? 0 : remainingPieces[0][i];
                }
            }

            if (remainingPieces[1][10] > 0 && remainingPieces[0][1] == 0)
                enemyInvincibles++;

            if (remainingPieces[1][11] > 0 && remainingPieces[0][3] == 0)
                enemyInvincibles += remainingPieces[0][11];

            return ((float)friendlyInvincibles / enemyInvincibles) / 38;

        }

        public override string ToString() {
            return "Invincible Unit Count";
        }
    }
}
