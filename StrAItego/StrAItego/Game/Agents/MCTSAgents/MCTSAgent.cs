using System;
using System.Collections.Generic;
using System.Diagnostics;
using StrAItego.Game.Agents.MCTSAgents.BoardEstimators;
using StrAItego.Game.Agents.MCTSAgents.BoardEvaluators;

namespace StrAItego.Game.Agents.MCTSAgents
{
    abstract class MCTSAgent : RandomAgent.RandomAgent
    {
        protected int rollouts, estimations, nextNewNode = 0;
        IBoardEvaluator boardEvaluator = null;
        IBoardEstimator boardEstimator = null;
        Node[] nodes;
        bool init = false;
        int maxDepthTotal = 0, maxBranchingFactorTotal = 0;
        int maxDepth = 0;
        int maxBranchingFactor = 0;

        Stopwatch stopwatch = new Stopwatch();

        protected Type nodeImplementation = typeof(Node);


        public MCTSAgent() : base() {
            name = "MCTS Agent";
        }

        public override Move? GetMove(Board board, GameLogger gameLogger) {
            if (gameLogger != null)
                stopwatch.Restart();

            if (!init) {
                for (int i = 0; i < nodes.Length; i++)
                    nodes[i] = (Node)Activator.CreateInstance(nodeImplementation, this);
                init = true;
            }
            List<Move> moves = board.GetValidMoves(Team.Red);
            if (moves.Count == 0)
                return null;

            maxDepth = 0;
            maxBranchingFactor = 0;

            (float value, int visits)[][] MCTSresults = new (float value, int visits)[moves.Count][];
            for (int i = 0; i < MCTSresults.Length; i++)
                MCTSresults[i] = new (float value, int visits)[estimations];

            Node[] topNodes = new Node[estimations];

            for (int i = 0; i < estimations; i++) {
                nextNewNode = i;
                Board estimatedBoard = boardEstimator.EstimateBoard(board, r);
                topNodes[i] = DoMCTS(estimatedBoard, rollouts / estimations);
                for(int j = 0; j < moves.Count; j++) {
                    MCTSresults[j][i] = topNodes[i].GetScoreOfMove(topNodes[i].GetMoves()[j]);
                }
            }

            
            float[] moveValues = new float[moves.Count];
            int[] moveVisits = new int[moves.Count];
            for(int i = 0; i < moves.Count; i++) { 
                for(int j = 0; j < estimations; j++) {
                    (float value, int visits) = MCTSresults[i][j];
                    moveValues[i] += visits == 0 ? 0 : value / visits;
                    moveVisits[i] += visits;
                }
            }
            int bestID = 0;
            for(int i = 1; i < moves.Count; i++) {
                if (moveValues[bestID]< moveValues[i])
                    bestID = i;
            }

            if(gameLogger != null) {
                stopwatch.Stop();
                gameLogger.LogMessage($"Max. depth: {maxDepth} (all: {maxDepthTotal}) | Max. branching factor: {maxBranchingFactor} (all: {maxBranchingFactorTotal}) - {stopwatch.Elapsed}");
                gameLogger.LogMessage("Estimation quality", true);
                foreach(Node topNode in topNodes) {
                    (int piecesRevealed, int piecesGuessed, int immovablePiecesRevealed, int immovablePiecesGuessed) = board.CompareSetupAccuracy(topNode.Board);
                    gameLogger.LogMessage($"Est. : Revealed: {piecesRevealed,+2}/40 | Correct: {piecesGuessed,+2}/{40 - piecesRevealed,+2} | Imm. Revealed: {immovablePiecesRevealed,+1}/7 | Imm. Correct: {immovablePiecesGuessed,+1}/{7 - immovablePiecesRevealed,+1}");
                }
                gameLogger.LogMessage("Move values", true);
                string header = "Value (All) | Visits (All) | ";
                for (int i = 1; i <= estimations; i++)
                    header += $"Value (E.{i}) | Visits (E.{i}) | ";
                header += "Move";
                gameLogger.LogMessage(header, true);
                moves.Sort((x, y) => moveValues[y.ID].CompareTo(moveValues[x.ID]));
                foreach(Move m in moves) {
                    string line = $"{$"{string.Format("{0:0.#######}", moveValues[m.ID] / estimations), -9}", +11} | {moveVisits[m.ID], +12} | ";
                    int i = 1;
                    for(int j = 0; j < estimations; j++) {
                        (float value, int visits) = MCTSresults[m.ID][j];
                        if (i < 10)
                            line += $"{$"{string.Format("{0:0.#######}", value / visits), -9}", +11} | {visits, +12} | ";
                        else if(i < 100)
                            line += $"{$"{string.Format("{0:0.#######}", value / visits), -9}", +12} | {visits, +13} | ";
                        else
                            line += $"{$"{string.Format("{0:0.#######}", value / visits), -9}", +13} | {visits, +14} | ";
                        i++;
                    }
                    line += "#move#";
                    gameLogger.LogMessage(line, false, m);
                }
            }

            return moves.Find(x => x.ID == bestID);
        }

        Node DoMCTS(Board board, int rollouts) {
            Node topNode = GetUnusedNode();
            topNode.MakeTopNode(board, Team.Red);

            int ro = 0;
            Node currentNode;
            int depth, branchingFactor;
            while (ro < rollouts) {
                currentNode = topNode;
                depth = 0;
                branchingFactor = currentNode.GetMoves().Count;
                while (currentNode.Evaluated) {
                    Move selectedMove = SelectMove(currentNode);
                    currentNode = currentNode.GetChild(selectedMove);
                    depth++;
                    branchingFactor = Math.Max(branchingFactor, currentNode.GetMoves().Count);
                }

                if (currentNode.Winner != Team.Neither) {
                    currentNode.AddValue(1 - (int)currentNode.Winner);
                    maxDepth = Math.Max(maxDepth, depth);
                }
                else {
                    //currentNode = currentNode.GetChild(currentNode.GetMoves()[r.Next(currentNode.GetMoves().Count)]);
                    float value = EvaluateBoard(currentNode);
                    currentNode.AddValue(value);
                    maxDepth = Math.Max(maxDepth, depth);
                    maxBranchingFactor = Math.Max(maxBranchingFactor, branchingFactor);
                }
                ro++;
            }

            maxDepthTotal = Math.Max(maxDepthTotal, maxDepth);
            maxBranchingFactorTotal = Math.Max(maxBranchingFactorTotal, maxBranchingFactor);

            return topNode;
        }

        public Node GetUnusedNode() {
            return nodes[nextNewNode++];
        }

        protected abstract Move SelectMove(Node n);

        protected float EvaluateBoard(Node n) {
            n.Evaluate();
            return boardEvaluator.EvaluateNode(n, r);
        }

        public override abstract IAgentParameters GetParameters();

        public override void SetParameters(IAgentParameters agentParameters) {
            base.SetParameters(agentParameters);
            MCTSAgentParameters parameters = (MCTSAgentParameters)agentParameters;
            rollouts = parameters.Rollouts;
            estimations = parameters.Estimations;
            boardEvaluator = parameters.GetBoardEvaluator;
            boardEstimator = parameters.GetBoardEstimator;
            name = parameters.ToString();
            nodes = new Node[(rollouts / estimations) + estimations];
        }

        public override void Dispose() {
            boardEvaluator = null;
            boardEstimator = null;
            nodes = null;
            base.Dispose();
        }
    }

    class Node
    {
        protected MCTSAgent agent;                                             // Agent that this node is used for

        Node parent;                                                           // Parent node
        Node[] children;                                                       // Child nodes
        Board currentBoard;                                                    // Current board state
        protected List<Move> nextMoves;                                        // List of valid moves
        Team team;                                                             // Whose turn it is
        bool evaluated = false;                                                // Whether this node has been evaluated yet or not

        float value = 0f;                                                      // Current value of node
        int visits = 0;                                                        // Number of times this node has been visited

        Team winner = Team.Neither;
        bool isLeaf = true;

        public Node(MCTSAgent agent) {
            this.agent = agent;
            children = new Node[Board.maxBranchingFactor];
            currentBoard = new Board();
        }

        public virtual void MakeTopNode(Board board, Team team) {
            board.CopyTo(currentBoard);
            this.team = team;
            if (nextMoves != null) {
                int prevChildrenCount = nextMoves.Count;
                int i = 0;
                while (i < prevChildrenCount)
                    children[i++] = null;
            }
            nextMoves = currentBoard.GetValidMoves(team);
            value = 0f;
            visits = 0;
            winner = Team.Neither;
            isLeaf = true;
            parent = null;
            evaluated = true;
        }

        public virtual void MakeChildNode(Node parent, Move m) {
            this.parent = parent;
            value = 0f;
            visits = 0;
            winner = Team.Neither;
            isLeaf = true;
            parent.currentBoard.CopyTo(currentBoard);
            currentBoard.MakeMove(m);
            team = 1 - parent.team;
            if (nextMoves != null) {
                int prevChildrenCount = nextMoves.Count;
                int i = 0;
                while (i < prevChildrenCount)
                    children[i++] = null;
            }
            nextMoves = currentBoard.GetValidMoves(team);
            if (Board.GetRank(m.Defender) == Rank.Flag || nextMoves.Count == 0)
                winner = parent.team;
            evaluated = false;
        }

        public List<Move> GetMoves() {
            return nextMoves;
        }

        public (float, int) GetScoreOfMove(Move m) {
            Node n = children[m.ID];
            return n == null ? (0f, 0) : (n.value, n.visits);
        }

        public float GetValueOfMove(Move m) {
            Node n = children[m.ID];
            return n == null ? 0f : n.value;
        }

        public int GetVisitsOfMove(Move m) {
            Node n = children[m.ID];
            return n == null ? 0 : n.visits;
        }

        public Node GetChild(Move m) {
            Node n = children[m.ID];
            if (n != null)
                return n;
            n = agent.GetUnusedNode();
            n.MakeChildNode(this, m);
            children[m.ID] = n;
            isLeaf = false;
            return n;
        }

        public Board Board {
            get { return currentBoard; }
        }

        public Team Winner {
            get { return winner; }
        }

        public Team TurnOfTeam {
            get { return team; }
        }

        public bool IsLeaf() {
            return isLeaf;
        }

        public int Visits {
            get { return visits; }
        }

        public bool Evaluated {
            get { return evaluated; }
        }

        public void Evaluate() {
            evaluated = true;
        }

        public void AddValue(float f) {
            visits++;
            value += f;
            parent?.AddValue(f);
        }

        public override string ToString() {
            return "Avg. score: " + (value / visits) + ", visits: " + visits + (winner == Team.Neither ? "" : ", winner = " + winner.ToString());
        }
    }
}
