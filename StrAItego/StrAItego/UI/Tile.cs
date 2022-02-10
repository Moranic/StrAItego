using StrAItego.Game;
using StrAItego.Properties;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace StrAItego.UI
{
    class Tile : TransparentPanel {
        object objlock = new object();
        PictureBox[] PictureBoxes = new PictureBox[26];
        int visible = 0;
        public Square Square { get; set; }
        PotentialRank latestInfo = PotentialRank.None;

        ToolTip tooltip;

        public Tile(Point location, Square square) {
            for (int i = 0; i < PictureBoxes.Length; i++)
                PictureBoxes[i] = new PictureBox();
            foreach (PictureBox pb in PictureBoxes) {
                pb.Visible = false;
                pb.Size = new System.Drawing.Size(64, 64);
                pb.SizeMode = PictureBoxSizeMode.StretchImage;
                pb.Margin = new Padding(0, 0, 0, 0);
                pb.Padding = new Padding(0, 0, 0, 0);
            }
            PictureBoxes[0].Image = Resources.redunknown;
            PictureBoxes[1].Image = Resources.redflag;
            PictureBoxes[2].Image = Resources.redspy;
            PictureBoxes[3].Image = Resources.redscout;
            PictureBoxes[4].Image = Resources.redminer;
            PictureBoxes[5].Image = Resources.redsergeant;
            PictureBoxes[6].Image = Resources.redlieutenant;
            PictureBoxes[7].Image = Resources.redcaptain;
            PictureBoxes[8].Image = Resources.redmajor;
            PictureBoxes[9].Image = Resources.redcolonel;
            PictureBoxes[10].Image = Resources.redgeneral;
            PictureBoxes[11].Image = Resources.redmarshal;
            PictureBoxes[12].Image = Resources.redbomb;
            PictureBoxes[13].Image = Resources.blueunknown;
            PictureBoxes[14].Image = Resources.blueflag;
            PictureBoxes[15].Image = Resources.bluespy;
            PictureBoxes[16].Image = Resources.bluescout;
            PictureBoxes[17].Image = Resources.blueminer;
            PictureBoxes[18].Image = Resources.bluesergeant;
            PictureBoxes[19].Image = Resources.bluelieutenant;
            PictureBoxes[20].Image = Resources.bluecaptain;
            PictureBoxes[21].Image = Resources.bluemajor;
            PictureBoxes[22].Image = Resources.bluecolonel;
            PictureBoxes[23].Image = Resources.bluegeneral;
            PictureBoxes[24].Image = Resources.bluemarshal;
            PictureBoxes[25].Image = Resources.bluebomb;
            foreach (PictureBox pb in PictureBoxes) {
                Controls.Add(pb);
                pb.MouseDown += Tile_MouseDown;
                pb.MouseUp += Tile_MouseUp;
                pb.MouseMove += Tile_MouseMove;
                pb.MouseEnter += ShowTooltip;
                pb.MouseLeave += HideTooltip;
            }
            Location = location;
            Size = new Size(64, 64);
            MinimumSize = Size;
            MaximumSize = Size;
            Margin = new Padding(0, 0, 0, 0);
            Padding = new Padding(0, 0, 0, 0);
            FixLocation();
            Square = square;
            tooltip = new ToolTip();
            tooltip.SetToolTip(this, latestInfo.ToString());
            MouseEnter += ShowTooltip;
            MouseLeave += HideTooltip;
        }

        void ShowTooltip(object sender, EventArgs e) {
            Point p = PointToScreen(new Point(0, 64));
            if(latestInfo != PotentialRank.None)
                tooltip.Show(latestInfo.ToString(), this, PointToClient(p));
        }

        void HideTooltip(object sender, EventArgs e) {
            if(tooltip.Active)
                tooltip.Hide(this);
        }

        public void MakeVisible(Unit unit, PotentialRank info, Team asTeam) {
            latestInfo = info;
            Team ofTeam = unit == Unit.None ? Team.Neither : unit > Unit.RedBomb ? Team.Blue : Team.Red;
            bool reveal = asTeam == Team.Both || ofTeam == asTeam || Game.Board.UnitKnown(info) || unit == Unit.None;
            int newIndex = reveal ? unit == Unit.None ? -1 : 
                                (int)unit + (int)ofTeam : 
                                ofTeam == Team.Red ? 
                                    0 : 
                                    13;
            if (newIndex == visible) //No need to update visual, already set to the right index.
                return;
            
            this.Invoke((MethodInvoker)delegate { 
                if(visible != -1)
                    PictureBoxes[visible].Visible = false;
                if (newIndex != -1)
                    PictureBoxes[newIndex].Visible = true;
                Parent.Invalidate(new Rectangle(Location, Size));
            });
            visible = newIndex;
        }

        static int[] _unitToIndex = {
            -1,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            13,
            13,
            13,
            13,
            13,
            13,
            13,
            13,
            13,
            13,
            13,
            13,
            1,
            2,
            3,
            4,
            5,
            6,
            7,
            8,
            9,
            10,
            11,
            12,
            14,
            15,
            16,
            17,
            18,
            19,
            20,
            21,
            22,
            23,
            24,
            25};

        Point originalLocation;
        bool isHeld = false;
        Point mouseOffset;

        public void FixLocation() {
            originalLocation = Location;
        }

        public void Tile_MouseDown(object sender, MouseEventArgs e) {
            isHeld = true;
            mouseOffset = Form.MousePosition;
        }

        public void Tile_MouseMove(object sender, MouseEventArgs e) {
            if (!isHeld)
                return;
            Point mousePosition = Form.MousePosition;
            mousePosition.Offset(originalLocation.X - mouseOffset.X, originalLocation.Y - mouseOffset.Y);
            mousePosition.X = mousePosition.X < 0 ? 0 : mousePosition.X > 576 ? 576 : mousePosition.X;
            mousePosition.Y = mousePosition.Y < 0 ? 0 : mousePosition.Y > 576 ? 576 : mousePosition.Y;
            Location = mousePosition;

            //Invalidate(new Rectangle(Location, Size));
        }

        public event EventHandler<TileDroppedEventArgs> TileDropped;

        public void Tile_MouseUp(object sender, EventArgs e) {
            isHeld = false;
            Point sendLocation = Location;
            sendLocation.X += 32;
            sendLocation.Y += 32;
            Location = originalLocation;
            //Invalidate(new Rectangle(Location, Size));
            OnTileDropped(new TileDroppedEventArgs(sendLocation, Square));
        }

        public virtual void OnTileDropped(TileDroppedEventArgs e) {
            TileDropped?.Invoke(this, e);
        }

    }

    public class TileDroppedEventArgs : EventArgs
    {
        public TileDroppedEventArgs(Point location, Square tile) {
            Location = location;
            DraggedTile = tile;
        }
        public Point Location { get; set; }
        public Square DraggedTile { get; set; }
    }

    public class TransparentPanel : Panel
    {
        protected override CreateParams CreateParams {
            get {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x00000020; // WS_EX_TRANSPARENT
                return cp;
            }
        }
        protected override void OnPaintBackground(PaintEventArgs e) {
            //base.OnPaintBackground(e);
        }

    }
}
