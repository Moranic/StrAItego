using System.Collections.Generic;
using System.Linq;
using StrAItego.Game.TFLite;

namespace StrAItego.Game.Agents.NaiveNNAgent
{
    class NaiveNNAgent : RandomAgent.RandomAgent
    {
        float[] binBoard = new float[3312];
        TFLiteModel model;

        public NaiveNNAgent() : base("Naive NN. Agent") { }

        public override void Dispose() {
            base.Dispose();
            model?.Dispose();
        }

        public override Move? GetMove(Board board, GameLogger gameLogger) {
            if(model == null)
                model = TFLiteManager.GetModel("HumanMovePredictor2");
            List<Move> moves = board.GetValidMoves(Team.Red);
            if (moves.Count == 0)
                return null;

            binBoard = board.ToBinary(binBoard, Team.Red);

            //float[] pred = TensorflowManager.RequestPrediction(binBoard, shape: (1, binBoard.Length), "HumanMovePredictor")[0];
            float[] pred = model.Predict(binBoard);
            
            (float score, Move move)[] scores = new (float, Move)[moves.Count];
            for(int i = 0; i < scores.Length; i++) {
                scores[i] = (pred[Board.GetIndexOfMove(moves[i])], moves[i]);
            }

            (float score, Move move) bestMove = (0f, new Move());
            for(int i = 0; i < scores.Length; i++) {
                if(scores[i].score > bestMove.score) {
                    bestMove = scores[i];
                }
            }

            Move m = bestMove.move;

            if (gameLogger != null) {
                float total = scores.Sum(x => x.score);
                float multiplier = 1f / total;
                gameLogger.LogMessage($"Naive NN Agent evaluating {moves.Count} moves (total eval. score: {total})");
                scores = scores.OrderBy(x => x.score).ToArray();
                for (int i = scores.Length - 1; i >= 0; i--) {
                    gameLogger.LogMessage($"Eval: {$"{string.Format("{0:0.#######}", scores[i].score * multiplier),-9}",+11} => #move#", i == scores.Length - 1, scores[i].move);
                }
            }

            
            return m;
        }

        public override IAgentParameters GetParameters() {
            return new NaiveNNAgentParameters();
        }

        public override void SetParameters(IAgentParameters agentParameters) {
            base.SetParameters(agentParameters);
            NaiveNNAgentParameters parameters = (NaiveNNAgentParameters)agentParameters;
            name = parameters.ToString();
        }
    }
}
