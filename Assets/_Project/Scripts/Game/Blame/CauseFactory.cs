namespace FB.Game.Blame
{
    public sealed class CauseFactory : ICauseFactory
    {
        private int _nextId = 1;

        public Cause Create(ActorId instigator, CauseAction action) =>
            new Cause(_nextId++, action, instigator, null);

        public Cause Create(ActorId instigator, CauseAction action, Cause parent) =>
            new Cause(_nextId++, action, instigator, parent == null ? null : new[] { parent });

        public Cause Create(ActorId instigator, CauseAction action, Cause first, Cause second)
        {
            if (first == null || second == null)
            {
                return Create(instigator, action, first ?? second);
            }

            return new Cause(_nextId++, action, instigator, new[] { first, second });
        }
    }
}
