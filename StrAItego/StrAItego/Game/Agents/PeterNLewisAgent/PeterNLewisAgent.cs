using System;
using System.Collections.Generic;
using System.Linq;

namespace StrAItego.Game.Agents.PeterNLewisAgent
{
    /*
    Author: Peter N Lewis (sourced from https://github.com/braathwaate/strategoevaluator/blob/master/agents/peternlewis/peternlewis.cpp)
    
    Originally: Submitted in 1997 to MacTech Programming Challenge and won.
    <http://www.mactech.com/articles/mactech/Vol.13/13.11/Nov97Challenge/index.html>
    Assumptions:
        Only time we spend thinking is counted against out 10 seconds (not time in GetMove/ReportMove)
        
    Method:
        Basically we keep track of the board and what we know and what they know.
        Each opponent piece has a bit map associated with it describing what pieces it could be.
        As we see more pieces, the bit map is culled.  If the piece moves, the bomb & flag bits are removed.
        If we've seen all Scouts (for example), then the Scout bit is removed from all remaining pieces.
        If all but one bit is remvoed, then we know what the piece is.
        
        At each turn, we simply apply a sequence of actions (listed below) and take the first action that works.
        
        It does very little in the way of lookahead (it plans out a path, but doesn't remember it and
        doesn't take it to account any movement by the opposition)
        
        It keeps a CRC of recent board positions (since the last strike) and doesn't replay any boards
        (we want to win, not draw!).
        
        If we exceed 10 seconds thinking time, we resign.  Not that this is particularly likely,
        in the games I tried, it spend less than half a second total.
    Optimizations:
        None.
    
    Comment:
        It actually plays a half decent game!  The end game is not as good as I'd like, but time is up!
*/

    /*
    USE SPY
        If our spy is next to their 1, kill it

    DEFEND AGAINST SPY
        if we have seen the spy, ignore this case

        If an unknown piece is next to the 1, then
            run, attack, have another piece attack, or ignore depending on a table
    ATTACK WEAKER
        If a known piece is next to a weaker known piece, attack it
            except if it places that piece in a dangerous location
    EXPLORE ATTACK
        If a 5,6,7,9 is next to an unknown piece, attack it
    RETREAT
        If a known piece is next to a stronger known piece, run away
            (preferably towards something that can kill it
            or if it's lowly, towards an unknown piece)

    SCOUT
        Try advancing scouts rapidly
    ATTACK DISTANT
        If a known piece is distant, but a clear path leads a slightly better piece towards it, advance the better piece
            (includes miners)
    EXPLORE DISTANT
        Try exploring (advance lowly pieces towards unknown pieces)
    ATTACK KNOWN WITH SAME DISTANT
        If a known piece can be attacked by a known identical piece, attack it

    FIND FLAG
        When few unmoved pieces remain, start assuming they are bombs/flags
    MOVE FORWARD
        Move any piece we can forward
    MOVE
        Move any piece we can
    RESIGN
        Give up
    */

    class PeterNLewisAgent : RandomAgent.RandomAgent
    {
        Queue<uint> CRC = new Queue<uint>();
        string moveReason = "";

        public PeterNLewisAgent() : base("Peter N. Lewis Agent") { }

        public override Move? GetMove(Board board, GameLogger gameLogger) {
            Move? m = FigureOutMove(board);
            if (m != null)
                AppendCRC(board, (Move)m);
            if (m != null && ((Move)m).Attacker == null)
                throw new Exception("Unexpected failure to find valid move!\r\n" + moveReason + " resulted in illegal move");
            gameLogger?.LogMessage($"{moveReason}", false, m);
            return m;
        }

        public Move? FigureOutMove(Board board) {
            //Obtain all valid moves.
            List<Move> moves = board.GetValidMoves(Team.Red);


            //USE SPY
            var spyMove = moves.Where(x => x.Attacker.Rank == Rank.Spy &&
                                           x.InfoOfDefender == PotentialRank.Marshal);
            if (spyMove.Count() > 0) {
                moveReason = "USE SPY";
                return spyMove.First();
            }

            //DEFEND AGAINST SPY
            if(board.UnitCount(Rank.Spy, Team.Blue) > 0) {     // Check if the enemy spy is still on the board. If not, skip this step.
                List<Move> marshalMoves = moves.Where(x => x.Attacker.Rank == Rank.Marshal).ToList();
                
                int unknowns = marshalMoves.Count(x => !x.InfoOfDefender.IsDiscovered() && x.Defender != null);
                if(unknowns > 0) {
                    int base_index = 0;
                    Square hasMarshal = marshalMoves.First().Origin;
                    
                    Square runSquare = FindSafeSquare(board, hasMarshal);
                    if(runSquare == Square.None)
                        base_index += 16;
                    if (unknowns > 1)
                        base_index += 8;
                    
                    foreach(Move m in marshalMoves) {
                        if (!m.InfoOfDefender.IsDiscovered() && m.Defender != null) {
                            int index = base_index;

                            if (moves.Exists(x => x.Attacker.Rank >= Rank.Scout && x.Attacker.Rank <= Rank.Captain && x.Attacker.Rank != Rank.Miner && x.Destination == m.Destination))
                                index += 4;

                            if (m.Destination.AdjacentSquares().Aggregate(0, 
                                (x, y) => x + (board[y] == Team.Blue && !board[y].IsDiscovered ? 1 : 0)) > 0)
                                index += 2;							

                            if ((m.InfoOfDefender & PotentialRank.NotBombOrFlag) == m.InfoOfDefender)
                                index += 1;

                            char lookup = defend_spy_table[index];
                            if(lookup == 'A') {	// Attack using marshal
                                moveReason = "DEFEND AGAINST SPY - Attack using marshal";
                                return m;
                            }
                            if(lookup == 'O') { // Attack using lowly unit
                                Move attackUsingLowlyUnit = moves.Find(x => x.Attacker.Rank >= Rank.Scout && x.Attacker.Rank <= Rank.Captain && x.Attacker.Rank != Rank.Miner && m.Destination == x.Destination);
                                if (attackUsingLowlyUnit.Attacker != null) {
                                    moveReason = "DEFEND AGAINST SPY - Attack using lowly unit";
                                    return attackUsingLowlyUnit;
                                }
                            }
                        }
                    }

                    if (runSquare != Square.None) {
                        Move spyDefendMove = marshalMoves.Find(x => x.Destination == runSquare);
                        if (spyDefendMove.Attacker != null && OKMove(board, spyDefendMove)) {
                            moveReason = "DEFEND AGAINST SPY - Run away";
                            return spyDefendMove;
                        }
                    }
                }
            }

            //ATTACK WEAKER
            for(Square i = Square.A1; i < Square.None; i++) {
                if(board[i] == Team.Blue) {
                    PotentialRank enemy = board[i];

                    Move? bestDir = null;
                    bool isBestRevealed = true;
                    Rank bestRank = Rank.Bomb;
                    Square[] adjacent = i.AdjacentSquares();

                    List<Move> directAttackingMoves = moves.Where(x => x.Destination == i && adjacent.Contains(x.Origin)).ToList();
                    foreach(Move m in directAttackingMoves) {
                        Rank attackerRank = m.Attacker;
                        if (!enemy.CouldKill(attackerRank)) {
                            if (attackerRank.WillKill(enemy)) {
                                bool thisRevealed = m.InfoOfAttacker.IsDiscovered();
                                if (isBestRevealed || !thisRevealed) {
                                    if (bestDir is null || attackerRank < bestRank) {
                                        bestDir = m;
                                        bestRank = attackerRank;
                                        isBestRevealed = thisRevealed;
                                    }
                                }
                            }
                        }
                    }
                    if (bestDir != null) {
                        moveReason = "ATTACK WEAKER";
                        return bestDir;
                    }
                }
            }

            //EXPLORE ATTACK
            foreach(Rank x in LowlyRanks) {
                for (Square i = Square.K10; i >= Square.A1; i--) {
                    Piece u = board[i];
                    if(x == u && u == Team.Red) {
                        List<Move> exploreMoves = moves.Where(x => x.Origin == i && x.Defender != null && !x.InfoOfDefender.IsDiscovered()).ToList();
                        Square[] adjacent = i.AdjacentSquares();
                        foreach (Square s in adjacent)
                            if (exploreMoves.Exists(x => x.Destination == s)) {
                                Move exploreMove = exploreMoves.Find(x => x.Destination == s);
                                if (exploreMove.Attacker != null) {
                                    moveReason = "EXPLORE ATTACK";
                                    return exploreMove;
                                }
                            }
                    }
                }
            }

            //RETREAT
            for (Square i = Square.K10; i >= Square.A1; i--) {
                Piece u = board[i];
                Rank r = u;
                PotentialRank pr = board[i];
                if(u == Team.Red && (pr & PotentialRank.BombOrFlag) == 0) {
                    List<Move> retreatMoves = moves.Where(x => x.Origin == i).ToList();
                    Move? retreatMove = null;
                    foreach (Move m in retreatMoves) {
                        if(r.WillKill(m.InfoOfDefender)) {
                            int bestPath = 1000;
                            for(Square to = Square.K10; to >= Square.A1; to--) {
                                int thisPath = bestPath;
                                if(board[to] == Team.Blue 
                                    && (board[to].IsDiscovered || r.WillKill(board[to]))) {
                                    Move? newMove = FindSafePath(board, retreatMoves, false, true, i, to, ref bestPath);
                                    if (newMove != null && OKMove(board, (Move)newMove))
                                        retreatMove = newMove;
                                    bestPath = thisPath;
                                }
                            }
                            if (bestPath < 1000 && retreatMove != null) {
                                moveReason = "RETREAT";
                                return retreatMove;
                            }
                        }
                    }
                }
            }


            //SCOUT
            List<Move> scoutMoves = moves.Where(x => x.Attacker.Rank == Rank.Scout && x.Defender == null).ToList();
            if (scoutMoves.Count > 0) {
                (Move?, int) bestScoutMove = scoutMoves.Aggregate<Move, (Move?, int)>(
                                                                 (null, -1),
                                                                 (x, y) => {
                                                                     int unknowns = CountUnknownEnemiesNoKnowns(board, y.Origin);
                                                                     return x.Item2 < unknowns && OKMove(board, y) ? (y, unknowns)
                                                                                               : x;
                                                                 });
                if (bestScoutMove.Item2 > 0) {
                    moveReason = "SCOUT";
                    return bestScoutMove.Item1;
                }
            }

            //ATTACK DISTANT
            int bestpath = 1000;
            Move? attackDistantMove = null;
            for(Square i = Square.A1; i <= Square.K10; i++) {
                if (board[i] == Team.Blue) {
                    PotentialRank possibilities = board[i];
                    PotentialRank danger = i.AdjacentSquares().Aggregate(PotentialRank.None,
                                                                                (x, y) => board[y] == Team.Blue ? x | board[y]
                                                                                                                                      : x);
                    if ((possibilities & (PotentialRank.Bomb | PotentialRank.Marshal)) != (PotentialRank.Bomb | PotentialRank.Marshal)) {
                        for(Square j = Square.A1; j <= Square.K10; j++) {
                            Piece u = board[j];
                            Team t = u;
                            if(t == Team.Red) {
                                Rank r = u;
                                if(r.WillKill(possibilities)) {
                                    if (r >= Rank.Captain || !danger.CouldKill(r)) {
                                        int thisPath = bestpath;
                                        Move? newMove = FindSafePath(board, moves, true, false, j, i, ref thisPath);
                                        if (newMove != null && OKMove(board, (Move)newMove)) {
                                            bestpath = thisPath;
                                            attackDistantMove = newMove;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (bestpath < 1000 & attackDistantMove != null && ((Move)attackDistantMove).Attacker != null) {
                moveReason = "ATTACK DISTANT";
                return attackDistantMove;
            }

            //EXPLORE DISTANT
            bestpath = 1000;
            Move? exploreDistantMove = null;
            for (Square i = Square.A1; i <= Square.K10; i++) {
                if (board[i] == Team.Blue && !board[i].IsDiscovered) {
                    for (Square j = Square.A1; j <= Square.K10; j++) {
                        Piece u = board[j];
                        if(u == Team.Red && LowlyRanks.Contains(u)) {
                            int thisPath = bestpath;
                            Move? newMove = FindSafePath(board, moves, false, true, j, i, ref thisPath);
                            if (newMove != null && OKMove(board, (Move)newMove)) {
                                bestpath = thisPath;
                                exploreDistantMove = newMove;
                            }
                        }
                    }
                }
            }
            if (bestpath < 1000 && exploreDistantMove != null && ((Move)exploreDistantMove).Attacker != null) {
                moveReason = "EXPLORE DISTANT";
                return exploreDistantMove;
            }

            //ATTACK KNOWN WITH SAME DISTANT
            bestpath = 1000;
            Move? attackKnownDistantMove = null;
            for (Square i = Square.A1; i <= Square.K10; i++) {
                if (board[i] == Team.Blue) {
                    PotentialRank possibilities = board[i];
                    
                    if ((possibilities & (PotentialRank.Bomb | PotentialRank.Marshal)) != (PotentialRank.Bomb | PotentialRank.Marshal)) {
                        for (Square j = Square.A1; j <= Square.K10; j++) {
                            Piece u = board[j];
                            if (u == Team.Red) {
                                Rank r = u;
                                if (r.WillKillOrSuicide(possibilities)) {
                                    int thisPath = bestpath;
                                    Move? newMove = FindSafePath(board, moves, true, true, j, i, ref thisPath);
                                    if (newMove != null && OKMove(board, (Move)newMove)) {
                                        bestpath = thisPath;
                                        attackKnownDistantMove = newMove;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (bestpath < 1000 & attackKnownDistantMove != null && ((Move)attackKnownDistantMove).Attacker != null) {
                moveReason = "ATTACK KNOWN WITH SAME DISTANT";
                return attackKnownDistantMove;
            }


            //FIND FLAG
            // This was never implemented by the original author.

            //MOVE FORWARD
            List<Move> forwardMoves = moves.Where(x => x.Origin.AdjacentSquare(Direction.North) == x.Destination).ToList();
            if(forwardMoves.Count > 0) {
                Move forwardMove = forwardMoves.Aggregate((x, y) => y.Origin > x.Origin && OKMove(board, y) ? y : x);
                moveReason = "MOVE FORWARD";
                return forwardMove;
            }

            //MOVE
            if(moves.Count > 0) {
                Move move = moves.Aggregate((x, y) => y.Origin > x.Origin & OKMove(board, y) ? y 
                                                                                             : y.Origin == x.Origin && y.Destination > x.Destination ? y 
                                                                                                                                                     : x);
                moveReason = "MOVE";
                return move;
            }
            //RESIGN
            moveReason = "RESIGN";
            return null;
        }

        static int CountUnknownEnemiesNoKnowns(Board board, Square i) {
            Square[] adjacent = i.AdjacentSquares();
            int unknowns = 0;
            foreach(Square j in adjacent) {
                if(board[i] == Team.Blue && board[i] != null) {
                    if (board[j].IsDiscovered)
                        return 0;
                    else
                        unknowns++;
                }
            }
            return unknowns;
        }

        static Square FindSafeSquare(Board board, Square sq) {
            Rank rank = board[sq];
            Square[] adjacent = sq.AdjacentSquares();
            for(int d = 0; d < 4; d++) {
                Square consider = adjacent[(2 + d) % 4];	// Try moving backwards first
                
                while(consider != Square.None && board[consider] == null) {
                    Square[] adjacent2 = consider.AdjacentSquares();
                    PotentialRank adjacentdanger = adjacent2.Aggregate(PotentialRank.None, (x, y) => x & board[y]);
                    if(!adjacentdanger.CouldKill(rank)) {
                        return consider;
                    }
                    if (rank != Rank.Scout)
                        break;
                    consider = adjacent2[2 + d % 4];
                }
            }
            return Square.None;
        }

        void AppendCRC(Board board, Move m) {
            uint crc = CalcBoardCRC(board, m);
            if (CRC.Count >= 1000)
                CRC.Dequeue();
            CRC.Enqueue(crc);
        }

        bool OKMove(Board board, Move m) {
            uint crc = CalcBoardCRC(board, m);

            if (m.Defender != null)
                return true;

            if (CRC.Contains(crc))
                return false;
            
            return true;
        }

        static uint CalcBoardCRC(Board board, Move m) {
            uint result = 0;
            int rankish;
            for(Square i = Square.A1; i <= Square.K10; i++) {
                if(i == m.Origin) {
                    rankish = 0;
                }
                else if(i == m.Destination) {
                    rankish = (int)(m.Attacker.Rank);
                }
                else if(board[i] == null) {
                    rankish = 0;
                }
                else if(board[i] == Team.Red) {
                    rankish = (int)board[i].Rank;
                }
                else {
                    rankish = board[i].IsDiscovered ? (int)board[i].Rank : 16;
                }
                result += (uint)rankish;	// Author admits this is not a brilliant CRC.
                result = result * 11 + (result >> 25);
            }

            return result;
        }

        static Move? FindSafePath(Board board, List<Move> moves, bool very_safe, bool suicide_ok, Square from, Square to, ref int best_path) {
            Piece u = board[from];
            Rank r = u;
            int from_row = from.Row();
            int from_col = from.Column();
            int to_row = to.Row();
            int to_col = to.Column();

            if (Math.Abs(from_row - to_row) + Math.Abs(from_col - to_col) > best_path)
                return null;

            if (Math.Abs(from_row - to_row) + Math.Abs(from_col - to_col) == 1)
                return moves.Find(x => x.Origin == from && x.Destination == to);

            int[] path_length_to = new int[92];
            for (int i = 0; i < 92; i++)
                path_length_to[i] = -1;
            Square[] queue = new Square[100];
            int queue_start = 0;
            int queue_fin = 0;
            int queue_next_len = 0;
            int current_len = 0;
            Square curr;

            queue[queue_fin] = from;
            path_length_to[(int)from] = 0;
            queue_fin++;
            queue_next_len = queue_fin;

            while(queue_fin > queue_start) {
                curr = queue[queue_start];
                queue_start++;

                for(Direction d = Direction.North; d <= Direction.West; d++) {	//Scout moves were not implemented.
                    Square rc = curr.AdjacentSquare(d);
                    if(rc != Square.None && path_length_to[(int)rc] == -1 && board[rc] != null) {
                        Square[] adjacent = rc.AdjacentSquares();
                        PotentialRank dangers = adjacent.Aggregate(PotentialRank.None, (x, y) => x &
                                            (board[y] == Team.Blue ? very_safe ? board[y] : board[y].PotentialRank.IsDiscovered() ? board[y] : PotentialRank.None : PotentialRank.None));
                        if(suicide_ok ? !dangers.CouldKillSafely(r)
                                      : !dangers.CouldKill(r)) {
                            path_length_to[(int)rc] = current_len + 1;
                            if (adjacent.Contains(to)) {
                                best_path = current_len + 1;
                                while (current_len > 0) {
                                    for (Direction bd = Direction.North; bd <= Direction.West; bd++) {
                                        Square backrc = rc.AdjacentSquare(bd);

                                        if (backrc != Square.None && path_length_to[(int)backrc] == current_len) {
                                            rc = backrc;
                                            break;
                                        }
                                    }
                                    current_len--;
                                }
                                return moves.Find(x => x.Origin == from && x.Destination == rc);
                            }
                            queue[queue_fin] = rc;
                            queue_fin++;
                        }
                        else {
                            path_length_to[(int)rc] = 1000;	//Can't go here
                        }
                    }
                }

                if(queue_start == queue_next_len) {
                    queue_next_len = queue_fin;
                    current_len++;
                }
            }
            return null;
        }

        string defend_spy_table = "RARROAOORARRRARRXAXAOAOOXAXAXAXA";
        Rank[] LowlyRanks = new Rank[] { Rank.Scout, Rank.Sergeant, Rank.Lieutenant, Rank.Captain };

        public override IAgentParameters GetParameters() {
            return new PeterNLewisAgentParameters();
        }

        public override void SetParameters(IAgentParameters agentParameters) {
            base.SetParameters(agentParameters);
            PeterNLewisAgentParameters parameters = (PeterNLewisAgentParameters)agentParameters;
            name = parameters.ToString();
        }

        public override void Dispose() {
            CRC = null;
            base.Dispose();
        }
    }
}

