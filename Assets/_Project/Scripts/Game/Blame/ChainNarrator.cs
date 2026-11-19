using System;
using System.Collections.Generic;

namespace FB.Game.Blame
{
    /// <summary>
    /// Picks a random template for an event and fills in names. Never repeats the previous line of the same
    /// event type when there is an alternative.
    /// </summary>
    public sealed class ChainNarrator
    {
        private readonly IReadOnlyList<BlameTemplate> _templates;
        private readonly IActorRegistry _actors;
        private readonly Random _random = new Random();
        private readonly List<int> _candidates = new List<int>(16);
        private readonly int[] _lastPicked;

        public ChainNarrator(BlameTextConfig config, IActorRegistry actors)
        {
            _templates = config.Templates;
            _actors = actors;
            _lastPicked = new int[Enum.GetValues(typeof(NarrativeEvent)).Length];
            for (int i = 0; i < _lastPicked.Length; i++)
            {
                _lastPicked[i] = -1;
            }
        }

        public string Narrate(NarrativeEvent narrativeEvent, ActorId mainActor, string item, string source)
        {
            _candidates.Clear();
            for (int i = 0; i < _templates.Count; i++)
            {
                if (_templates[i].Fits(narrativeEvent, mainActor))
                {
                    _candidates.Add(i);
                }
            }

            if (_candidates.Count == 0)
            {
                return $"{item}: {narrativeEvent}";
            }

            int last = _lastPicked[(int)narrativeEvent];
            if (_candidates.Count > 1)
            {
                _candidates.Remove(last);
            }

            int picked = _candidates[_random.Next(_candidates.Count)];
            _lastPicked[(int)narrativeEvent] = picked;
            return _templates[picked].Text
                .Replace("{A}", _actors.NameOf(mainActor))
                .Replace("{I}", item ?? string.Empty)
                .Replace("{S}", source ?? string.Empty);
        }
    }
}
