using System.Collections.Generic;

namespace FB.Game.Blame
{
    /// <summary>
    /// The blame DAG. Causes already point to their parents; the graph registers every node it has seen and
    /// answers "whose fault is this?": the weight of an event (1.0) flows from the node up to the roots,
    /// split equally between parents at every step. Pure C#.
    /// </summary>
    public sealed class CauseGraph
    {
        private readonly Dictionary<int, Cause> _nodes = new Dictionary<int, Cause>();
        private readonly Stack<Cause> _pending = new Stack<Cause>();

        public int Count => _nodes.Count;

        public bool TryGet(int id, out Cause cause) => _nodes.TryGetValue(id, out cause);

        /// <summary>Registers the cause and all of its ancestors.</summary>
        public void Register(Cause cause)
        {
            _pending.Push(cause);
            while (_pending.Count > 0)
            {
                Cause node = _pending.Pop();
                if (!_nodes.TryAdd(node.Id, node))
                {
                    continue;
                }

                for (int i = 0; i < node.Parents.Count; i++)
                {
                    _pending.Push(node.Parents[i]);
                }
            }
        }

        /// <summary>Fills <paramref name="shares"/> (indexed by ActorId) with each root actor's share; sum = 1.</summary>
        public void CollectRootShares(Cause cause, float[] shares)
        {
            for (int i = 0; i < shares.Length; i++)
            {
                shares[i] = 0f;
            }

            Walk(cause, 1f, shares);
        }

        private static void Walk(Cause node, float weight, float[] shares)
        {
            int parents = node.Parents.Count;
            if (parents == 0)
            {
                shares[(int)node.Instigator] += weight;
                return;
            }

            float part = weight / parents;
            for (int i = 0; i < parents; i++)
            {
                Walk(node.Parents[i], part, shares);
            }
        }
    }
}
