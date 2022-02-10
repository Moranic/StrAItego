using StrAItego.Game.Agents.MCTSAgents.BoardEstimators;
using StrAItego.Game.Agents.MCTSAgents.BoardEvaluators;
using StrAItego.Game.Agents.RandomAgent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace StrAItego.Game.Agents.MCTSAgents
{
    abstract class MCTSAgentParameters : RandomAgentParameters
    {
        public int Rollouts { get; set; } = 10000;
        public int Estimations { get; set; } = 1;

        Label rolloutsLabel, estimationsLabel;
        NumericUpDown rollouts, estimations;
        protected ComboBox boardEvaluators, boardEstimators;
        Label boardEvaluatorsLabel, boardEstimatorsLabel;
        Type selectedEvaluator, selectedEstimator;
        protected string evaluatorName = "", estimatorName = "";

        static Dictionary<string, Type> evaluatorTypes = new Dictionary<string, Type>();
        static Dictionary<string, Type> estimatorTypes = new Dictionary<string, Type>();

        static MCTSAgentParameters() {
            List<Type> evaluators = new List<Type>();
            Type ti = typeof(IBoardEvaluator);
            foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies()) {
                foreach (Type t in asm.GetTypes()) {
                    if (ti.IsAssignableFrom(t) && t.IsClass && !t.IsAbstract) {
                        evaluators.Add(t);
                    }
                }
            }

            foreach(Type t in evaluators) {
                IBoardEvaluator be = (IBoardEvaluator)Activator.CreateInstance(t);
                evaluatorTypes.Add(be.ToString(), t);
            }

            List<Type> estimators = new List<Type>();
            ti = typeof(IBoardEstimator);
            foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies()) {
                foreach(Type t in asm.GetTypes()) {
                    if (ti.IsAssignableFrom(t) && t.IsClass && !t.IsAbstract) {
                        estimators.Add(t);
                    }
                }
            }

            foreach(Type t in estimators) {
                IBoardEstimator be = (IBoardEstimator)Activator.CreateInstance(t);
                estimatorTypes.Add(be.ToString(), t);
            }
        }

        public MCTSAgentParameters() : base() {
            rolloutsLabel = new Label();
            rolloutsLabel.Text = "No. of rollouts:";
            
            rollouts = new NumericUpDown();
            rollouts.ValueChanged += OnRolloutsChange;
            rollouts.Minimum = 1;
            rollouts.Maximum = int.MaxValue;
            rollouts.Increment = 10000;
            rollouts.Value = 10000;
            rollouts.RightToLeft = RightToLeft.Yes;
            rollouts.UpDownAlign = LeftRightAlignment.Left;

            estimationsLabel = new Label();
            estimationsLabel.Text = "No. of estimations:";
            
            estimations = new NumericUpDown();
            estimations.ValueChanged += OnEstimationsChange;
            estimations.Minimum = 1;
            estimations.Maximum = int.MaxValue;
            estimations.Increment = 1;
            estimations.Value = 1;
            estimations.RightToLeft = RightToLeft.Yes;
            estimations.UpDownAlign = LeftRightAlignment.Left;

            boardEvaluatorsLabel = new Label();
            boardEvaluatorsLabel.Text = "Select board evaluator:";

            boardEstimatorsLabel = new Label();
            boardEstimatorsLabel.Text = "Select board estimator:";

            boardEvaluators = new ComboBox();
            boardEvaluators.Sorted = true;
            boardEvaluators.Items.AddRange(evaluatorTypes.Keys.ToArray());
            boardEvaluators.SelectedValueChanged += OnBoardEvaluatorSelectionChanged;

            boardEstimators = new ComboBox();
            boardEstimators.Sorted = true;
            boardEstimators.Items.AddRange(estimatorTypes.Keys.ToArray());
            boardEstimators.SelectedValueChanged += OnBoardEstimatorSelectionChanged;

            rollouts.Dock = DockStyle.Top;
            rolloutsLabel.Dock = DockStyle.Top;
            estimations.Dock = DockStyle.Top;
            estimationsLabel.Dock = DockStyle.Top;
            boardEvaluators.Dock = DockStyle.Top;
            boardEvaluatorsLabel.Dock = DockStyle.Top;
            boardEstimators.Dock = DockStyle.Top;
            boardEstimatorsLabel.Dock = DockStyle.Top;

            panel.Controls.Add(rollouts);
            panel.Controls.Add(rolloutsLabel);

            panel.Controls.Add(boardEvaluators);
            panel.Controls.Add(boardEvaluatorsLabel);

            panel.Controls.Add(estimations);
            panel.Controls.Add(estimationsLabel);

            panel.Controls.Add(boardEstimators);
            panel.Controls.Add(boardEstimatorsLabel);

            panel.AutoScroll = true;
        }

        public override bool IsValid() {
            return base.IsValid() && selectedEvaluator != null && selectedEstimator != null;
        }

        void OnRolloutsChange(object sender, EventArgs e) {
            Rollouts = (int)rollouts.Value;
        }

        void OnEstimationsChange(object sender, EventArgs e) {
            Estimations = (int)estimations.Value;
        }

        public IBoardEvaluator GetBoardEvaluator {
            get { return (IBoardEvaluator)Activator.CreateInstance(selectedEvaluator); }
        }

        public IBoardEstimator GetBoardEstimator {
            get { return (IBoardEstimator)Activator.CreateInstance(selectedEstimator); }
        }

        void OnBoardEvaluatorSelectionChanged(object sender, EventArgs e) {
            selectedEvaluator = evaluatorTypes[(string)boardEvaluators.SelectedItem];
            evaluatorName = (string)boardEvaluators.SelectedItem;
        }

        void OnBoardEstimatorSelectionChanged(object sender, EventArgs e) {
            selectedEstimator = estimatorTypes[(string)boardEstimators.SelectedItem];
            estimatorName = (string)boardEstimators.SelectedItem;
        }

        public override string ToString() {
            return "MCTS Agent (est: " + estimatorName + " (" + Estimations + "), eval: " + evaluatorName + " (" + Rollouts + "), seed: " + (randomSeed.Checked ? rseed : tseed) + ") w. " + setupProviderName;
        }
    }
}
