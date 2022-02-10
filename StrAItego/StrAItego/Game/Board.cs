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
        //Unit[] board = new Unit[92];
        //PotentialRank[] boardInfo = new PotentialRank[92];
        int[][] remainingPieces = new int[2][];
        int[][] potentialPieces = new int[2][];
        bool[][] foundAll = new bool[2][];
        int[][] piecesOnBoard = new int[2][];

        public static int maxBranchingFactor = 154;
        // 244 = Terrible upper bound. 8 scouts x 18 moves + 25 pieces x 4 moves = 244 moves if all could move unimpeded.
        // 160 = Better but not 100% sure. Can be lowered by placing two flags on the board.

        List<Move> moves = new List<Move>(maxBranchingFactor);

        // For the two-squares rule
        Square lastMovedOriginRed = Square.None, 
               lastMovedOriginBlue = Square.None, 
               lastMovedDestinationRed = Square.None, 
               lastMovedDestinationBlue = Square.None;
        int tsrCounterRed = 0,
            tsrCounterBlue = 0;

        //Create new board
        public Board() {
            //for (int i = 0; i < boardInfo.Length; i++)
            //    if (i < 40 || i > 51)
            //        boardInfo[i] = PotentialRank.Any;
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

        //Create copy of board
        public Board(Board board) {
            
            //board.boardInfo.CopyTo(boardInfo, 0);
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

        //Create estimation of board
        public Board(Board baseBoard, Rank[] replacedUnits) {
            //baseBoard.boardInfo.CopyTo(boardInfo, 0);
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
                    board[i].PotentialRank = RankToPotentialRank(board[i].Rank);
                }
            }
        }

        public void EnterEstimation(Rank[] replacedUnits) {
            //remainingPieces[1] = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            //potentialPieces[1] = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            //foundAll[1] = new bool[] { true, true, true, true, true, true, true, true, true, true, true, true };
            //piecesOnBoard[1] = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            //foreach (Rank u in replacedUnits) {
            //    piecesOnBoard[1][(int)u - 1]++;
            //}

            for(int i = 0; i < 40; i++) {
                pieces[1][i].Rank = replacedUnits[i];
            }

            //int replaced = 0;
            //for (int i = 0; i < board.Length; i++) {
            //    if (board[i]?.Team == Team.Blue) {
            //        board[i].Rank = replacedUnits[replaced++];
            //        board[i].PotentialRank = RankToPotentialRank(board[i].Rank);
            //    }
            //}
        }

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
                        !GetAdjacentSquares(move.Origin).Contains(move.Destination))
                        move.Attacker.PotentialRank = UpdateInfo(move.InfoOfAttacker, PotentialRank.Scout, false);
                    else                                                                // Not a scout, just move. Attacker can't be a bomb or flag.
                        move.Attacker.PotentialRank = UpdateInfo(move.InfoOfAttacker, PotentialRank.NotBombOrFlag, false);
                    board[(int)move.Destination] = move.Attacker;
                }
                else {
                    Outcome o = DoAttack(move.Attacker, move.Defender); // Check for outcome.
                                                                        // Both get revealed, calculate info
                    move.Attacker.PotentialRank = UpdateInfo(move.InfoOfAttacker, RankToPotentialRank(move.Attacker.Rank), false);
                    move.Defender.PotentialRank = UpdateInfo(move.InfoOfDefender, RankToPotentialRank(move.Defender.Rank), true);

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

        public PotentialRank UpdateInfo(PotentialRank oldInfo, PotentialRank update, bool fromOpponent) {
            PotentialRank newInfo = oldInfo & update;


            // Check for changes
            PotentialRank changedInfo = (PotentialRank)(oldInfo - newInfo);

            if (changedInfo == PotentialRank.None)  // Nothing changed
                return newInfo;

            // Check for discovery
            if (UnitKnown(newInfo)) {
                Rank foundRank = PotentialRankToRank(newInfo);
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
                    //foundAll[team][(int)Rank.Bomb - 1] = true;
                    foundAll[team][(int)Rank.Flag - 1] = true;
                    CheckForReveals();
                    return;
                }
                for (int rank = (int)Rank.Flag + 1; rank <= (int)Rank.Bomb; rank++) {
                    // Check anything other than flag/bomb
                    if((potentialPieces[team][rank - 1] == remainingPieces[team][rank - 1] || remainingPieces[team][rank - 1] == 0) && !foundAll[team][rank - 1]) { // We can reveal all pieces of rank rank of team team
                        PotentialRank pr = RankToPotentialRank((Rank)rank);
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

        public static bool UnitKnown(PotentialRank info) {
            bool res = info > 0 && (info & (info - 1)) == 0;
            return res;
        }

        public int CountPossibilitiesOnSquare(Square sq) {
            uint v = (uint)InfoOnSquare(sq);
            v -= ((v >> 1) & 0x55555555); // reuse input as temporary
            v = (v & 0x33333333) + ((v >> 2) & 0x33333333); // temp
            uint c = ((v + (v >> 4) & 0xF0F0F0F) * 0x1010101) >> 24; // count
            return (int)c;
        }

        public Piece[] GetPieces(Team t) {
            return t == Team.Red ? pieces[0] : pieces[1];
        }

        public List<Move> GetValidMoves(Team team) {
            moves.Clear();
            if (team == Team.Neither) return moves;
            if (team == Team.Both) {
                moves.AddRange(GetValidMoves(Team.Red));
                moves.AddRange(GetValidMoves(Team.Blue));
                return moves;
            }

            for (Square i = Square.A1; i <= Square.K10; i++) {
                Piece u = OnSquare(i);
                Rank r = GetRank(u);
                if (u == null ||
                    r == Rank.Flag ||
                    r == Rank.Bomb) continue;   //No moves to be made for empty tile, bombs or flags

                // Scouts
                if ((GetRank(u) == Rank.Scout) && u.Team == team) {
                    AddScoutMoves(i, team, moves);
                    continue;
                }

                // Generic units
                if(u.Team == team)
                    AddGenericMoves(i, team, moves);
            }

            // We need to filter out any moves prohibited by the Two-Squares Rule.
            //if (team == Team.Red && tsrCounterRed == 3) {
            //    moves = moves.Where(x => !(x.Origin == lastMovedDestinationRed && x.Destination == lastMovedOriginRed)).ToList();
            //}
            //else if (team == Team.Blue && tsrCounterBlue == 3) {
            //    moves = moves.Where(x => !(x.Origin == lastMovedDestinationBlue && x.Destination == lastMovedOriginBlue)).ToList();
            //}

            return moves;
        }

        void AddGenericMoves(Square i, Team t, List<Move> moves) {
            Square toExplore;
            Piece u;
            for (Direction d = Direction.North; d <= Direction.West; d++) {
                toExplore = GetAdjacentSquare(i, d);

                if (toExplore == Square.None) continue; // We are out of bounds

                u = OnSquare(toExplore);
                if (OfTeam(u) == t) { // Blocked by friendly unit
                    continue;
                }
                else {
                    if((t == Team.Red && (tsrCounterRed < 3 || i != lastMovedDestinationRed || toExplore != lastMovedOriginRed)) ||
                        t == Team.Blue && (tsrCounterBlue < 3 || i != lastMovedDestinationBlue || toExplore != lastMovedOriginBlue))
                        moves.Add(new Move(OnSquare(i), i, toExplore, u, moves.Count));
                }
            }
        }

        void AddScoutMoves(Square i, Team t, List<Move> moves) {
            bool blocked;
            Square toExplore;
            Piece u;
            for(Direction d = Direction.North; d <= Direction.West; d++) {
                blocked = false;
                toExplore = i;

                while (!blocked) {
                    toExplore = GetAdjacentSquare(toExplore, d);

                    if (toExplore == Square.None) break; // We are out of bounds

                    u = OnSquare(toExplore);
                    Team ot = OfTeam(u);
                    if (ot == t) { // Blocked by friendly unit
                        blocked = true;
                    }
                    else {
                        if ((t == Team.Red && (tsrCounterRed < 3 || i != lastMovedDestinationRed || toExplore != lastMovedOriginRed)) ||
                             t == Team.Blue && (tsrCounterBlue < 3 || i != lastMovedDestinationBlue || toExplore != lastMovedOriginBlue))
                            moves.Add(new Move(OnSquare(i), i, toExplore, u, moves.Count));
                        if (ot != Team.Neither) blocked = true;  // Can't move further due to enemy unit
                    }
                }
            }
        }

        public Piece OnSquare(Square s) {
            if (s == Square.None) return null;
            return board[(int)s];
        }

        public PotentialRank InfoOnSquare(Square s) {
            if (s == Square.None) return PotentialRank.None;
            return board[(int)s]?.PotentialRank ?? PotentialRank.None;
        }

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

        public void TakeSetup(Rank[] setup, bool resetPotentialRanks = true) {
            for (int i = 0; i < 40; i++) {
                board[i] = new Piece(setup[i], Team.Red, i);
                pieces[0][i] = board[i];
            }
            if (resetPotentialRanks)
                for (int i = 0; i < 40; i++)
                    pieces[0][i].ResetPotentialRank();
        }

        public float[] PiecesToBinary(Team t, float[] bin) {
            Piece[] ps = pieces[t == Team.Red ? 0 : 1];
            for (int i = 0; i < 480; i++)
                bin[i] = 0f;
            for(int i = 0; i < 40; i++) {
                Piece p = ps[i];
                // Mark potential ranks
                for (Rank r = Rank.Flag; r <= Rank.Bomb; r++) {
                    PotentialRank pr = RankToPotentialRank(r);
                    if ((p.PotentialRank & pr) > 0) {
                        bin[i * 12 + ((int)r - 1)] = 1f;
                    }
                }
            }

            return bin;
        }

        public int UnitCount(Rank r, Team t) {
            return piecesOnBoard[(int)t][(int)r - 1];
        }

        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            for (int i = 9; i >= 0; i--) {
                for (int j = 0; j < 10; j++) {
                    sb.Append(UnitToString(OnSquare(GetSquare(i, j))));
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }

        public int RemainingOnBoard(Team t, Rank r) {
            return piecesOnBoard[(int)t][(int)r - 1];
        }

        public float[] ToBinary(Team asTeam) {
            return ToBinary(new float[3312], asTeam);
        }

        public float[] ToBinary(float[] bin, Team asTeam) {
            // Format: 92 floats per rank, 92 floats per potential rank, then 92 floats per enemy potential rank
            // Total: 3312 floats
            for (int i = 0; i < bin.Length; i++)
                bin[i] = 0f;
            if (asTeam == Team.Red) {
                for (Square i = Square.A1; i <= Square.K10; i++) {
                    Piece p = OnSquare(i);
                    if (p == null)
                        continue;
                    // First, mark actual rank
                    int index = (int)i;                     // Add position
                    index += 92 * ((int)p.Rank - 1);        // Add rank
                    if (p.Team == asTeam)
                        bin[index] = 1f;

                    // Then, mark potential ranks
                    for (Rank r = Rank.Flag; r <= Rank.Bomb; r++) {
                        PotentialRank pr = RankToPotentialRank(r);
                        if ((p.PotentialRank & pr) > 0) {
                            bin[1104 * (p.Team == asTeam ? 1 : 2) + (int)i + 92 * ((int)r - 1)] = 1f;
                        }
                    }
                }
            }
            else {
                for (Square i = Square.K10; i >= Square.A1; i--) {
                    Piece p = OnSquare(i);
                    if (p == null)
                        continue;
                    // First, mark actual rank
                    int index = (int)i;                     // Add position
                    index += 92 * ((int)p.Rank - 1);        // Add rank
                    if (p.Team == asTeam)
                        bin[index] = 1f;

                    // Then, mark potential ranks
                    for (Rank r = Rank.Flag; r <= Rank.Bomb; r++) {
                        PotentialRank pr = RankToPotentialRank(r);
                        if ((p.PotentialRank & pr) > 0) {
                            bin[1104 * (p.Team == asTeam ? 1 : 2) + (int)i + 92 * ((int)r - 1)] = 1f;
                        }
                    }
                }
            }

            return bin;
        }

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

        public (int, int, int, int) CompareSetupAccuracy(Board compBoard) {
            Piece[] corr = pieces[1];
            Piece[] guess = compBoard.pieces[1];

            // Return revealed, correctly guessed, immovable revealed, correctly guessed immovable
            int revealed = corr.Count(x => UnitKnown(x.PotentialRank));
            int immovablerevealed = corr.Count(x => UnitKnown(x.PotentialRank) && (x.Rank == Rank.Bomb || x.Rank == Rank.Flag));

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
    }
}
