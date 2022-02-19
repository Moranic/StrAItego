using StrAItego.Game;
using StrAItego.Game.Agents;
using StrAItego.UI;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using StrAItego.Properties;
using System.Threading;
using StrAItego.Game.TFLite;

namespace StrAItego
{
    public partial class StrAItego : Form
    {
        UI.Board board;
        Game.Game g;
        GameLogger gl;
        Dictionary<string, Type> agentTypes = new Dictionary<string, Type>();
        Dictionary<IAgentParameters, Type> agentConfigurations = new Dictionary<IAgentParameters, Type>();
        //List<(Type, IAgentParameters)> agentConfigurations = new List<(Type, IAgentParameters)>();
        Task mainGame;
        Task runner;

        Thread[] runnerThreads = new Thread[0];
        int startedCounter = 0, finishedCounter = 0;
        Task timer;
        bool cancelTimer = false;

        RunResults latestResults;


        public StrAItego() {
            Directory.CreateDirectory("Logs");
            Task tfliteinit = new Task(() => TFLiteManager.Init());
            tfliteinit.Start();
            InitializeComponent();
            //TensorflowManager.Initialise();
            
            DoubleBuffered = true;
            Icon = Resources.program;

            board = new UI.Board(splitContainer1.Panel1);
            splitContainer1.Panel1.Tag = board;
            //board.AttemptMove += TestClickAndDrag;
            List<Type> agents = GetAgents();
            foreach (Type t in agents)
                agentTypes.Add(Activator.CreateInstance(t).ToString(), t);

            splitContainer3.Panel1.Tag = Team.Red;
            splitContainer3.Panel2.Tag = Team.Blue;

            agentBox1.Items.AddRange(agentTypes.Keys.ToArray());
            agentBox2.Items.AddRange(agentTypes.Keys.ToArray());
            agentBox3.Items.AddRange(agentTypes.Keys.Where(x => ((IAgent)Activator.CreateInstance(agentTypes[x])).IsAI()).ToArray());
            agentBox1.SelectedValueChanged += OnAgentSelection;
            agentBox2.SelectedValueChanged += OnAgentSelection;
            agentBox3.SelectedValueChanged += OnAgentSelection;

            for (Team i = Team.Red; i <= Team.Both; i++)
                comboBox1.Items.Add(i);
            comboBox1.SelectedItem = Team.Both;
            comboBox1.SelectedValueChanged += OnChangeDrawTeam;

            // Uncomment here to calculate the total amount of possible moves in a simple but stupid way.
            //int total = 0;
            //int scouttotal = 0;
            //Game.Board b = new Game.Board();
            //for(Unit u = Unit.RedFlag; u <= Unit.RedBomb; u++) {
            //    for(Square s = Square.A1; s <= Square.K10; s++) {
            //        b.SetSquare(Unit.None, (s == Square.A1 ? Square.K10 : s - 1));
            //        b.SetSquare(u, s);
            //        int c = b.GetValidMoves(Team.Red).Count;
            //        total += c;
            //        scouttotal += u == Unit.RedScout ? c : 0;
            //    }
            //}
            //MessageBox.Show("Found total: " + total + ", excl. units: " + scouttotal);

            // Uncomment to write all possible moves to file.
            //using(StreamWriter sw = new StreamWriter("moves.txt")) {
            //    for(Square source = Square.A1; source <= Square.K10; source++) {
            //        //Find left-most square
            //        Square adjacent = source;
            //        while(Game.Board.GetAdjacentSquare(adjacent, Direction.West) != Square.None) {
            //            adjacent = Game.Board.GetAdjacentSquare(adjacent, Direction.West);
            //        }
            //        //Write pairs in left-right order
            //        while(adjacent != Square.None) {
            //            if(source != adjacent)
            //                sw.WriteLine($"({source},{adjacent})");
            //            adjacent = Game.Board.GetAdjacentSquare(adjacent, Direction.East);
            //        }
            //
            //        //Find bottom-most square
            //        adjacent = source;
            //        while (Game.Board.GetAdjacentSquare(adjacent, Direction.South) != Square.None) {
            //            adjacent = Game.Board.GetAdjacentSquare(adjacent, Direction.South);
            //        }
            //        //Write pairs in bottom-top order
            //        while (adjacent != Square.None) {
            //            if (source != adjacent)
            //                sw.WriteLine($"({source},{adjacent})");
            //            adjacent = Game.Board.GetAdjacentSquare(adjacent, Direction.North);
            //        }
            //    }
            //}
            tfliteinit.Wait();
        }


        void OnAgentSelection(object sender, EventArgs e) {
            ComboBox cb = (ComboBox)sender;
            Control parent = cb.Parent;
            Panel pp = parent.Controls.OfType<Panel>().First();
            pp.Controls.Clear();
            if (cb == agentBox3) {
                button2.Text = "Add";
                button2.Enabled = cb.SelectedItem != null;
            }
            configurationsBox.Enabled = true;
            if (cb.SelectedItem == null)
                return;
            IAgent agent = (IAgent)Activator.CreateInstance(agentTypes[(string)cb.SelectedItem]);
            IAgentParameters parameters = agent.GetParameters();
            Panel p = parameters.GetControls();
            p.BorderStyle = BorderStyle.Fixed3D;
            p.Dock = DockStyle.Fill;
            pp.Controls.Add(p);
        }

        List<Type> GetAgents() {
            List<Type> agents = new List<Type>();
            Type ti = typeof(IAgent);
            foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies()) {
                foreach (Type t in asm.GetTypes()) {
                    if (ti.IsAssignableFrom(t) && t.IsClass && !t.IsAbstract) {
                        agents.Add(t);
                    }
                }
            }
            return agents;
        }

        void OnChangeDrawTeam(object sender, EventArgs e) {
            board.ChangeDrawTeam((Team)comboBox1.SelectedItem);
        }

        private void button1_Click(object sender, EventArgs e) {
            if (agentBox1.SelectedItem == null || agentBox2.SelectedItem == null)
                return;
            board.DisconnectGame();
            g?.CancelGame();
            gl = null;

            IAgent redAgent = (IAgent)Activator.CreateInstance(agentTypes[(string)agentBox1.SelectedItem]);
            IAgent blueAgent = (IAgent)Activator.CreateInstance(agentTypes[(string)agentBox2.SelectedItem]);
            IAgentParameters redParameters = splitContainer3.Panel1.Controls.Find("ParametersPanel", true)[0].Tag as IAgentParameters;
            IAgentParameters blueParameters = splitContainer3.Panel2.Controls.Find("ParametersPanel", true)[0].Tag as IAgentParameters;
            
            if(!redParameters.IsValid() || !blueParameters.IsValid()) {
                MessageBox.Show("Invalid parameters!");
                return;
            }
            
            redParameters.ResetRandom();
            blueParameters.ResetRandom();
            
            redAgent.SetParameters(redParameters);
            blueAgent.SetParameters(blueParameters);

            gl = new GameLogger();
            g = new Game.Game(redAgent, blueAgent);
            gl.Link(g);
            board.ConnectGame(g, (Team)comboBox1.SelectedItem);
            mainGame = new Task(() => {
                Team result = g.PlayGame(numericUpDown1, gl);
                if(result != Team.Neither)
                    MessageBox.Show(result.ToString() + " wins the game!");
                });
            mainGame.Start();
        }

        private void button2_Click(object sender, EventArgs e) {
            if (agentBox3.SelectedItem == null)
                return;
            Type agentType = agentTypes[(string)agentBox3.SelectedItem];
            IAgentParameters parameters = splitContainer4.Panel2.Controls.Find("ParametersPanel", true)[0].Tag as IAgentParameters;
            if (agentConfigurations.ContainsKey(parameters)) {
                agentConfigurations.Remove(parameters);
                configurationsBox.Items.Remove(parameters);
            }
            agentConfigurations.Add(parameters, agentType);
            configurationsBox.Items.Add(parameters);
            agentBox3.SelectedItem = null;
            button2.Enabled = false;
            button5.Enabled = agentConfigurations.Count >= 2;
        }

        private void button4_Click(object sender, EventArgs e) {
            if (configurationsBox.SelectedItem == null)
                return;
            agentConfigurations.Remove((IAgentParameters)configurationsBox.SelectedItem);
            configurationsBox.Items.Remove(configurationsBox.SelectedItem);
            button2.Text = "Add";
            button2.Enabled = agentBox3.SelectedItem != null;
            configurationsBox.Enabled = true;
            button5.Enabled = agentConfigurations.Count >= 2;
        }

        private void button3_Click(object sender, EventArgs e) {
            if (configurationsBox.SelectedItem == null)
                return;

            agentBox3.SelectedItem = agentTypes.First(x => x.Value == agentConfigurations[(IAgentParameters)configurationsBox.SelectedItem]).Key;
            Control parent = agentBox3.Parent;
            Panel pp = parent.Controls.OfType<Panel>().First();
            pp.Controls.Clear();
            pp.Controls.Add(((IAgentParameters)configurationsBox.SelectedItem).GetControls());
            button2.Text = "Confirm changes";
            button2.Enabled = true;
            configurationsBox.Enabled = false;
        }

        private void configurationsBox_SelectedIndexChanged(object sender, EventArgs e) {
            button4.Enabled = configurationsBox.SelectedItem != null;
            button3.Enabled = configurationsBox.SelectedItem != null;
            if(configurationsBox.Items.Count >= 2) {
                button6.Enabled = configurationsBox.SelectedItem != null;
            }
            else {
                button6.Enabled = false;
            }
        }

        private void button5_Click(object sender, EventArgs e) {
            CancelRun();
            int threads = (int)noOfThreadsBox.Value;
            runner = new Task(() => {
                // Copy data to new thread.
                Dictionary<IAgentParameters, Type> configs = new Dictionary<IAgentParameters, Type>();
                foreach (KeyValuePair<IAgentParameters, Type> x in agentConfigurations)
                    configs.Add(x.Key, x.Value);
                int noOfGames = (int)numericUpDown2.Value;

                IAgentParameters[] parameters = configs.Keys.ToArray();
                // Prepare games
                Game.Game[] games = new Game.Game[configs.Keys.Count * (configs.Keys.Count - 1) * noOfGames];

                for (int i = 0; i < noOfGames; i++) {
                    for (int j = 0; j < parameters.Length; j++) {
                        for (int k = 0; k < parameters.Length; k++) {
                            if (j != k) {
                                IAgent redPlayer = (IAgent)Activator.CreateInstance(configs[parameters[j]]);
                                IAgent bluePlayer = (IAgent)Activator.CreateInstance(configs[parameters[k]]);

                                redPlayer.SetParameters(parameters[j]);
                                bluePlayer.SetParameters(parameters[k]);

                                games[i * configs.Keys.Count * (configs.Keys.Count - 1) + j * (configs.Keys.Count - 1) + (k > j ? k - 1 : k)] =
                                new Game.Game(redPlayer, bluePlayer);
                            }
                        }
                    }
                }

                RunGames(games, threads);
            });
            runner.Start();
        }

        private void button8_Click(object sender, EventArgs e) {
            ResultsWindow r = new ResultsWindow(latestResults);
            r.ShowDialog();
            r.Dispose();
        }

        private void button6_Click(object sender, EventArgs e) {    // Play Selected Games
            CancelRun();
            IAgentParameters selected = (IAgentParameters)configurationsBox.SelectedItem;
            int threads = (int)noOfThreadsBox.Value;
            runner = new Task(() => {
                // Copy data to new thread.
                Dictionary<IAgentParameters, Type> configs = new Dictionary<IAgentParameters, Type>();
                foreach (KeyValuePair<IAgentParameters, Type> x in agentConfigurations)
                    configs.Add(x.Key, x.Value);
                int noOfGames = (int)numericUpDown2.Value;

                IAgentParameters[] parameters = configs.Keys.Where(x => x != selected).ToArray();

                // Prepare games
                Game.Game[] games = new Game.Game[2 * (configs.Keys.Count - 1) * noOfGames];

                for (int i = 0; i < noOfGames; i++) {
                    for (int k = 0; k < parameters.Length; k++) {
                        IAgent redPlayer = (IAgent)Activator.CreateInstance(configs[selected]);
                        IAgent bluePlayer = (IAgent)Activator.CreateInstance(configs[parameters[k]]);

                        redPlayer.SetParameters(selected);
                        bluePlayer.SetParameters(parameters[k]);

                        games[i * (2 * parameters.Length) + 2 * k] =
                        new Game.Game(redPlayer, bluePlayer);

                        redPlayer = (IAgent)Activator.CreateInstance(configs[parameters[k]]);
                        bluePlayer = (IAgent)Activator.CreateInstance(configs[selected]);

                        bluePlayer.SetParameters(selected);
                        redPlayer.SetParameters(parameters[k]);

                        games[i * (2 * parameters.Length) + 2 * k + 1] =
                        new Game.Game(redPlayer, bluePlayer);
                    }
                }

                RunGames(games, threads);
            });
            runner.Start();
        }

        private void RunGames(Game.Game[] games, int noOfThreads) {
            timer = new Task(() => {
                while (!cancelTimer) {
                    int currentCounter = 100 * finishedCounter;
                    int progress = currentCounter / games.Length;
                    progressBar1.Invoke((MethodInvoker)delegate {
                        progressBar1.Value = progress;
                    });
                    Thread.Sleep(500);
                }
            });
            timer.Start();

            runnerThreads = new Thread[noOfThreads];
            for(int i = 0; i < runnerThreads.Length; i++) {
                runnerThreads[i] = new Thread(() => RunGame(games, ref startedCounter, ref finishedCounter));
                runnerThreads[i].IsBackground = true;
                runnerThreads[i].Priority = ThreadPriority.AboveNormal;
            }
            foreach (Thread t in runnerThreads)
                t.Start();

            button7.Invoke((MethodInvoker)delegate { button7.Enabled = true; });

            foreach (Thread t in runnerThreads)
                t.Join();
            bool aborted = finishedCounter < games.Length;

            
            //Task[] tasks = new Task[games.Length];
            //for (int i = 0; i < games.Length; i++) {
            //    int gindex = i;
            //    tasks[gindex] = new Task(() => {
            //        Team result = games[gindex].PlayGame();
            //
            //        Interlocked.Increment(ref finishedCounter);
            //    });
            //    tasks[gindex].Start();
            //}
            //Task.WaitAll(tasks);

            cancelTimer = true;
            if(!aborted)
                progressBar1.Invoke((MethodInvoker)delegate {
                    progressBar1.Value = 100;
                });
            //MessageBox.Show((new RunResults(games)).ToString());
            latestResults = new RunResults(games);

            if(!aborted)
                button8.Invoke((MethodInvoker)delegate {
                    button8.Enabled = true;
                });
            timer.Wait();

            GC.Collect(2);
        }

        void CancelRun() {
            cancelTimer = true;
            //if (timer != null)
            //    timer.Wait();
            //timer?.Dispose();
            //timer = null;
            //cancelTimer = false;
            progressBar1.Value = 0;
            foreach (Thread t in runnerThreads)
                t.Abort();
            foreach (Thread t in runnerThreads)
                t.Join();
            runner?.Wait();
            timer = null;
            runner = null;
            cancelTimer = false;
            runnerThreads = new Thread[0];
            startedCounter = 0;
            finishedCounter = 0;
            if(latestResults != null)
                button8.Enabled = true;
            button7.Enabled = false;
        }

        private void button7_Click(object sender, EventArgs e) {
            CancelRun();
        }

        private void button9_Click(object sender, EventArgs e) {
            if (gl != null) {
                LogWindow l = new LogWindow(gl);
                Task t = new Task(() => {
                    l.ShowDialog();
                    l.Clean();
                    l.Dispose();
                });
                t.Start();
            }
        }

        static void RunGame(Game.Game[] games, ref int startedCounter, ref int finishedCounter) {
            while (true) {
                int toStart = Interlocked.Increment(ref startedCounter) - 1;
                if (toStart >= games.Length)
                    return;
                try {
                    games[toStart].PlayGame();
                }
                catch(Exception ex) {
                    if (ex is ThreadAbortException) {
                        Thread.ResetAbort();
                        return;
                    }
                    Task t = new Task(() => MessageBox.Show("Exception raised in thread!\n\r" + games[toStart].ToString() + "\n\r" + ex.Message));
                    t.Start();
                    games[toStart].CancelGame();
                    games[toStart].PlayGame(null, null, true);    // Sets result to Team.Neither.
                }
                Interlocked.Increment(ref finishedCounter);
                if (Environment.WorkingSet > 12884895291) //12 GB
                    GC.Collect(2);
            }
        }
    }
}
