using System.Collections.Generic;

namespace FB.Game.Blame
{
    public sealed class BlameLedger : IBlameLedger
    {
        private readonly float[] _damage;
        private readonly float[] _points;
        private readonly List<BlameRecord> _records = new List<BlameRecord>(64);

        public BlameLedger(IActorRegistry actors)
        {
            _damage = new float[actors.Count];
            _points = new float[actors.Count];
        }

        public int TotalDamage => FireDamage + FirefighterDamage;
        public int FireDamage { get; private set; }
        public int FirefighterDamage { get; private set; }
        public float TotalPoints { get; private set; }
        public IReadOnlyList<BlameRecord> Records => _records;

        public float DamageOf(ActorId actor) => _damage[(int)actor];
        public float PointsOf(ActorId actor) => _points[(int)actor];

        /// <summary>Splits damage and points between root actors by their shares.</summary>
        public void Add(float[] shares, int damage, float points, BlameRecord record)
        {
            float firefighter = 0f;
            for (int i = 0; i < shares.Length; i++)
            {
                _damage[i] += damage * shares[i];
                _points[i] += points * shares[i];
                if ((ActorId)i == ActorId.Player)
                {
                    firefighter = damage * shares[i];
                }
            }

            int firefighterDollars = (int)System.Math.Round(firefighter);
            FirefighterDamage += firefighterDollars;
            FireDamage += damage - firefighterDollars;
            TotalPoints += points;
            _records.Add(record);
        }
    }
}
