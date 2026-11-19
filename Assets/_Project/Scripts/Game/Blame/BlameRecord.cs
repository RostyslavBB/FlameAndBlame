namespace FB.Game.Blame
{
    /// <summary>One entry of the blame ledger: what happened, how much it cost and who is mainly to blame.</summary>
    public sealed class BlameRecord
    {
        public BlameRecord(string text, string chain, int damage, int severity, ActorId mainActor)
        {
            Text = text;
            Chain = chain;
            Damage = damage;
            Severity = severity;
            MainActor = mainActor;
        }

        /// <summary>Funny line for the feed.</summary>
        public string Text { get; }

        /// <summary>Formatted cause chain, e.g. "Ти → HoseSpray → Puddle → Electrocution → Ти".</summary>
        public string Chain { get; }

        public int Damage { get; }

        /// <summary>1 (minor) .. 3 (big).</summary>
        public int Severity { get; }

        public ActorId MainActor { get; }
    }
}
