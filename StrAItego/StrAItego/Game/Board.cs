using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StrAItego.Game
{
    public partial class Board
    {
        Piece[] board = new Piece[92];
        Piece[][] pieces = new Piece[2][];
        int[][] remainingPieces = new int[2][];
        int[][] potentialPieces = new int[2][];
        bool[][] foundAll = new bool[2][];
        int[][] piecesOnBoard = new int[2][];

        public static int maxBranchingFactor = 154;
        // 244 = Terrible upper bound. 8 scouts x 18 moves + 25 pieces x 4 moves = 244 moves if all could move unimpeded.
        // 160 = Better but not 100% sure. Can be lowered by placing two flags on the board.
        // We instantiate using this number to avoid having to resize lists later on, which places a lot of pressure on the Garbage Collector.
        List<Move> moves = new List<Move>(maxBranchingFactor);

        // For the two-squares rule
        #region Two-Squares Rule
        Square lastMovedOriginRed = Square.None, 
               lastMovedOriginBlue = Square.None, 
               lastMovedDestinationRed = Square.None, 
               lastMovedDestinationBlue = Square.None;
        int tsrCounterRed = 0,
            tsrCounterBlue = 0;
        #endregion

        // Board object creation/copying
        #region Board Creation
        /// <summary>
        /// Instantiates a new Board object.
        /// </summary>
        public Board() {
            for(int i = 0; i <= 1; i++) {
                pieces[i] = new Piece[40];
                for (int j = 0; j < 40; j++)
                    pieces[i][j] = new Piece(Rank.None, Team.Neither, -1);  // Dummy pieces
                remainingPieces[i] = new int[] { 1, 1, 8, 5, 4, 4, 4, 3, 2, 1, 1, 6};
                potentialPieces[i] = new int[] { 40, 40, 40, 40, 40, 40, 40, 40, 40, 40, 40, 40};
                foundAll[i] = new bool[] { false, false, false, false, false, false, false, false, false, false, false, false };
                piecesOnBoard[i] = new int[] { 1, 1, 8, 5, 4, 4, 4, 3, 2, 1, 1, 6 };
            }
        }

        /// <summary>
        /// Creates a copy of a given Board.
        /// <br>Useful to avoid accidentally cheating by editing a game's main board.</br>
        /// </summary>
        public Board(Board board) {
            for (int i = 0; i <= 1; i++) {
                pieces[i] = new Piece[40];
                for(int j = 0; j < 40; j++) {
                    pieces[i][j] = board.pieces[i][j].Copy();
                }
                remainingPieces[i] = new int[] { 1, 1, 8, 5, 4, 4, 4, 3, 2, 1, 1, 6 };
                potentialPieces[i] = new int[] { 40, 40, 40, 40, 40, 40, 40, 40, 40, 40, 40, 40 };
                foundAll[i] = new bool[] { false, false, false, false, false, false, false, false, false, false, false, false };
                piecesOnBoard[i] = new int[] { 1, 1, 8, 5, 4, 4, 4, 3, 2, 1, 1, 6 };
            }

            // Place pieces onto the board.
            for (int i = 0; i < 92; i++) {
                Piece p = board.board[i];
                this.board[i] = p == null ? null :
                    pieces[p.Team == Team.Red ? 0 : 1][p.SetupOrigin];
            }
            board.remainingPieces[0].CopyTo(remainingPieces[0], 0);
            board.remainingPieces[1].CopyTo(remainingPieces[1], 0);
            board.potentialPieces[0].CopyTo(potentialPieces[0], 0);
            board.potentialPieces[1].CopyTo(potentialPieces[1], 0);
            board.foundAll[0].CopyTo(foundAll[0], 0);
            board.foundAll[1].CopyTo(foundAll[1], 0);
            board.piecesOnBoard[0].CopyTo(piecesOnBoard[0], 0);
            board.piecesOnBoard[1].CopyTo(piecesOnBoard[1], 0);
            
            lastMovedDestinationBlue = board.lastMovedDestinationBlue;
            lastMovedDestinationRed = board.lastMovedDestinationRed;
            lastMovedOriginBlue = board.lastMovedOriginBlue;
            lastMovedOriginRed = board.lastMovedOriginRed;
            tsrCounterBlue = board.tsrCounterBlue;
            tsrCounterRed = board.tsrCounterRed;
        }

        /// <summary>
        /// Copies this board to a given board.
        /// <br>Useful to avoid unnecessary GC pressure, and to avoid accidentally cheating by editing a game's main board.</br>
        /// </summary>
        public void CopyTo(Board board) {
            for(int i = 0; i < 40; i++) {
                pieces[0][i].CopyTo(board.pieces[0][i]);
                pieces[1][i].CopyTo(board.pieces[1][i]);
            }
            for (int i = 0; i < 92; i++) {
                Piece p = this.board[i];
                board.board[i] = p == null ? null :
                    board.pieces[p.Team == Team.Red ? 0 : 1][p.SetupOrigin];
            }

            remainingPieces[0].CopyTo(board.remainingPieces[0], 0);
            remainingPieces[1].CopyTo(board.remainingPieces[1], 0);
            potentialPieces[0].CopyTo(board.potentialPieces[0], 0);
            potentialPieces[1].CopyTo(board.potentialPieces[1], 0);
            foundAll[0].CopyTo(board.foundAll[0], 0);
            foundAll[1].CopyTo(board.foundAll[1], 0);
            piecesOnBoard[0].CopyTo(board.piecesOnBoard[0], 0);
            piecesOnBoard[1].CopyTo(board.piecesOnBoard[1], 0);

            board.lastMovedDestinationBlue = lastMovedDestinationBlue;
            board.lastMovedDestinationRed = lastMovedDestinationRed;
            board.lastMovedOriginBlue = lastMovedOriginBlue;
            board.lastMovedOriginRed = lastMovedOriginRed;
            board.tsrCounterBlue = tsrCounterBlue;
            board.tsrCounterRed = tsrCounterRed;
        }
        #endregion

        // Methods for making board estimations or entering information in them
        #region Board estimations
        /// <summary>
        /// Create an estimation of a board by replacing the ranks of the opponent's units with replacedUnits.
        /// <br>Copies all info from baseBoard and applies the replacement to the board this method is called on.</br>
        /// </summary>
        public Board(Board baseBoard, Rank[] replacedUnits) {
            for (int i = 0; i <= 1; i++) {
                pieces[i] = new Piece[40];
                for (int j = 0; j < 40; j++) {
                    pieces[i][j] = baseBoard.pieces[i][j].Copy();
                }
                remainingPieces[i] = new int[] { 1, 1, 8, 5, 4, 4, 4, 3, 2, 1, 1, 6 };
                potentialPieces[i] = new int[] { 40, 40, 40, 40, 40, 40, 40, 40, 40, 40, 40, 40 };
                foundAll[i] = new bool[] { false, false, false, false, false, false, false, false, false, false, false, false };
                piecesOnBoard[i] = new int[] { 1, 1, 8, 5, 4, 4, 4, 3, 2, 1, 1, 6 };
            }
            // Place pieces onto the board.
            for (int i = 0; i < 92; i++) {
                Piece p = baseBoard.board[i];
                board[i] = p == null ? null :
                    pieces[p.Team == Team.Red ? 0 : 1][p.SetupOrigin];
            }

            baseBoard.remainingPieces[0].CopyTo(remainingPieces[0], 0);
            remainingPieces[1] = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            baseBoard.potentialPieces[0].CopyTo(potentialPieces[0], 0);
            potentialPieces[1] = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            baseBoard.foundAll[0].CopyTo(foundAll[0], 0);
            foundAll[1] = new bool[] { true, true, true, true, true, true, true, true, true, true, true, true };
            baseBoard.piecesOnBoard[0].CopyTo(piecesOnBoard[0], 0);
            piecesOnBoard[1] = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            foreach(Rank u in replacedUnits) {
                piecesOnBoard[1][(int)u - 1]++;
            }

            int replaced = 0;
            for(int i = 0; i < board.Length; i++) {
                if(board[i]?.Team == Team.Blue) {
                    board[i].Rank = replacedUnits[replaced++];
                    board[i].PotentialRank = board[i].Rank.ToPotentialRank();
                }
            }
        }
        
        /// <summary>
        /// Directly replaces the ranks on a board with replacedUnits, to make an estimation.
        /// </summary>
        public void EnterEstimation(Rank[] replacedUnits) {
            for(int i = 0; i < 40; i++) {
                pieces[1][i].Rank = replacedUnits[i];
            }
        }
        #endregion

        // Methods related to making a move
        #region Making moves
        /// <summary>
        /// Applies a move to the board and recalculates the PotentialRank information for the whole board. Use isSetupMove to swap pieces instead, used in the HumanAgent setup.
        /// <br>Note: Usually expects a move for Red in normal gameplay, but does support moves from Blue.</br>
        /// </summary>
        public void MakeMove(Move move, bool isSetupMove = false) {
            if (isSetupMove) {
                board[(int)move.Origin] = move.Defender;    // Allow unit swapping during setup phase.
                board[(int)move.Destination] = move.Attacker;
            }
            else {
                board[(int)move.Origin] = null;                // Whatever happens, moving unit is removed.
                //boardInfo[(int)move.Origin] = PotentialRank.None;

                if (move.Defender == null) {                                   // If we move into an empty square, just move the unit.
                    if (move.Attacker.Rank == Rank.Scout &&                             // Discover scout if it moves more than one space.
                        !move.Origin.AdjacentSquares().Contains(move.Destination))
                        move.Attacker.PotentialRank = UpdateInfo(move.InfoOfAttacker, PotentialRank.Scout, false);
                    else                                                                // Not a scout, just move. Attacker can't be a bomb or flag.
                        move.Attacker.PotentialRank = UpdateInfo(move.InfoOfAttacker, PotentialRank.NotBombOrFlag, false);
                    board[(int)move.Destination] = move.Attacker;
                }
                else {
                    Outcome o = move.Attacker.Attacks(move.Defender); // Check for outcome.
                                                                        // Both get revealed, calculate info
                    move.Attacker.PotentialRank = UpdateInfo(move.InfoOfAttacker, move.Attacker.Rank.ToPotentialRank(), false);
                    move.Defender.PotentialRank = UpdateInfo(move.InfoOfDefender, move.Defender.Rank.ToPotentialRank(), true);

                    int asTeam = (int)move.Attacker.Team;

                    if (o == Outcome.Defeat) {
                        // Discover the unit in case we lost.
                        piecesOnBoard[asTeam][(int)move.Attacker.Rank - 1]--;
                    }
                    else if (o == Outcome.Victory) {
                        board[(int)move.Destination] = move.Attacker; // Attacking unit is discovered.
                        piecesOnBoard[1 - asTeam][(int)move.Defender.Rank - 1]--;
                    }
                    else if (o == Outcome.Tie) {
                        board[(int)move.Destination] = null;   // Tie, both units removed from board.
                        piecesOnBoard[asTeam][(int)move.Attacker.Rank - 1]--;
                        piecesOnBoard[1 - asTeam][(int)move.Defender.Rank - 1]--;
                    }
                }

                // Check for Two-Squares Rule
                if (move.Attacker.Team == Team.Red) {
                    // If move was made by red
                    tsrCounterRed = move.Origin == lastMovedDestinationRed && move.Destination == lastMovedOriginRed ? tsrCounterRed + 1 : 1;
                    lastMovedOriginRed = move.Origin;
                    lastMovedDestinationRed = move.Destination;
                }
                else {
                    // If move was made by blue
                    tsrCounterBlue = move.Origin == lastMovedDestinationBlue && move.Destination == lastMovedOriginBlue ? tsrCounterBlue + 1 : 1;
                    lastMovedOriginBlue = move.Origin;
                    lastMovedDestinationBlue = move.Destination;
                }
            }

            CheckForReveals();
        }

        static (PotentialRank, Rank)[] unitRanks = new (PotentialRank, Rank)[]{
            (PotentialRank.Bomb,        Rank.Bomb),
            (PotentialRank.Captain,     Rank.Captain),
            (PotentialRank.Colonel,     Rank.Colonel),
            (PotentialRank.Flag,        Rank.Flag),
            (PotentialRank.General,     Rank.General),
            (PotentialRank.Lieutenant,  Rank.Lieutenant),
            (PotentialRank.Major,       Rank.Major),
            (PotentialRank.Marshal,     Rank.Marshal),
            (PotentialRank.Miner,       Rank.Miner),
            (PotentialRank.Scout,       Rank.Scout),
            (PotentialRank.Sergeant,    Rank.Sergeant),
            (PotentialRank.Spy,         Rank.Spy)
        };

        /// <summary>
        /// Updates PotentialRank information
        /// </summary>
        private PotentialRank UpdateInfo(PotentialRank oldInfo, PotentialRank update, bool fromOpponent) {
            PotentialRank newInfo = oldInfo & update;


            // Check for changes
            PotentialRank changedInfo = (PotentialRank)(oldInfo - newInfo);

            if (changedInfo == PotentialRank.None)  // Nothing changed
                return newInfo;

            // Check for discovery
            if (newInfo.IsDiscovered()) {
                Rank foundRank = newInfo.ToRank();
                remainingPieces[fromOpponent ? 1 : 0][(int)foundRank - 1]--;
                potentialPieces[fromOpponent ? 1 : 0][(int)foundRank - 1]--;
            }

            foreach((PotentialRank, Rank) pr in unitRanks) {
                if((pr.Item1 & changedInfo) != 0) {
                    potentialPieces[fromOpponent ? 1 : 0][(int)pr.Item2 - 1]--;
                }
            }           

            return newInfo;
        }

        // Note: method used below is slightly inefficient, but correct. A faster refactor is possible, by grouping certain ranks.
        /// <summary>
        /// Check if any pieces have been revealed.
        /// </summary>
        void CheckForReveals() {
            for (int team = 0; team <= 1; team++) {
                if (((potentialPieces[team][(int)Rank.Bomb - 1]) ==
                    (remainingPieces[team][(int)Rank.Flag - 1] + remainingPieces[team][(int)Rank.Bomb - 1]) ||
                    remainingPieces[team][(int)Rank.Bomb - 1] == 0)
                    && !foundAll[team][(int)Rank.Flag - 1]) {
                    for (int sq = 0; sq < 92; sq++) {
                        Piece p = board[sq];
                        if (p != null && (int)p.Team == team) {
                            if (p.Rank == Rank.Flag || p.Rank == Rank.Bomb)
                                p.PotentialRank = UpdateInfo(p.PotentialRank, PotentialRank.BombOrFlag, team == 1);
                            else
                                p.PotentialRank = UpdateInfo(p.PotentialRank, PotentialRank.NotBombOrFlag, team == 1);
                        }
                    }
                    foundAll[team][(int)Rank.Flag - 1] = true;
                    CheckForReveals();
                    return;
                }
                for (int rank = (int)Rank.Flag + 1; rank <= (int)Rank.Bomb; rank++) {
                    // Check anything other than flag/bomb
                    if((potentialPieces[team][rank - 1] == remainingPieces[team][rank - 1] || remainingPieces[team][rank - 1] == 0) && !foundAll[team][rank - 1]) { // We can reveal all pieces of rank rank of team team
                        PotentialRank pr = ((Rank)rank).ToPotentialRank();
                        for(int sq = 0; sq < 92; sq++) {
                            Piece p = board[sq];
                            if (p != null && (int)p.Team == team) {
                                if (p.Rank == (Rank)rank)
                                    p.PotentialRank = UpdateInfo(p.PotentialRank, pr, team == 1);
                                else
                                    p.PotentialRank = UpdateInfo(p.PotentialRank, (PotentialRank)(PotentialRank.Any - pr), team == 1);
                            }
                        }
                        foundAll[team][rank - 1] = true;
                        CheckForReveals();
                        return;
                    }
                }
            }
        }
        #endregion

        // Methods relating to obtaining a list of moves
        #region Get list of moves
        /// <summary>
        /// Gets a list of all valid moves that can be done by team team.
        /// <br>Note: always returns the same list object, but cleared and re-filled. That way we avoid unnecessary GC pressure from recreating the list.</br>
        /// </summary>
        public List<Move> GetValidMoves(Team team) {
            moves.Clear();
            if (team == Team.Neither) return moves;
            if (team == Team.Both) {
                moves.AddRange(GetValidMoves(Team.Red));
                moves.AddRange(GetValidMoves(Team.Blue));
                return moves;
            }

            for (Square i = Square.A1; i <= Square.K10; i++) {
                Piece u = this[i];
                if (u == null ||
                    u == Rank.Flag ||
                    u == Rank.Bomb) continue;   //No moves to be made for empty tile, bombs or flags

                // Scouts
                if (u == Rank.Scout && u == team) {
                    AddScoutMoves(i, team, moves);
                    continue;
                }

                // Generic units
                if(u == team)
                    AddGenericMoves(i, team, moves);
            }

            return moves;
        }

        /// <summary>
        /// Adds moves for a generic piece.
        /// </summary>
        void AddGenericMoves(Square i, Team t, List<Move> moves) {
            Square toExplore;
            Piece u;
            for (Direction d = Direction.North; d <= Direction.West; d++) {
                toExplore = i.AdjacentSquare(d);

                if (toExplore == Square.None) continue; // We are out of bounds

                u = this[toExplore];
                if (u == t) { // Blocked by friendly unit
                    continue;
                }
                else {
                    if((t == Team.Red && (tsrCounterRed < 3 || i != lastMovedDestinationRed || toExplore != lastMovedOriginRed)) ||
                        t == Team.Blue && (tsrCounterBlue < 3 || i != lastMovedDestinationBlue || toExplore != lastMovedOriginBlue))
                        moves.Add(new Move(this[i], i, toExplore, u, moves.Count));
                }
            }
        }

        /// <summary>
        /// Adds moves for a scout
        /// </summary>
        void AddScoutMoves(Square i, Team t, List<Move> moves) {
            bool blocked;
            Square toExplore;
            Piece u;
            for(Direction d = Direction.North; d <= Direction.West; d++) {
                blocked = false;
                toExplore = i;

                while (!blocked) {
                    toExplore = toExplore.AdjacentSquare(d);

                    if (toExplore == Square.None) break; // We are out of bounds

                    u = this[toExplore];
                    if (u == t) { // Blocked by friendly unit
                        blocked = true;
                    }
                    else {
                        if ((t == Team.Red && (tsrCounterRed < 3 || i != lastMovedDestinationRed || toExplore != lastMovedOriginRed)) ||
                             t == Team.Blue && (tsrCounterBlue < 3 || i != lastMovedDestinationBlue || toExplore != lastMovedOriginBlue))
                            moves.Add(new Move(this[i], i, toExplore, u, moves.Count));
                        if (u != Team.Neither) blocked = true;  // Can't move further due to enemy unit
                    }
                }
            }
        }
        #endregion

        /// <summary>
        /// Index accessor that returns the piece that is standing on square s.
        /// <br>Be careful when setting this, as you may accidentally cheat if you are using the game's primary board!</br>
        /// </summary>
        public Piece this[Square s]
        {
            get { return s == Square.None ? null : board[(int)s]; }
            set { board[(int)s] = value; }
        }

        /// <summary>
        /// Count the number of possible ranks a piece on square sq could still have.
        /// </summary>
        public int CountPossibilitiesOnSquare(Square sq)
        {
            uint v = (uint)this[sq].PotentialRank;
            v -= ((v >> 1) & 0x55555555); // reuse input as temporary
            v = (v & 0x33333333) + ((v >> 2) & 0x33333333); // temp
            uint c = ((v + (v >> 4) & 0xF0F0F0F) * 0x1010101) >> 24; // count
            return (int)c;
        }

        /// <summary>
        /// Get all pieces for team t.
        /// </summary>
        public Piece[] GetPieces(Team t)
        {
            return t == Team.Red ? pieces[0] : pieces[1];
        }

        /// <summary>
        /// Inverts a board, flipping it on both axes and inverting the colours. Used after making a move, so an agent always plays as red.
        /// </summary>
        public void Invert() {
            for (int i = (int)Square.A1; i <= (int)Square.K5; i++) {
                Piece u = board[i];
                board[i] = board[91 - i];
                board[91 - i] = u;
            }

            foreach (Piece p in pieces[0])
                p.Invert();
            foreach (Piece p in pieces[1])
                p.Invert();

            Piece[] ptemp = pieces[0];
            pieces[0] = pieces[1];
            pieces[1] = ptemp;
            int[] temp = potentialPieces[0];
            potentialPieces[0] = potentialPieces[1];
            potentialPieces[1] = temp;
            temp = remainingPieces[0];
            remainingPieces[0] = remainingPieces[1];
            remainingPieces[1] = temp;
            bool[] ftemp = foundAll[0];
            foundAll[0] = foundAll[1];
            foundAll[1] = ftemp;
            temp = piecesOnBoard[0];
            piecesOnBoard[0] = piecesOnBoard[1];
            piecesOnBoard[1] = temp;

            //TSR
            int t = tsrCounterBlue;
            tsrCounterBlue = tsrCounterRed;
            tsrCounterRed = t;

            Square ts = lastMovedDestinationBlue;
            lastMovedDestinationBlue = 91 - lastMovedDestinationRed;
            lastMovedDestinationRed = 91 - ts;
            ts = lastMovedOriginBlue;
            lastMovedOriginBlue = 91 - lastMovedOriginRed;
            lastMovedOriginRed = 91 - ts;
        }

        /// <summary>
        /// Applies a given setup to a board.
        /// </summary>
        public void TakeSetup(Rank[] setup, bool resetPotentialRanks = true) {
            for (int i = 0; i < 40; i++) {
                board[i] = new Piece(setup[i], Team.Red, i);
                pieces[0][i] = board[i];
            }
            if (resetPotentialRanks)
                for (int i = 0; i < 40; i++)
                    pieces[0][i].ResetPotentialRank();
        }

        /// <summary>
        /// Counts the number of remaining pieces of rank r and team t on the board.
        /// </summary>
        public int UnitCount(Rank r, Team t) {
            return piecesOnBoard[(int)t][(int)r - 1];
        }

        /// <summary>
        /// Gets the square in a specified row and column.
        /// </summary>
        public static Square GetSquare(int row, int column)
        {
            if (row < 0 || row > 9 || column < 0 || column > 9)
                return Square.None;
            return _squareTable[row * 10 + column];
        }

        // Methods relating to mapping the board to some binary representation
        #region Binary mappings
        /// <summary>
        /// Converts the PotentialRank information of all 40 pieces of a team to a binary representation of 40*12=480 0/1s.
        /// </summary>
        public float[] PiecesToBinary(Team t, float[] bin)
        {
            Piece[] ps = pieces[t == Team.Red ? 0 : 1];
            for (int i = 0; i < 480; i++)
                bin[i] = 0f;
            for (int i = 0; i < 40; i++)
            {
                Piece p = ps[i];
                // Mark potential ranks
                for (Rank r = Rank.Flag; r <= Rank.Bomb; r++)
                {
                    PotentialRank pr = r.ToPotentialRank();
                    if ((p.PotentialRank & pr) > 0)
                    {
                        bin[i * 12 + ((int)r - 1)] = 1f;
                    }
                }
            }

            return bin;
        }

        /// <summary>
        /// Converts the Rank, PotentialRank and enemy PotentialRank information on all 92 squares to a binary representation.
        /// <br>This gives 92*12*3=3312 total 0/1s.</br>
        /// </summary>
        public float[] ToBinary(Team asTeam) {
            return ToBinary(new float[3312], asTeam);
        }

        /// <summary>
        /// Converts the Rank, PotentialRank and enemy PotentialRank information on all 92 squares to a binary representation.
        /// <br>This gives 92*12*3=3312 total 0/1s.</br>
        /// </summary>
        public float[] ToBinary(float[] bin, Team asTeam) {
            // Format: 92 floats per rank, 92 floats per potential rank, then 92 floats per enemy potential rank
            // Total: 3312 floats
            for (int i = 0; i < bin.Length; i++)
                bin[i] = 0f;
            if (asTeam == Team.Red) {
                for (Square i = Square.A1; i <= Square.K10; i++) {
                    Piece p = this[i];
                    if (p == null)
                        continue;
                    // First, mark actual rank
                    int index = (int)i;                     // Add position
                    index += 92 * ((int)p.Rank - 1);        // Add rank
                    if (p.Team == asTeam)
                        bin[index] = 1f;

                    // Then, mark potential ranks
                    for (Rank r = Rank.Flag; r <= Rank.Bomb; r++) {
                        PotentialRank pr = r.ToPotentialRank();
                        if ((p.PotentialRank & pr) > 0) {
                            bin[1104 * (p.Team == asTeam ? 1 : 2) + (int)i + 92 * ((int)r - 1)] = 1f;
                        }
                    }
                }
            }
            else {
                for (Square i = Square.K10; i >= Square.A1; i--) {
                    Piece p = this[i];
                    if (p == null)
                        continue;
                    // First, mark actual rank
                    int index = (int)i;                     // Add position
                    index += 92 * ((int)p.Rank - 1);        // Add rank
                    if (p.Team == asTeam)
                        bin[index] = 1f;

                    // Then, mark potential ranks
                    for (Rank r = Rank.Flag; r <= Rank.Bomb; r++) {
                        PotentialRank pr = r.ToPotentialRank();
                        if ((p.PotentialRank & pr) > 0) {
                            bin[1104 * (p.Team == asTeam ? 1 : 2) + (int)i + 92 * ((int)r - 1)] = 1f;
                        }
                    }
                }
            }

            return bin;
        }

        /// <summary>
        /// Converts the ranks of the setup of a team to a binary representation of 40*12=480 0/1s.
        /// </summary>
        public float[] SetupToBinary(Team t, float[] binarysetup = null, int startingIndex = 0) {
            Piece[] pcs = t == Team.Red ? pieces[0] : pieces[1];
            if(binarysetup == null)
                binarysetup = new float[480];
            else {
                for (int i = 0; i < 480; i++)
                    binarysetup[startingIndex + i] = 0f;
            }

            for(int i = 0; i < 40; i++) {
                binarysetup[startingIndex + i * 12 + ((int)pcs[i].Rank - 1)] = 1;
            }

            return binarysetup;
        }
        #endregion

        // Methods useful for diagnostic or debug purposes.
        #region Diagnostic methods
        /// <summary>
        /// Pretty-prints a board to a string representation.
        /// </summary>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 9; i >= 0; i--)
            {
                for (int j = 0; j < 10; j++)
                {
                    sb.Append(this[GetSquare(i, j)].UnitToString());
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }

        /// <summary>
        /// Compares the accuracy of a given setup to the actual setup.
        /// <br>Returns revealed, correctly guessed, immovable revealed, corectly guessed immovable</br>
        /// </summary>
        public (int, int, int, int) CompareSetupAccuracy(Board compBoard) {
            Piece[] corr = pieces[1];
            Piece[] guess = compBoard.pieces[1];

            // Return revealed, correctly guessed, immovable revealed, correctly guessed immovable
            int revealed = corr.Count(x => x.PotentialRank.IsDiscovered());
            int immovablerevealed = corr.Count(x => x.PotentialRank.IsDiscovered() && (x.Rank == Rank.Bomb || x.Rank == Rank.Flag));

            int correctTotal = 0;
            int correctlyguessedimmovable = 0;
            for(int i = 0; i < 40; i++) {
                if (corr[i].Rank == guess[i].Rank) {
                    correctTotal++;
                    if (corr[i].Rank == Rank.Bomb || corr[i].Rank == Rank.Flag)
                        correctlyguessedimmovable++;
                }

            }

            return (revealed, correctTotal - revealed, immovablerevealed, correctlyguessedimmovable - immovablerevealed);
        }
        #endregion
    }
}
