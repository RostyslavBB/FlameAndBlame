using System.Collections.Generic;
using UnityEngine;

namespace FB.Game.Blame
{
    /// <summary>Funny feed lines per event type (Ukrainian). Edited by writers, not programmers.</summary>
    [CreateAssetMenu(menuName = "FB/Blame Text Config", fileName = "BlameTextConfig")]
    public sealed class BlameTextConfig : ScriptableObject
    {
        [SerializeField] private List<BlameTemplate> _templates = new List<BlameTemplate>();

        public IReadOnlyList<BlameTemplate> Templates => _templates;

#if UNITY_EDITOR
        public void EditorSetTemplates(IEnumerable<BlameTemplate> templates)
        {
            _templates = new List<BlameTemplate>(templates);
        }
#endif
    }
}
