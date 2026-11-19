namespace FB.Game.Blame
{
    /// <summary>One entry of the blame ledger: what happened, how much it cost and who is mainly to blame.</summary>
    public sealed class BlameRecord
    {
        public BlameRecord(NarrativeEvent kind, string text, string chain, int damage, int severity, ActorId mainActor,
            Cause cause, string target)
        {
            Cause = cause;
            Target = target;
            Kind = kind;
            Text = text;
            Chain = chain;
            Damage = damage;
            Severity = severity;
            MainActor = mainActor;
        }

        /// <summary>What kind of event this was (picks the feed line, lets the UI filter).</summary>
        public NarrativeEvent Kind { get; }

        /// <summary>Funny line for the feed.</summary>
        public string Text { get; }

        /// <summary>Formatted cause chain, e.g. "Ти → HoseSpray → Puddle → Electrocution → Ти".</summary>
        public string Chain { get; }

        public int Damage { get; }

        /// <summary>1 (minor) .. 3 (big).</summary>
        public int Severity { get; }

        public ActorId MainActor { get; }

        /// <summary>The cause of this event; its parents lead back to the root culprits.</summary>
        public Cause Cause { get; }

        /// <summary>Who or what got hurt, ready for display ("Сусід", "«Штора»").</summary>
        public string Target { get; }
    }
}
