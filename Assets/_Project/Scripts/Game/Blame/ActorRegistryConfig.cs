using System.Collections.Generic;
using UnityEngine;

namespace FB.Game.Blame
{
    /// <summary>Display names and badge colors of everyone who can be blamed.</summary>
    [CreateAssetMenu(menuName = "FB/Actor Registry Config", fileName = "ActorRegistryConfig")]
    public sealed class ActorRegistryConfig : ScriptableObject
    {
        [SerializeField] private List<ActorEntry> _actors = new List<ActorEntry>();

        public IReadOnlyList<ActorEntry> Actors => _actors;

#if UNITY_EDITOR
        public void EditorSetActors(IEnumerable<ActorEntry> actors)
        {
            _actors = new List<ActorEntry>(actors);
        }
#endif
    }
}
