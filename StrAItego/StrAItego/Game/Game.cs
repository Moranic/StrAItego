using System;
using System.Threading;
using StrAItego.Game.Agents;

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

        /// <summary>
        /// Create a new game.
        /// </summary>
        public Game(IAgent redPlayer, IAgent bluePlayer) {
            board = new Board();
            agents[0] = redPlayer;
            agents[1] = bluePlayer; // Blue player's perspective is as if playing red.
            turn = Team.Red;
        }

        /// <summary>
        /// Create a new game that continues from a specific state.
        /// </summary>
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

        /// <summary>
        /// Plays a game. Will request moves from the RedAgent and BlueAgent.
        /// </summary>
        /// <param name="moveDelay">Delay added between moves, to prevent the game from going so fast the UI can't keep up.</param>
        /// <param name="gameLogger">GameLogger object to log debug information from the game.</param>
        /// <param name="skipSetup">Skip the setup phase.</param>
        /// <param name="disposeAgents">Dispose of the agents after the game. Set to false to reuse the agents.</param>
        /// <returns>Winner of the game.</returns>
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
                    ((Move)m).Defender == Unit.BlueFlag ||
                   ((Move)m).Defender == Unit.RedFlag)) {
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

        /// <summary>
        /// Event that fires if an agent makes a move.
        /// </summary>
        public EventHandler<MoveMadeEventArgs> MoveMade;
        /// <summary>
        /// Event that fires if the setup ends.
        /// </summary>
        public EventHandler<EventArgs> SetupEnd;

        /// <summary>
        /// Get which Team's turn it is.
        /// </summary>
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

        /// <summary>
        /// The number of moves made in the game.
        /// </summary>
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
