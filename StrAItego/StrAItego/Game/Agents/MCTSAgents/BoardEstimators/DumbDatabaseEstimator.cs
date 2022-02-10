using System;
using System.Collections.Generic;
using System.Linq;
using StrAItego.Game.Agents.SetupProviders.GravonSetups;

namespace StrAItego.Game.Agents.MCTSAgents.BoardEstimators
{
    class DumbDatabaseEstimator : IBoardEstimator
    {
        static PotentialRank[][] database;
        static int[] dbfrequencies;
        
        // Used for hungarian algorithm
        static PotentialRank[] units = {
            //PotentialRank.Flag,
            //PotentialRank.Spy,
            //PotentialRank.Scout, PotentialRank.Scout, PotentialRank.Scout, PotentialRank.Scout, PotentialRank.Scout, PotentialRank.Scout, PotentialRank.Scout, PotentialRank.Scout,
            //PotentialRank.Miner, PotentialRank.Miner, PotentialRank.Miner, PotentialRank.Miner, PotentialRank.Miner,
            //PotentialRank.Sergeant, PotentialRank.Sergeant, PotentialRank.Sergeant, PotentialRank.Sergeant,
            //PotentialRank.Lieutenant, PotentialRank.Lieutenant, PotentialRank.Lieutenant, PotentialRank.Lieutenant,
            //PotentialRank.Captain, PotentialRank.Captain, PotentialRank.Captain, PotentialRank.Captain,
            //PotentialRank.Major, PotentialRank.Major, PotentialRank.Major,
            //PotentialRank.Colonel, PotentialRank.Colonel,
            //PotentialRank.General,
            //PotentialRank.Marshal,
            //PotentialRank.Bomb, PotentialRank.Bomb, PotentialRank.Bomb, PotentialRank.Bomb, PotentialRank.Bomb, PotentialRank.Bomb
            PotentialRank.Miner, PotentialRank.Bomb, PotentialRank.Flag, PotentialRank.Bomb, PotentialRank.Lieutenant, PotentialRank.Sergeant, PotentialRank.Bomb, PotentialRank.Sergeant, PotentialRank.Bomb, PotentialRank.Sergeant,
            PotentialRank.Major, PotentialRank.Miner, PotentialRank.Bomb, PotentialRank.Colonel, PotentialRank.Miner, PotentialRank.Lieutenant, PotentialRank.General, PotentialRank.Bomb, PotentialRank.Miner, PotentialRank.Scout,
            PotentialRank.Lieutenant, PotentialRank.Colonel, PotentialRank.Captain, PotentialRank.Scout, PotentialRank.Scout, PotentialRank.Captain, PotentialRank.Major, PotentialRank.Miner, PotentialRank.Lieutenant, PotentialRank.Captain,
            PotentialRank.Scout, PotentialRank.Scout, PotentialRank.Sergeant, PotentialRank.Marshal, PotentialRank.Captain, PotentialRank.Scout, PotentialRank.Spy, PotentialRank.Major, PotentialRank.Scout, PotentialRank.Scout
            };

        static DumbDatabaseEstimator() {
            string data = GravonSetup.data;
            Dictionary<string, int> datadict = new Dictionary<string, int>(data.Length / 40);
            
            dbfrequencies = new int[data.Length / 40];
            for(int i = 0; i < data.Length / 40; i++) {
                string setup = data.Substring(i * 40, 40);
                datadict.TryGetValue(setup, out int count);
                datadict[setup] = ++count;
            }
            var dbarr = datadict.ToArray();
            dbarr = dbarr.OrderByDescending(x => x.Value).ToArray();
            database = new PotentialRank[dbarr.Length][];
            int k = 0;
            foreach (var x in dbarr) {
                database[k] = StringToPotentialRanks(x.Key);
                dbfrequencies[k++] = x.Value;
            }
        }

        public void Dispose() {
            return;
        }

        public Board EstimateBoard(Board fromBoard, Random r = null) {
            PotentialRank[] info = fromBoard.GetPieces(Team.Blue).Select(x => x.PotentialRank).ToArray();
            PotentialRank[] match = SearchDatabase(info);


            Rank[] newRanks = new Rank[40];
            if (match == null) {
                match = SearchDatabaseForClosestMatch(info);
                int[,] costs = new int[40, 40];
                for(int i = 0; i < 40; i++) {
                    PotentialRank pinfo = info[i];
                    PotentialRank pmatch = match[i];
                    for(int j = 0; j < 40; j++) {
                        PotentialRank uRank = match[j];
                        if((pinfo & uRank) == 0) {
                            // Incompatible rank
                            costs[i, j] = 10000000;
                        }
                        else if((pinfo & pmatch) == 0) {
                            costs[i, j] = 100;
                        }
                    }
                }
                int[] assignments = HungarianAlgorithm.HungarianAlgorithm.FindAssignments(costs);
                for (int i = 0; i < 40; i++)
                    newRanks[i] = Board.PotentialRankToRank(match[assignments[i]]);
            }
            else {
                newRanks = match.Select(x => Board.PotentialRankToRank(x)).ToArray();
            }
            Board newBoard = new Board(fromBoard);
            newBoard.EnterEstimation(newRanks);
            return newBoard;
        }

        private static PotentialRank[] SearchDatabase(PotentialRank[] info) {
            bool found = false;
            for (int i = 0; i < database.Length; i++) {
                for (int j = 0; j < 40; j++) {
                    // Check if match
                    if (info == null || database[i] == null)
                        throw new Exception("wut");
                    if ((info[j] & database[i][j]) == 0)
                        break;
                    if (j == 39)
                        found = true;
                }
                if(found)
                    return database[i];
            }
            return null;
        }

        private static PotentialRank[] SearchDatabaseForClosestMatch(PotentialRank[] info) {
            int best = -1;
            int bestind = -1;
            for (int i = 0; i < database.Length; i++) {
                int curr = 0;
                for (int j = 0; j < 40; j++) {
                    // Check compatibility
                    if ((info[j] & database[i][j]) > 0)
                        curr++;
                    if (curr + (39 - j) < best)
                        break;  // We can't possibly improve on the best anymore.
                }

                if (curr > best) {
                    bestind = i;
                    best = curr;
                }
            }
            return database[bestind];
        }

        public override string ToString() {
            return "'Dumb' Database Estimator";
        }

        static PotentialRank[] StringToPotentialRanks(string setup) {
            PotentialRank[] conv = new PotentialRank[setup.Length];
            for(int i = 0; i < setup.Length; i++) {
                conv[i] = setup[i] switch {
                    'B' => PotentialRank.Bomb,
                    '1' => PotentialRank.Spy,
                    '2' => PotentialRank.Scout,
                    '3' => PotentialRank.Miner,
                    '4' => PotentialRank.Sergeant,
                    '5' => PotentialRank.Lieutenant,
                    '6' => PotentialRank.Captain,
                    '7' => PotentialRank.Major,
                    '8' => PotentialRank.Colonel,
                    '9' => PotentialRank.General,
                    'M' => PotentialRank.Marshal,
                    'F' => PotentialRank.Flag,
                    _ => throw new ArgumentException("Unknown symbol encountered! " + setup[i])
                };
            }
            return conv;
        }
    }
}
