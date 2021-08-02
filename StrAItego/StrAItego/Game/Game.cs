using StrAItego.Game.Agents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StrAItego.Game
{
    public class Game
    {
        Board board;
        IAgent[] agents = new IAgent[2];
        Team turn;
        Team winner = Team.Neither;
        bool cancelled = false;
        int movesMade = 0;

        public Game(IAgent redPlayer, IAgent bluePlayer) {
            board = new Board();
            agents[0] = redPlayer;
            agents[1] = bluePlayer; // Blue player's perspective is as if playing red.
            turn = Team.Red;
        }

        public Game(IAgent redPlayer, IAgent bluePlayer, Board b, Team team) {
            board = new Board(b);
            agents[0] = redPlayer;
            agents[1] = bluePlayer;
            turn = team;
        }

        public Board GetBoard() {
            return board;
        }

        public void CancelGame() {
            cancelled = true;
        }

        public Team PlayGame(System.Windows.Forms.NumericUpDown moveDelay = null, GameLogger gameLogger = null, bool skipSetup = false, bool disposeAgents = true) {
            if (!skipSetup) {
                // Setup
                Rank[] blueSetup = agents[1].GetSetup(board);
                board.TakeSetup(blueSetup);
                board.Invert();
                Rank[] redSetup = agents[0].GetSetup(board);
                board.TakeSetup(redSetup);
                SetupEnd?.Invoke(this, new EventArgs());
            }

            while (!cancelled) {
                IAgent agent = agents[(int)turn];
                Move? m = agent.GetMove(board, gameLogger);
                if (m is null) {
                    winner = 1 - turn;
                    DisposeAgents(disposeAgents);
                    return winner;
                }
                board.MakeMove((Move)m);
                movesMade++;
                OnMoveMade(turn == Team.Red ? (Move)m : ((Move)m).GetInvertedMove());
                if (((Move)m).Defender != null && (
                    ((Move)m).Defender.GetUnit() == Unit.BlueFlag ||
                   ((Move)m).Defender.GetUnit() == Unit.RedFlag)) {
                    winner = turn;
                    DisposeAgents(disposeAgents);
                    return winner;
                }
                if(movesMade >= 2000) {
                    winner = Team.Neither;
                    DisposeAgents(disposeAgents);
                    return winner;
                }
                turn = 1 - turn;
                board.Invert();
                if (moveDelay?.Value > 0)
                    Thread.Sleep((int)moveDelay.Value);
            }
            winner = Team.Neither;
            DisposeAgents(disposeAgents);
            return winner;
        }

        public EventHandler<MoveMadeEventArgs> MoveMade;
        public EventHandler<EventArgs> SetupEnd;


        public Team GetTurn {
            get { return turn; }
        }
        void OnMoveMade(Move m) {
            MoveMade?.Invoke(this, new MoveMadeEventArgs(m));
        }

        void DisposeAgents(bool disposeAgents) {
            if(disposeAgents)
                foreach (IAgent agent in agents)
                    agent.Dispose();
        }

        public IAgent RedPlayer {
            get { return agents[0]; }
        }

        public IAgent BluePlayer {
            get { return agents[1]; }
        }

        public Team GetWinner {
            get { return winner; }
        }

        public int MovesMade {
            get { return movesMade; }
        }

        public override string ToString() {
            return RedPlayer.ToString() + " vs. " + BluePlayer.ToString() + ", move " + movesMade + ", " + turn + "'s turn";
        }
    }

    public class MoveMadeEventArgs : EventArgs
    {
        public Move Move { get; set; }

        public MoveMadeEventArgs(Move m) {
            Move = m;
        }
    }
}
