using StrAItego.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StrAItego.Game.Agents.HumanAgent
{
    public class HumanAgent : IAgent
    {
        Square From { get; set; }
        Square To { get; set; }
        AutoResetEvent waitResetEvent;
        EventHandler<MoveMadeEventArgs> SetupMoveMade;
        Team team;
        UI.Board UIboard;
        bool setupDone;
        string name = "Human Agent";

        public Move? GetMove(Board board, GameLogger gameLogger) {
            if (team == Team.Blue)
                board.Invert();
            From = Square.None;
            To = Square.None;
            List<Move> validMoves = board.GetValidMoves(team);
            if (validMoves.Count == 0)
                return null;
            while (true) {
                waitResetEvent.WaitOne();
                if (validMoves.Any(x => x.Origin == From && x.Destination == To)) {
                    Move? m = validMoves.Find(x => x.Origin == From && x.Destination == To);
                    if (m != null) {
                        if (team == Team.Blue) {
                            board.Invert();
                            m = ((Move)m).GetInvertedMove();
                        }
                        return m;
                    }
                }
            }
        }

        void OnAttemptedMove(object sender, AttemptMoveEventArgs e) {
            From = e.From;
            To = e.To;
        }

        public Rank[] GetSetup(Board board) {
            Rank[] units = {
            Rank.Bomb, Rank.Bomb, Rank.Bomb, Rank.Bomb, Rank.Bomb, Rank.Bomb,
            Rank.Marshal,
            Rank.General,
            Rank.Colonel, Rank.Colonel,
            Rank.Major, Rank.Major, Rank.Major,
            Rank.Captain, Rank.Captain, Rank.Captain, Rank.Captain,
            Rank.Lieutenant, Rank.Lieutenant, Rank.Lieutenant, Rank.Lieutenant,
            Rank.Sergeant, Rank.Sergeant, Rank.Sergeant, Rank.Sergeant,
            Rank.Miner, Rank.Miner, Rank.Miner, Rank.Miner, Rank.Miner,
            Rank.Scout, Rank.Scout, Rank.Scout, Rank.Scout, Rank.Scout, Rank.Scout, Rank.Scout, Rank.Scout,
            Rank.Spy,
            Rank.Flag
            };

            board.TakeSetup(units, false);
            
            

            if (team == Team.Blue)
                board.Invert();
            UIboard.DrawBoard(board, team);
            Button confirmButton = new Button();
            confirmButton.Text = "Confirm setup";
            confirmButton.Size = new System.Drawing.Size(108, 108);
            confirmButton.Click += OnConfirm;
            confirmButton.Name = "ConfirmButton";

            UIboard.ParentBoard.Invoke((MethodInvoker)delegate {
                UIboard.ParentBoard.Controls.Add(confirmButton);
                confirmButton.Location = new System.Drawing.Point(266, 266);
                confirmButton.BringToFront();
            });
            
            SetupMoveMade += UIboard.OnMoveMade;
            setupDone = false;
            while (!setupDone) {
                From = Square.None;
                To = Square.None;
                waitResetEvent.WaitOne();
                if (setupDone)
                    break;
                if ((team == Team.Red && From <= Square.K4 && To <= Square.K4) || 
                    (team == Team.Blue && From >= Square.A7 && To >= Square.A7)) {
                    Move m = new Move(board.OnSquare(From), From, To, board.OnSquare(To), 0);
                    board.MakeMove(m, true);
                    SetupMoveMade.Invoke(this, new MoveMadeEventArgs(m));
                }
            }
            UIboard.ParentBoard.Invoke((MethodInvoker)delegate {
                UIboard.ParentBoard.Controls.Remove(confirmButton);
                confirmButton.Dispose();
            });
            SetupMoveMade -= UIboard.OnMoveMade;
            if (team == Team.Blue)
                board.Invert();
            for (Square i = Square.A1; i <= Square.K4; i++) {
                units[(int)i] = board.OnSquare(i).Rank;
            }
            return units;
        }

        void OnConfirm(object sender, EventArgs e) {
            setupDone = true;
            waitResetEvent.Set();
        }

        public override string ToString() {
            return name;
        }

        public IAgentParameters GetParameters() {
            return new HumanAgentParameters();
        }

        public void SetParameters(IAgentParameters agentParameters) {
            HumanAgentParameters parameters = agentParameters as HumanAgentParameters;
            team = parameters.AsTeam;
            parameters.Board.AttemptMove += OnAttemptedMove;
            waitResetEvent = parameters.Board.MoveMadeResetEvent;
            UIboard = parameters.Board;
            name = parameters.ToString();
        }

        public bool IsAI() {
            return false;
        }

        public void Dispose() {
            waitResetEvent = null;
            SetupMoveMade = null;
            UIboard = null;
        }
    }
}
