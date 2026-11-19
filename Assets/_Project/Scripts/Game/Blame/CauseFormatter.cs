using System.Text;

namespace FB.Game.Blame
{
    /// <summary>
    /// Renders a cause chain from the roots down, e.g.
    /// "(Ти → HoseSpray → Puddle + Стара проводка → FaultyWiring) → Electrocution → Ти".
    /// </summary>
    public sealed class CauseFormatter
    {
        private const string Arrow = " → ";

        private readonly IActorRegistry _actors;
        private readonly StringBuilder _builder = new StringBuilder(256);

        public CauseFormatter(IActorRegistry actors)
        {
            _actors = actors;
        }

        public string Format(Cause cause, string victim)
        {
            _builder.Clear();
            Append(cause);
            if (!string.IsNullOrEmpty(victim))
            {
                _builder.Append(Arrow).Append(victim);
            }

            return _builder.ToString();
        }

        private void Append(Cause node)
        {
            int parents = node.Parents.Count;
            if (parents == 0)
            {
                _builder.Append(_actors.NameOf(node.Instigator));
            }
            else if (parents == 1)
            {
                Append(node.Parents[0]);
            }
            else
            {
                _builder.Append('(');
                for (int i = 0; i < parents; i++)
                {
                    if (i > 0)
                    {
                        _builder.Append(" + ");
                    }

                    Append(node.Parents[i]);
                }

                _builder.Append(')');
            }

            _builder.Append(Arrow).Append(node.Action);
        }
    }
}
