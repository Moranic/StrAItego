using StrAItego.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StrAItego
{
    public partial class ResultsWindow : Form
    {

        DataGridView d;
        DataGridView l;
        public ResultsWindow(RunResults r) {
            InitializeComponent();

            d = new DataGridView();
            d.RowHeadersVisible = false;

            l = new DataGridView();
            l.RowHeadersVisible = false;

            List<string> redPlayers = r.Results.Keys.Select(x => x.Item1).Distinct().ToList();
            List<string> bluePlayers = r.Results.Keys.Select(x => x.Item2).Distinct().ToList();

            redPlayers.Sort();
            bluePlayers.Sort();

            // Add header row

            d.Columns.Add(new DataGridViewColumn(new DataGridViewTextBoxCell()));
            d.Columns[0].Name = "↓ Red/Blue →";
            foreach (string bp in bluePlayers) {
                int i = d.Columns.Add(new DataGridViewColumn(new DataGridViewTextBoxCell()));
                d.Columns[i].Name = bp;
            }

            l.Columns.Add(new DataGridViewColumn(new DataGridViewTextBoxCell()));
            l.Columns[0].Name = "Matchup";
            l.Columns.Add(new DataGridViewColumn(new DataGridViewTextBoxCell()));
            l.Columns[1].Name = "Win Rate";


            foreach (string red in redPlayers) {
                string[] row = new string[bluePlayers.Count + 1];
                row[0] = red;
                int i = 1;
                foreach(string blue in bluePlayers) {
                    if (r.Results.ContainsKey((red, blue))) {
                        row[i] = r.Results[(red, blue)].Item1 + "/" + r.Results[(red, blue)].Item2;
                    }
                    else
                        row[i] = "";
                    i++;
                }
                d.Rows.Add(row);
            }

            List<(string[], float)> rows = new List<(string[], float)>();
            foreach((string Red, string Blue) in r.Results.Keys) {
                string[] row = new string[2];
                row[0] = Red + " vs. " + Blue;
                (int rw, int bw) r1 = r.Results[(Red, Blue)];
                (int bw, int rw) r2 = (0, 0);
                r.Results.TryGetValue((Blue, Red), out r2);
                float value = (r1.rw + r2.rw) / (float)(r1.rw + r1.bw + r2.rw + r2.bw) * 100f;
                row[1] =  value + "%";
                rows.Add((row, value));
            }
            rows.Sort((x, y) => y.Item2.CompareTo(x.Item2));
            foreach ((string[] row, float _) in rows)
                l.Rows.Add(row);

            d.Dock = DockStyle.Top;
            d.EditMode = DataGridViewEditMode.EditProgrammatically;
            d.AllowUserToAddRows = false;
            d.AllowUserToDeleteRows = false;
            d.AllowUserToResizeRows = false;

            d.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            splitContainer1.Panel1.Controls.Add(l);
            splitContainer1.Panel1.Controls.Add(d);

            int totalRowHeight = d.ColumnHeadersHeight;

            foreach (DataGridViewRow row in d.Rows)
                totalRowHeight += row.Height;

            d.Height = totalRowHeight + SystemInformation.HorizontalScrollBarHeight + 2;

            l.Dock = DockStyle.Top;
            l.EditMode = DataGridViewEditMode.EditProgrammatically;
            l.AllowUserToAddRows = false;
            l.AllowUserToDeleteRows = false;
            l.AllowUserToResizeRows = false;
            l.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            totalRowHeight = l.ColumnHeadersHeight;

            foreach (DataGridViewRow row in l.Rows)
                totalRowHeight += row.Height;

            l.Height = totalRowHeight + SystemInformation.HorizontalScrollBarHeight + 2;

            splitContainer1.Panel1.AutoScroll = true;
            //splitContainer1.IsSplitterFixed = false;
            //splitContainer1.MinimumSize = new Size(Math.Min(700, Math.Max(l.Width, d.Width) + SystemInformation.VerticalScrollBarWidth + 2), Math.Min(900, l.Height + d.Height + d.Margin.Bottom + l.Margin.Top + 50));
            //splitContainer1.Width = splitContainer1.MinimumSize.Width;
            //splitContainer1.Height = splitContainer1.MinimumSize.Height;
            //splitContainer1.SplitterDistance = l.Height + d.Height + d.Margin.Bottom + l.Margin.Top;
            //splitContainer1.IsSplitterFixed = true;
            //MinimumSize = splitContainer1.MinimumSize;
            MaximumSize = new Size(1500, l.Height + d.Height + d.Margin.Bottom + l.Margin.Top + 100);
        }
    }
}
