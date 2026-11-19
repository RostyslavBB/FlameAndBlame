using UnityEngine;

namespace FB.Game.UI
{
    public readonly struct VerdictResult
    {
        public VerdictResult(string text, string stampLabel, Color color)
        {
            Text = text;
            StampLabel = stampLabel;
            Color = color;
        }

        public string Text { get; }
        public string StampLabel { get; }
        public Color Color { get; }
    }
}
