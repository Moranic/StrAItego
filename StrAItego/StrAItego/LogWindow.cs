using StrAItego.Game;
using StrAItego.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StrAItego
{
    public partial class LogWindow : Form
    {
        LogBox lb;
        public LogWindow(GameLogger gl) {
            InitializeComponent();
            lb = new LogBox();
            splitContainer1.Panel1.Controls.Add(lb);
            lb.LinkLogger(gl);
        }

        public void Clean() {
            lb.Disconnect();
        }

        private void button1_Click(object sender, EventArgs e) {
            LogEntry[] entries = lb.GetEntries();
            StreamWriter sw = new StreamWriter("Logs\\" + DateTime.Now.ToFileTime() + ".log");
            foreach(LogEntry logEntry in entries) {
                sw.WriteLine($"{logEntry.EntryColor, -18}" + "| " + logEntry);
                sw.Flush();
            }
            sw.Close();
        }
    }
}
