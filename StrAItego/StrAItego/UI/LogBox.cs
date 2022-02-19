using System.Drawing;
using System.Windows.Forms;
using StrAItego.Game;

namespace StrAItego.UI
{
    class LogBox : ListBox
    {
        GameLogger gl;
        public LogBox() : base() {
            DrawMode = DrawMode.OwnerDrawFixed;
            DrawItem += OnDrawItem;
            Dock = DockStyle.Fill;
            ScrollAlwaysVisible = true;
            HorizontalScrollbar = true;
        }

        public void LinkLogger(GameLogger gameLogger) {
            Items.AddRange(gameLogger.GetEntries());
            gameLogger.LogEntryMade += OnLogEntry;
            gl = gameLogger;
        }

        void OnLogEntry(object sender, LogEntryEventArgs e) {
            if (!IsHandleCreated) return;
            try {
                Invoke((MethodInvoker)delegate {
                    Items.Add(e.Entry);
                });
            }
            catch { }
        }

        public void Disconnect() {
            gl.LogEntryMade -= OnLogEntry;
        }

        void OnDrawItem(object sender, DrawItemEventArgs e) {
            try {
                if (Items.Count > 0) {
                    LogEntry logEntry = (LogEntry)Items[e.Index];
                    Graphics g = e.Graphics;
                    SolidBrush b = new SolidBrush(logEntry.EntryColor);
                    g.FillRectangle(b, e.Bounds);

                    SolidBrush t = new SolidBrush(e.ForeColor);
                    g.DrawString(logEntry.ToString(), new Font(FontFamily.GenericMonospace, e.Font.SizeInPoints, logEntry.Bold ? FontStyle.Bold : FontStyle.Regular), t, GetItemRectangle(e.Index).Location);
                }
                e.DrawFocusRectangle();
            }
            catch { }
        }

        public LogEntry[] GetEntries() {
            return gl.GetEntries();
        }
    }
}
