using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using StrAItego.Game;

namespace StrAItego.UI
{
    public class Board {
        Tile[] tiles = new Tile[92];
        Panel parentBoard;
        Game.Game game = null;
        Team drawTeam;
        public AutoResetEvent MoveMadeResetEvent = new AutoResetEvent(false);
        bool setup = false;

        public Board(Panel board) {
            parentBoard = board;
            for (Square i = Square.A1; i <= Square.K4; i++) {
                tiles[(int)i] = new Tile(new Point((int)i % 10 * 64, 576 - 64 * ((int)i / 10)), i);
            }
            tiles[(int)Square.A5] = new Tile(new Point(0, 320), Square.A5);
            tiles[(int)Square.B5] = new Tile(new Point(64, 320), Square.B5);
            tiles[(int)Square.E5] = new Tile(new Point(256, 320), Square.E5);
            tiles[(int)Square.F5] = new Tile(new Point(320, 320), Square.F5);
            tiles[(int)Square.J5] = new Tile(new Point(512, 320), Square.J5);
            tiles[(int)Square.K5] = new Tile(new Point(576, 320), Square.K5);
            tiles[(int)Square.A6] = new Tile(new Point(0, 256), Square.A6);
            tiles[(int)Square.B6] = new Tile(new Point(64, 256), Square.B6);
            tiles[(int)Square.E6] = new Tile(new Point(256, 256), Square.E6);
            tiles[(int)Square.F6] = new Tile(new Point(320, 256), Square.F6);
            tiles[(int)Square.J6] = new Tile(new Point(512, 256), Square.J6);
            tiles[(int)Square.K6] = new Tile(new Point(576, 256), Square.K6);
            for (Square i = Square.A7; i <= Square.K10; i++) {
                tiles[(int)i] = new Tile(new Point(((int)i + 8) % 10 * 64, 576 - 64 * (((int)i + 8) / 10)), i);
            }
            foreach (Tile tile in tiles) {
                parentBoard.Controls.Add(tile);
                tile.TileDropped += OnAttemptMove;
            }
        }

        public void DrawBoard(Game.Board board, Team asTeam) {
            for (Square i = Square.A1; i <= Square.K10; i++) {
                Square index = game.GetTurn == Team.Red ? i : 91 - i;
                Unit u = board.OnSquare(index);
                PotentialRank info = board.InfoOnSquare(index);
                if (game.GetTurn == Team.Blue && u > Unit.None)
                    u = u > Unit.RedBomb ? u - 12 : u + 12;
                tiles[(int)i].MakeVisible(u, info, asTeam);
            }
        }

        public void DisconnectGame() {
            MoveMadeResetEvent = new AutoResetEvent(false);
            if (game != null) {
                game.MoveMade -= OnMoveMade;
                game.SetupEnd -= OnSetupEnd;
                foreach (Tile t in tiles)
                    t.MakeVisible(Unit.None, PotentialRank.None, Team.Neither);
            }
            Control[] buttons = parentBoard.Controls.Find("ConfirmButton", true);
            foreach (Control c in buttons)
                parentBoard.Controls.Remove(c);
        }

        public void ConnectGame(Game.Game g, Team asTeam) {
            game = g;
            game.MoveMade += OnMoveMade;
            game.SetupEnd += OnSetupEnd;
            drawTeam = asTeam;
            DrawBoard(game.GetBoard(), drawTeam);
            setup = true;
        }

        public void ChangeDrawTeam(Team asTeam) {
            drawTeam = asTeam;
            if(!setup && game != null)
                DrawBoard(game.GetBoard(), drawTeam);
        }

        public void OnSetupEnd(object sender, EventArgs e) {
            setup = false;
            DrawBoard(game.GetBoard(), drawTeam);
        }

        public void OnMoveMade(object sender, MoveMadeEventArgs e) {
            if (setup) {
                Square origin = e.Move.Origin;
                Square destination = e.Move.Destination;
                Unit attackedUnit = e.Move.Defender;
                Unit movedUnit = e.Move.Attacker;
                tiles[(int)origin].MakeVisible(attackedUnit, e.Move.InfoOfDefender, Team.Both);
                tiles[(int)destination].MakeVisible(movedUnit, e.Move.InfoOfAttacker, Team.Both);
                return;
            }
            else {
                //Square origin = e.Move.Origin;
                //tiles[(int)origin].MakeVisible(Unit.None, Team.Both);
                //
                //Unit attackedUnit = e.Move.Defender;
                //Square destination = e.Move.Destination;
                //Unit movedUnit = e.Move.Attacker;
                //
                //if (attackedUnit == Unit.None) {
                //    if (Game.Board.GetRank(movedUnit) == Rank.Scout && movedUnit < Unit.DiscoveredRedFlag &&   // Discover scout if it moves more than one space.
                //        !Game.Board.GetAdjacentSquares(origin).Contains(destination))
                //        tiles[(int)destination].MakeVisible(movedUnit + 24, drawTeam);
                //    else
                //        tiles[(int)destination].MakeVisible(movedUnit, drawTeam);
                //    return;
                //}
                //
                //Outcome o = Game.Board.DoAttack(movedUnit, attackedUnit); // Check for outcome.
                //if (o == Outcome.Defeat) {
                //    tiles[(int)destination].MakeVisible(Game.Board.DiscoverUnit(attackedUnit), drawTeam); // Discover the unit in case we lost.
                //    return;
                //}
                //if (o == Outcome.Victory)
                //    tiles[(int)destination].MakeVisible(Game.Board.DiscoverUnit(movedUnit), drawTeam); // Attacking unit is discovered.
                //if (o == Outcome.Tie)
                //    tiles[(int)destination].MakeVisible(Unit.None, drawTeam);   // Tie, both units removed from board.
                DrawBoard(game.GetBoard(), drawTeam);
            }
        }

        Tile GetTileAtPoint(Point pt) {
            try {
                return (Tile)parentBoard.GetChildAtPoint(pt);
            }
            catch { }
            return null;
        }

        public event EventHandler<AttemptMoveEventArgs> AttemptMove;

        void OnAttemptMove(object sender, TileDroppedEventArgs e) {
            Square from = e.DraggedTile;
            Square? to = GetTileAtPoint(e.Location)?.Square;
            if (to == null || from == to) return;
            AttemptMove?.Invoke(this, new AttemptMoveEventArgs(from, (Square)to));
            MoveMadeResetEvent.Set();
        }

        public Panel ParentBoard {
            get { return parentBoard; }
        }
    }

    public class AttemptMoveEventArgs : EventArgs
    {
        public Square From { get; set; }
        public Square To { get; set; }

        public AttemptMoveEventArgs(Square from, Square to) {
            From = from;
            To = to;
        }
    }
}
