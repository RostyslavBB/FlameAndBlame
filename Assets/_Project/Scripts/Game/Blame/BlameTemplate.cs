using System;
using UnityEngine;

namespace FB.Game.Blame
{
    /// <summary>One feed line. Placeholders: {A} main culprit, {I} item, {S} source item.</summary>
    [Serializable]
    public struct BlameTemplate
    {
        [SerializeField] private NarrativeEvent _event;
        [SerializeField] private ActorFilter _filter;
        [SerializeField, TextArea] private string _text;

        public BlameTemplate(NarrativeEvent narrativeEvent, ActorFilter filter, string text)
        {
            _event = narrativeEvent;
            _filter = filter;
            _text = text;
        }

        public NarrativeEvent Event => _event;
        public ActorFilter Filter => _filter;
        public string Text => _text;

        public bool Fits(NarrativeEvent narrativeEvent, ActorId mainActor) =>
            _event == narrativeEvent &&
            (_filter == ActorFilter.Any || (_filter == ActorFilter.Player) == (mainActor == ActorId.Player));
    }
}
