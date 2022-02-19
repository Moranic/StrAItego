using System;
using System.Collections.Generic;
using System.Drawing;

namespace StrAItego.Game
{
    /// <summary>
    /// Object that handles debug messages from agents playing a game. It can be accessed by using the "View Log" button.
    /// </summary>
    public class GameLogger
    {
        Game linkedGame;
        List<LogEntry> entries = new List<LogEntry>();

        /// <summary>
        /// Links a game to this logger.
        /// </summary>
        public void Link(Game g) {
            if (linkedGame != null)
                linkedGame.MoveMade -= OnMoveMade;
            g.MoveMade += OnMoveMade;
            linkedGame = g;
            LogEntryMade += OnEntryMade;
        }

        /// <summary>
        /// Log a debug message. Will be presented in the colour of the agent that made the log entry.
        /// </summary>
        /// <param name="message">The message that should be logged.</param>
        /// <param name="bold">Whether or not this message should be displayed in <b>Bold</b></param>
        /// <param name="relatedMove">The move this message is related to. Any mentions of the text "<b>#move#</b>" will be replaced with the pretty-printed move.</param>
        public void LogMessage(string message, bool bold = false, Move? relatedMove = null) {
            Color c = Turn == Team.Red ? Color.LightCoral : Color.SkyBlue;
            if(relatedMove != null) {
                Move m = (Move)relatedMove;
                if (Turn == Team.Blue)
                    m = m.GetInvertedMove();
                message = message.Replace("#move#", m.LogString());
            }
            LogEntryMade?.Invoke(this, new LogEntryEventArgs(message, c, bold));
        }

        void OnMoveMade(object sender, MoveMadeEventArgs e) {
            LogEntryMade?.Invoke(this, new LogEntryEventArgs(e.Move.LogString(), Color.Gainsboro, true));
        }

        Team Turn {
            get { return linkedGame.GetTurn; }
        }

        void OnEntryMade(object sender, LogEntryEventArgs e) {
            entries.Add(e.Entry);
        }

        public EventHandler<LogEntryEventArgs> LogEntryMade;

        public LogEntry[] GetEntries() {
            return entries.ToArray();
        }
    }

    public class LogEntry
    {
        public string EntryText { get; }
        public Color EntryColor { get; }
        public bool Bold { get; }
        public DateTime Time { get; }

        public LogEntry(string entryText, Color entryColor, bool bold) {
            EntryText = entryText;
            EntryColor = entryColor;
            Bold = bold;
            Time = DateTime.Now;
        }

        public override string ToString() {
            return $"[{Time.TimeOfDay}] {EntryText}";
        }
    }

    public class LogEntryEventArgs
    {
        public LogEntry Entry { get; }
        public LogEntryEventArgs(string EntryText, Color EntryColor, bool Bold) {
            Entry = new LogEntry(EntryText, EntryColor, Bold);
        }
    }
}
