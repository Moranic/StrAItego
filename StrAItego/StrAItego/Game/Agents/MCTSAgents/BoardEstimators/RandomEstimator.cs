using System;
using System.Collections.Generic;

namespace StrAItego.Game.Agents.MCTSAgents.BoardEstimators
{
    class RandomEstimator : IBoardEstimator
    {
        //// Store graph
        //GraphNode source, sink;
        //PieceNode[] pieceNodes;
        //RankNode[] rankNodes;
        //
        // Store BFS structures
        bool[] visited;
        GraphNode[] parent;
        Queue<GraphNode> nodeQueue;
        List<GraphNode> path;
        
        // Store result
        Rank[] units = new Rank[40];

        // Vars for new procedure
        int[] remaining = new int[12];
        Piece[] pieceStorage = new Piece[40];

        public RandomEstimator() {
            ////Create the node objects
            //int nodeID = 0;
            //source = new GraphNode(nodeID++, false);
            //pieceNodes = new PieceNode[40]; // 40 enemy pieces in total
            //rankNodes = new RankNode[12];   // 12 potential ranks to assign to each
            //for (int i = 0; i < 40; i++)
            //    pieceNodes[i] = new PieceNode(nodeID++, i);
            //for (Rank i = Rank.Flag; i <= Rank.Bomb; i++)
            //    rankNodes[(int)i - 1] = new RankNode(nodeID++, i);
            //sink = new GraphNode(nodeID++, true);
            //
            //// Make links
            //foreach (PieceNode pn in pieceNodes) {
            //    source.ConnectNode(pn, 0);
            //    foreach (RankNode rn in rankNodes) {
            //        pn.ConnectNode(rn, 0);
            //    } 
            //}
            //foreach (RankNode rn in rankNodes)
            //    rn.ConnectNode(sink, 0);
            //
            //// Build BFS structures
            //visited = new bool[sink.ID + 1];
            //parent = new GraphNode[sink.ID + 1];
            //nodeQueue = new Queue<GraphNode>();
            //path = new List<GraphNode>();
            //
            //// Store result
            //units = new Rank[40];


        }

        // Randomly scrambles the remaining units in a way that is still valid.
        public Board EstimateBoard(Board fromBoard, Random r = null) {
            Board b = new Board();
            return MakeEstimationOnBoard(fromBoard, b, r);
        }

        public Board MakeEstimationOnBoard(Board fromBoard, Board onBoard, Random r = null) {
            //// Shuffle all edges for randomness
            //source.ShuffleEdges(r);
            //foreach (PieceNode pn in pieceNodes)
            //    pn.ShuffleEdges(r);
            //foreach (RankNode rn in rankNodes)
            //    rn.ShuffleEdges(r);
            //sink.ShuffleEdges(r);
            //
            //// Set capacities
            //foreach (Piece p in fromBoard.GetPieces(Team.Blue)) {
            //    pieceNodes[p.SetupOrigin].SetEdgeCapacities(p.PotentialRank);
            //}
            //foreach (RankNode rn in rankNodes)
            //    rn.SetEdgeCapacities();
            //
            //int maxFlow = 0;
            //BFS(source, sink);
            //while (path.Count > 0) {
            //    // We don't have to find out path flow, it is always 1 (because capacities outgoing from source are all 1).
            //    maxFlow++;
            //    for (int i = 0; i < path.Count - 1; i++) {
            //        GraphNode from = path[i];
            //        GraphNode to = path[i + 1];
            //        // Find related edge and add saturation
            //        foreach(Edge e in from.Edges) {
            //            if((e.From == from && e.To == to) || (e.To == from && e.From == to)) {
            //                e.AddSaturation(e.From == from ? 1 : -1);
            //                break;
            //            }
            //
            //        }
            //    }
            //
            //    BFS(source, sink);
            //}
            //
            ////Rank[] units = pieceNodes.Select(x => ((RankNode)x.Edges.Find(y => y.From == x && y.Capacity != 0 && y.Saturated).To).Rank).ToArray();
            //
            //for(int i = 0; i < 40; i++) {
            //    PieceNode pn = pieceNodes[i];
            //    foreach(Edge e in pn.Edges) {
            //        if (e.Capacity > 0 && e.Saturated && e.To != pn) {
            //            units[i] = ((RankNode)e.To).Rank;
            //            break;
            //        }
            //    }
            //}

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
                if (Board.UnitKnown(p.PotentialRank)) {
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

        List<GraphNode> BFS(GraphNode source, GraphNode sink) {
            // Reset datastructures
            for(int i = 0; i < sink.ID + 1; i++) {
                visited[i] = false;
                parent[i] = null;
            }
            nodeQueue.Clear();

            // Do BFS
            nodeQueue.Enqueue(source);
            visited[source.ID] = true;

            while(nodeQueue.Count > 0) {
                GraphNode u = nodeQueue.Dequeue();
                foreach(Edge e in u.Edges) {
                    GraphNode ind = e.From == u ? e.To : e.From;
                    int val = e.From == u ? e.RemainingCapacity : e.Saturation;
                    if(!visited[ind.ID] && val > 0) {
                        nodeQueue.Enqueue(ind);
                        visited[ind.ID] = true;
                        parent[ind.ID] = u;
                    }
                }
            }
            path.Clear();
            if (visited[sink.ID]) {
                GraphNode n = sink;
                while(n != null) {
                    path.Add(n);
                    n = parent[n.ID];
                }
                path.Reverse();
            }
            return path;
        }

        public override string ToString() {
            return "Random Estimator";
        }

        public void Dispose() {
        }
    }


    class RankNode : GraphNode
    {
        public Rank Rank { get; }
        public PotentialRank PotentialRank { get; }
        public RankNode(int id, Rank rank) : base(id, false) {
            Rank = rank;
            PotentialRank = Board.RankToPotentialRank(Rank);
        }

        public void SetEdgeCapacities() {
            foreach(Edge e in Edges) {
                if(e.From == this) {
                    e.Reset(Board.OriginalOfRank(Rank));
                }
                // else case is handled by PieceNode object
            }
        }

        public override string ToString() {
            return base.ToString() + ", " + Rank;
        }
    }

    class PieceNode : GraphNode
    {
        public int Piece { get; }
        public PieceNode(int id, int piece) : base(id, false) {
            Piece = piece;
        }

        public void SetEdgeCapacities(PotentialRank pr) {
            foreach(Edge e in Edges) {
                if (e.From == this) {
                    if ((((RankNode)e.To).PotentialRank & pr) > 0) {
                        e.Reset(1);
                    }
                    else {
                        e.Reset(0);
                    }
                }
                else  // Source
                    e.Reset(1);
            }
        }

        public override string ToString() {
            return base.ToString() + ", " + Piece;
        }
    }

    class GraphNode
    {
        bool IsSink { get; }

        List<Edge> edges = new List<Edge>();
        public int ID { get; }

        public GraphNode(int id, bool sink = false) {
            IsSink = sink;
            ID = id;
        }

        public void ConnectNode(GraphNode n, int capacity) {
            Edge e = new Edge(this, n, capacity);
            edges.Add(e);
            n.ConnectEdge(e);
        }

        void ConnectEdge(Edge e) {
            edges.Add(e);
        }

        public void ShuffleEdges(Random r) {
            int n = edges.Count;
            while (n > 1) {
                n--;
                int k = r.Next(n + 1);
                Edge value = edges[k];
                edges[k] = edges[n];
                edges[n] = value;
            }
        }

        public List<Edge> Edges { get { return edges; } }

        public override string ToString() {
            return "ID: " + ID + (IsSink ? ", Sink" : "");
        }
    }

    class Edge
    {
        public GraphNode From { get; }
        public GraphNode To { get; }
        public int Saturation { get; private set; } = 0;
        public int Capacity { get; private set; }
        public bool Visited { get; set; }

        public Edge(GraphNode from, GraphNode to, int capacity) {
            From = from;
            To = to;
            Capacity = capacity;
        }

        public bool Saturated {
            get { return Saturation == Capacity; }
        }

        public int RemainingCapacity {
            get { return Capacity - Saturation; }
        }

        public void AddSaturation(int value) {
            Saturation += value;
        }

        public void Reset(int capacity) {
            Saturation = 0;
            Capacity = capacity;
            Visited = false;
        }

        public override string ToString() {
            return From + " -> " + To + " (" + Saturation + "/" + Capacity + ")";
        }
    }
}
