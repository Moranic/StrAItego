using StrAItego.Game.TFLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrAItego.Game.Agents.MCTSAgents.BoardEstimators
{
    class DirectRankEstimator : IBoardEstimator
    {
        TFLiteModel model;
        float[] binsetup = new float[480];
        static Rank[] units = {
            Rank.Flag,
            Rank.Spy,
            Rank.Scout, Rank.Scout, Rank.Scout, Rank.Scout, Rank.Scout, Rank.Scout, Rank.Scout, Rank.Scout,
            Rank.Miner, Rank.Miner, Rank.Miner, Rank.Miner, Rank.Miner,
            Rank.Sergeant, Rank.Sergeant, Rank.Sergeant, Rank.Sergeant,
            Rank.Lieutenant, Rank.Lieutenant, Rank.Lieutenant, Rank.Lieutenant,
            Rank.Captain, Rank.Captain, Rank.Captain, Rank.Captain,
            Rank.Major, Rank.Major, Rank.Major,
            Rank.Colonel, Rank.Colonel,
            Rank.General,
            Rank.Marshal,
            Rank.Bomb, Rank.Bomb, Rank.Bomb, Rank.Bomb, Rank.Bomb, Rank.Bomb
            };

        static int[] startingIndexOfRank = { 0, 0, 1, 2, 10, 15, 19, 23, 27, 30, 32, 33, 34, 40 };

        public Board EstimateBoard(Board fromBoard, Random r = null) {
            if (model == null) {
                model = TFLiteManager.GetModel("DirectRankEstimator1");
            }
            
            binsetup = fromBoard.PiecesToBinary(Team.Blue, binsetup);

            float[][] pred = model.PredictMultipleOutputs(binsetup, 40);
            Piece[] pieces = fromBoard.GetPieces(Team.Blue);

            int[,] costs = new int[40, 40];
            int[] seenOfRank = new int[13];
            int[] knownOfRank = new int[13];
            for(int i = 0; i < 40; i++) {
                if (Board.UnitKnown(pieces[i].PotentialRank)) {
                    knownOfRank[(int)pieces[i].Rank]++;
                }
            }

            for(int i = 0; i < 40; i++) {
                // For each piece we assign the costs from pred
                float[] prob = pred[i];
                // Probabilities need to be converted to integer costs. Scale between 0 and 1.000.000
                int[] rankcost = new int[12];
                for (int j = 0; j < 12; j++)
                    rankcost[j] = 1000000 - (int)(prob[j] * 1000000);
                Piece p = pieces[i];
                if (Board.UnitKnown(p.PotentialRank)) {
                    for(int j = 0; j < 40; j++) {
                        costs[i, j] = 10000000;
                    }
                    costs[i, startingIndexOfRank[(int)p.Rank] + seenOfRank[(int)p.Rank]++] = 0;
                }
                else {
                    for(int j = 1; j < 13; j++) {
                        int known = knownOfRank[j];
                        for (int k = 0; k < known; k++)
                            costs[i, startingIndexOfRank[j] + k] = 10000000;
                        for (int k = startingIndexOfRank[j] + known; k < startingIndexOfRank[j + 1]; k++)
                            costs[i, k] = rankcost[j - 1];
                    }
                }
            }

            int[] assignment = HungarianAlgorithm.HungarianAlgorithm.FindAssignments(costs);

            Rank[] newRanks = new Rank[40];
            for (int i = 0; i < 40; i++)
                newRanks[i] = units[assignment[i]];

            //Sanity check
            for(int i = 0; i < 40; i++) {
                if (Board.UnitKnown(pieces[i].PotentialRank) && pieces[i].Rank != newRanks[i])
                    throw new Exception("Replaced known piece rank!");
            }

            Board newBoard = new Board(fromBoard);
            newBoard.EnterEstimation(newRanks);
            // Return found board
            return newBoard;
        }

        public static unsafe float Int64BitsToSingle(long value) {
            return *(float*)(&value);
        }

        public override string ToString() {
            return "Direct Rank Estimator";
        }

        public void Dispose() {
            model?.Dispose();
        }
    }
}

namespace HungarianAlgorithm
{
    /// <summary>
    /// Hungarian Algorithm.
    /// </summary>
    public static class HungarianAlgorithm
    {
        /// <summary>
        /// Finds the optimal assignments for a given matrix of agents and costed tasks such that the total cost is minimized.
        /// </summary>
        /// <param name="costs">A cost matrix; the element at row <em>i</em> and column <em>j</em> represents the cost of agent <em>i</em> performing task <em>j</em>.</param>
        /// <returns>A matrix of assignments; the value of element <em>i</em> is the column of the task assigned to agent <em>i</em>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="costs"/> is null.</exception>
        public static int[] FindAssignments(this int[,] costs) {
            if (costs == null)
                throw new ArgumentNullException(nameof(costs));

            var h = costs.GetLength(0);
            var w = costs.GetLength(1);

            for (var i = 0; i < h; i++) {
                var min = int.MaxValue;

                for (var j = 0; j < w; j++) {
                    min = Math.Min(min, costs[i, j]);
                }

                for (var j = 0; j < w; j++) {
                    costs[i, j] -= min;
                }
            }

            var masks = new byte[h, w];
            var rowsCovered = new bool[h];
            var colsCovered = new bool[w];

            for (var i = 0; i < h; i++) {
                for (var j = 0; j < w; j++) {
                    if (costs[i, j] == 0 && !rowsCovered[i] && !colsCovered[j]) {
                        masks[i, j] = 1;
                        rowsCovered[i] = true;
                        colsCovered[j] = true;
                    }
                }
            }

            HungarianAlgorithm.ClearCovers(rowsCovered, colsCovered, w, h);

            var path = new Location[w * h];
            var pathStart = default(Location);
            var step = 1;

            while (step != -1) {
                switch (step) {
                    case 1:
                        step = HungarianAlgorithm.RunStep1(masks, colsCovered, w, h);
                        break;

                    case 2:
                        step = HungarianAlgorithm.RunStep2(costs, masks, rowsCovered, colsCovered, w, h, ref pathStart);
                        break;

                    case 3:
                        step = HungarianAlgorithm.RunStep3(masks, rowsCovered, colsCovered, w, h, path, pathStart);
                        break;

                    case 4:
                        step = HungarianAlgorithm.RunStep4(costs, rowsCovered, colsCovered, w, h);
                        break;
                }
            }

            var agentsTasks = new int[h];

            for (var i = 0; i < h; i++) {
                for (var j = 0; j < w; j++) {
                    if (masks[i, j] == 1) {
                        agentsTasks[i] = j;
                        break;
                    }
                }
            }

            return agentsTasks;
        }

        private static int RunStep1(byte[,] masks, bool[] colsCovered, int w, int h) {
            if (masks == null)
                throw new ArgumentNullException(nameof(masks));

            if (colsCovered == null)
                throw new ArgumentNullException(nameof(colsCovered));

            for (var i = 0; i < h; i++) {
                for (var j = 0; j < w; j++) {
                    if (masks[i, j] == 1)
                        colsCovered[j] = true;
                }
            }

            var colsCoveredCount = 0;

            for (var j = 0; j < w; j++) {
                if (colsCovered[j])
                    colsCoveredCount++;
            }

            if (colsCoveredCount == h)
                return -1;

            return 2;
        }
        private static int RunStep2(int[,] costs, byte[,] masks, bool[] rowsCovered, bool[] colsCovered, int w, int h, ref Location pathStart) {
            if (costs == null)
                throw new ArgumentNullException(nameof(costs));

            if (masks == null)
                throw new ArgumentNullException(nameof(masks));

            if (rowsCovered == null)
                throw new ArgumentNullException(nameof(rowsCovered));

            if (colsCovered == null)
                throw new ArgumentNullException(nameof(colsCovered));

            while (true) {
                var loc = HungarianAlgorithm.FindZero(costs, rowsCovered, colsCovered, w, h);
                if (loc.row == -1)
                    return 4;

                masks[loc.row, loc.column] = 2;

                var starCol = HungarianAlgorithm.FindStarInRow(masks, w, loc.row);
                if (starCol != -1) {
                    rowsCovered[loc.row] = true;
                    colsCovered[starCol] = false;
                }
                else {
                    pathStart = loc;
                    return 3;
                }
            }
        }
        private static int RunStep3(byte[,] masks, bool[] rowsCovered, bool[] colsCovered, int w, int h, Location[] path, Location pathStart) {
            if (masks == null)
                throw new ArgumentNullException(nameof(masks));

            if (rowsCovered == null)
                throw new ArgumentNullException(nameof(rowsCovered));

            if (colsCovered == null)
                throw new ArgumentNullException(nameof(colsCovered));

            var pathIndex = 0;
            path[0] = pathStart;

            while (true) {
                var row = HungarianAlgorithm.FindStarInColumn(masks, h, path[pathIndex].column);
                if (row == -1)
                    break;

                pathIndex++;
                path[pathIndex] = new Location(row, path[pathIndex - 1].column);

                var col = HungarianAlgorithm.FindPrimeInRow(masks, w, path[pathIndex].row);

                pathIndex++;
                path[pathIndex] = new Location(path[pathIndex - 1].row, col);
            }

            HungarianAlgorithm.ConvertPath(masks, path, pathIndex + 1);
            HungarianAlgorithm.ClearCovers(rowsCovered, colsCovered, w, h);
            HungarianAlgorithm.ClearPrimes(masks, w, h);

            return 1;
        }
        private static int RunStep4(int[,] costs, bool[] rowsCovered, bool[] colsCovered, int w, int h) {
            if (costs == null)
                throw new ArgumentNullException(nameof(costs));

            if (rowsCovered == null)
                throw new ArgumentNullException(nameof(rowsCovered));

            if (colsCovered == null)
                throw new ArgumentNullException(nameof(colsCovered));

            var minValue = HungarianAlgorithm.FindMinimum(costs, rowsCovered, colsCovered, w, h);

            for (var i = 0; i < h; i++) {
                for (var j = 0; j < w; j++) {
                    if (rowsCovered[i])
                        costs[i, j] += minValue;
                    if (!colsCovered[j])
                        costs[i, j] -= minValue;
                }
            }
            return 2;
        }

        private static int FindMinimum(int[,] costs, bool[] rowsCovered, bool[] colsCovered, int w, int h) {
            if (costs == null)
                throw new ArgumentNullException(nameof(costs));

            if (rowsCovered == null)
                throw new ArgumentNullException(nameof(rowsCovered));

            if (colsCovered == null)
                throw new ArgumentNullException(nameof(colsCovered));

            var minValue = int.MaxValue;

            for (var i = 0; i < h; i++) {
                for (var j = 0; j < w; j++) {
                    if (!rowsCovered[i] && !colsCovered[j])
                        minValue = Math.Min(minValue, costs[i, j]);
                }
            }

            return minValue;
        }
        private static int FindStarInRow(byte[,] masks, int w, int row) {
            if (masks == null)
                throw new ArgumentNullException(nameof(masks));

            for (var j = 0; j < w; j++) {
                if (masks[row, j] == 1)
                    return j;
            }

            return -1;
        }
        private static int FindStarInColumn(byte[,] masks, int h, int col) {
            if (masks == null)
                throw new ArgumentNullException(nameof(masks));

            for (var i = 0; i < h; i++) {
                if (masks[i, col] == 1)
                    return i;
            }

            return -1;
        }
        private static int FindPrimeInRow(byte[,] masks, int w, int row) {
            if (masks == null)
                throw new ArgumentNullException(nameof(masks));

            for (var j = 0; j < w; j++) {
                if (masks[row, j] == 2)
                    return j;
            }

            return -1;
        }
        private static Location FindZero(int[,] costs, bool[] rowsCovered, bool[] colsCovered, int w, int h) {
            if (costs == null)
                throw new ArgumentNullException(nameof(costs));

            if (rowsCovered == null)
                throw new ArgumentNullException(nameof(rowsCovered));

            if (colsCovered == null)
                throw new ArgumentNullException(nameof(colsCovered));

            for (var i = 0; i < h; i++) {
                for (var j = 0; j < w; j++) {
                    if (costs[i, j] == 0 && !rowsCovered[i] && !colsCovered[j])
                        return new Location(i, j);
                }
            }

            return new Location(-1, -1);
        }
        private static void ConvertPath(byte[,] masks, Location[] path, int pathLength) {
            if (masks == null)
                throw new ArgumentNullException(nameof(masks));

            if (path == null)
                throw new ArgumentNullException(nameof(path));

            for (var i = 0; i < pathLength; i++) {
                if (masks[path[i].row, path[i].column] == 1) {
                    masks[path[i].row, path[i].column] = 0;
                }
                else if (masks[path[i].row, path[i].column] == 2) {
                    masks[path[i].row, path[i].column] = 1;
                }
            }
        }
        private static void ClearPrimes(byte[,] masks, int w, int h) {
            if (masks == null)
                throw new ArgumentNullException(nameof(masks));

            for (var i = 0; i < h; i++) {
                for (var j = 0; j < w; j++) {
                    if (masks[i, j] == 2)
                        masks[i, j] = 0;
                }
            }
        }
        private static void ClearCovers(bool[] rowsCovered, bool[] colsCovered, int w, int h) {
            if (rowsCovered == null)
                throw new ArgumentNullException(nameof(rowsCovered));

            if (colsCovered == null)
                throw new ArgumentNullException(nameof(colsCovered));

            for (var i = 0; i < h; i++) {
                rowsCovered[i] = false;
            }

            for (var j = 0; j < w; j++) {
                colsCovered[j] = false;
            }
        }

        private struct Location
        {
            internal readonly int row;
            internal readonly int column;

            internal Location(int row, int col) {
                this.row = row;
                this.column = col;
            }
        }
    }
}