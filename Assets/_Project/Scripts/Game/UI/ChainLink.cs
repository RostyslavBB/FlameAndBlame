using UnityEngine;

namespace FB.Game.UI
{
    /// <summary>One card of the blame chain shown on the report: who, what they did, in the actor's color.</summary>
    public readonly struct ChainLink
    {
        public ChainLink(string title, string subtitle, Color color)
        {
            Title = title;
            Subtitle = subtitle;
            Color = color;
        }

        public string Title { get; }
        public string Subtitle { get; }
        public Color Color { get; }
    }
}
