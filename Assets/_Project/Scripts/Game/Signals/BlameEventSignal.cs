using FB.Game.Blame;
using UnityEngine;

namespace FB.Game.Signals
{
    /// <summary>A narrated blame event, ready for the UI feed.</summary>
    public readonly struct BlameEventSignal
    {
        public BlameEventSignal(BlameRecord record, Color actorColor, string actorName)
        {
            Record = record;
            ActorColor = actorColor;
            ActorName = actorName;
        }

        public BlameRecord Record { get; }
        public string Text => Record.Text;
        public int Damage => Record.Damage;
        public int Severity => Record.Severity;
        public NarrativeEvent Kind => Record.Kind;
        public ActorId Actor => Record.MainActor;
        public Color ActorColor { get; }
        public string ActorName { get; }
    }
}
