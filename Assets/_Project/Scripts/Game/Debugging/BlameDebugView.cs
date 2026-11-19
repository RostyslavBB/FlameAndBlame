using UnityEngine;

namespace FB.Game.Debugging
{
    /// <summary>Passive IMGUI panel for the blame debug overlay. Shows whatever text the presenter sets.</summary>
    public sealed class BlameDebugView : MonoBehaviour
    {
        private const float Width = 760f;

        private string _text = string.Empty;
        private bool _visible;
        private GUIStyle _style;

        public void SetVisible(bool visible) => _visible = visible;

        public void SetText(string text) => _text = text;

        private void OnGUI()
        {
            if (!_visible)
            {
                return;
            }

            _style ??= new GUIStyle(GUI.skin.box)
            {
                alignment = TextAnchor.UpperLeft,
                fontSize = 15,
                wordWrap = true,
                richText = true,
                padding = new RectOffset(12, 12, 10, 10)
            };
            float height = _style.CalcHeight(new GUIContent(_text), Width);
            GUI.Box(new Rect(12f, 12f, Width, height), _text, _style);
        }
    }
}
