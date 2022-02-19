using System;
using System.Collections.Generic;

namespace StrAItego.Game.Agents.MCTSAgents.BoardEstimators
{
    class RandomEstimator : BoardEstimator
    {
        // Store result
        Rank[] units = new Rank[40];

        // Vars for new procedure
        int[] remaining = new int[12];
        Piece[] pieceStorage = new Piece[40];

        public RandomEstimator() : base("Random Estimator"){ }

        // Randomly scrambles the remaining units in a way that is still valid.
        public override Board EstimateBoard(Board fromBoard, Random r = null) {
            Board b = new Board();
            return MakeEstimationOnBoard(fromBoard, b, r);
        }

        public Board MakeEstimationOnBoard(Board fromBoard, Board onBoard, Random r = null) {
            // Idea: There are only x many types of pieces:
            // 1: Those with a known rank, e.g. by capture or scout movement
            // 2: Those that have not yet moved and could be bombs/flag or any other rank
            // 3: Remaining pieces that have moved but have not captured, can be any moving rank
            // New procedure:
            // - First, assign the ranks for group 1, see what remains.
            // - Second, assign all bombs and the flag to the set of unmoved pieces, e.g. group 2
            // - Third, from the remaining pieces in group 2 and the entirety of group 3, assign all remaining ranks.
            // Alternative procedure:
            // Grab list of all pieces and shuffle them
            // Go over list once to find all known pieces
            // Go over the list again to assign the flag and the bombs, all in order
            // Then go over the list a third time to assign all the other ranks
            
            // Reset remaining
            remaining[0] = 1;
            remaining[1] = 1;
            remaining[2] = 8;
            remaining[3] = 5;
            remaining[4] = 4;
            remaining[5] = 4;
            remaining[6] = 4;
            remaining[7] = 3;
            remaining[8] = 2;
            remaining[9] = 1;
            remaining[10] = 1;
            remaining[11] = 6;

            // Grab pieces
            fromBoard.GetPieces(Team.Blue).CopyTo(pieceStorage, 0);

            // Shuffle list of pieces
            Shuffle(pieceStorage, r);

            for (int i = 0; i < 40; i++)
                units[i] = Rank.None;

            // First loop, assign all known ranks
            foreach (Piece p in pieceStorage) {
                if (p.IsDiscovered) {
                    remaining[(int)p.Rank - 1]--;
                    units[p.SetupOrigin] = p.Rank;
                }
            }

            // Second loop, assign flag and bombs
            foreach (Piece p in pieceStorage) {
                if (units[p.SetupOrigin] != Rank.None)
                    continue;
                if(remaining[0] > 0) {  // We still need to assign the flag
                    if((p.PotentialRank & PotentialRank.Flag) > 0) { // Could be the flag
                        remaining[0]--;
                        units[p.SetupOrigin] = Rank.Flag;
                    }
                    continue;
                }
                if(remaining[11] > 0) { // We still need to assign bombs
                    if ((p.PotentialRank & PotentialRank.Bomb) > 0) { // Could be the flag
                        remaining[11]--;
                        units[p.SetupOrigin] = Rank.Bomb;
                    }
                    continue;
                }
            }

            // Third loop, assign any remaining ranks
            int k = 0;
            for(int i = 1; i <= 10; i++) {
                for(int j = 0; j < remaining[i]; j++) {
                    while (units[pieceStorage[k].SetupOrigin] != Rank.None)
                        k++;
                    units[pieceStorage[k].SetupOrigin] = (Rank)(i + 1);
                }
            }

            fromBoard.CopyTo(onBoard);
            onBoard.EnterEstimation(units);
            return onBoard;
        }

        public static void Shuffle<T>(T[] array, Random r) {
            int n = array.Length;
            while (n > 1) {
                n--;
                int k = r.Next(n + 1);
                var value = array[k];
                array[k] = array[n];
                array[n] = value;
            }
        }
    }
}
