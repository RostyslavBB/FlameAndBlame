using System;
using UnityEngine;

namespace FB.Game.Fire
{
    /// <summary>One fire outbreak per round: a random prop from the candidates catches fire after a delay.</summary>
    [Serializable]
    public struct FireStartGroup
    {
        [SerializeField] private string _room;
        [SerializeField] private string[] _candidates;
        [SerializeField, Min(0f)] private float _delay;

        public FireStartGroup(string room, float delay, params string[] candidates)
        {
            _room = room;
            _delay = delay;
            _candidates = candidates;
        }

        public string Room => _room;

        /// <summary>Prop ids (GameObject names); one is picked at random each round.</summary>
        public string[] Candidates => _candidates;

        /// <summary>Seconds after the round starts.</summary>
        public float Delay => _delay;
    }
}
