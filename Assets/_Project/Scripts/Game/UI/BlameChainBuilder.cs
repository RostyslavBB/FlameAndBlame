using System.Collections.Generic;
using FB.Game.Blame;
using UnityEngine;

namespace FB.Game.UI
{
    /// <summary>
    /// Turns one blame record into a short, readable chain of cards: from the root culprit, along the causes that
    /// lead to it (repeats merged, e.g. "Вогонь ×3"), to the damaged target and its price. Where the cause graph
    /// branches (puddle + power) it follows the branch of the main culprit, so the chain explains the verdict.
    /// </summary>
    public sealed class BlameChainBuilder
    {
        private static readonly ChainLink Ellipsis = new ChainLink("…", "і так далі", new Color(0.55f, 0.5f, 0.5f));
        private static readonly Color TargetColor = new Color(0.86f, 0.2f, 0.15f);

        private readonly IActorRegistry _actors;
        private readonly List<Cause> _path = new List<Cause>(16);

        public BlameChainBuilder(IActorRegistry actors)
        {
            _actors = actors;
        }

        /// <summary>Fills <paramref name="links"/> (at most <paramref name="maxLinks"/>, target included).</summary>
        public void Build(BlameRecord record, int maxLinks, List<ChainLink> links)
        {
            links.Clear();
            BuildPath(record.Cause, record.MainActor);
            int causeSlots = Mathf.Max(1, maxLinks - 1);
            for (int i = 0; i < _path.Count; i++)
            {
                int repeats = 1;
                while (i + 1 < _path.Count && SameStep(_path[i], _path[i + 1]))
                {
                    repeats++;
                    i++;
                }

                links.Add(Link(_path[i], repeats));
            }

            // Too long: keep the root and the last steps, the middle collapses into "…".
            if (links.Count > causeSlots && causeSlots >= 3)
            {
                int tail = causeSlots - 2;
                links.RemoveRange(1, links.Count - 1 - tail);
                links.Insert(1, Ellipsis);
            }

            links.Add(new ChainLink(record.Target ?? "?", record.Damage > 0 ? "+" + Money.Of(record.Damage) : "постраждав",
                TargetColor));
        }

        private void BuildPath(Cause cause, ActorId mainActor)
        {
            _path.Clear();
            Cause node = cause;
            while (node != null)
            {
                _path.Insert(0, node);
                node = PickParent(node, mainActor);
            }
        }

        private static Cause PickParent(Cause node, ActorId mainActor)
        {
            if (node.Parents.Count == 0)
            {
                return null;
            }

            for (int i = 0; i < node.Parents.Count; i++)
            {
                if (LeadsTo(node.Parents[i], mainActor))
                {
                    return node.Parents[i];
                }
            }

            return node.Parents[0];
        }

        private static bool LeadsTo(Cause node, ActorId root)
        {
            if (node.Parents.Count == 0)
            {
                return node.Instigator == root;
            }

            for (int i = 0; i < node.Parents.Count; i++)
            {
                if (LeadsTo(node.Parents[i], root))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool SameStep(Cause a, Cause b) => a.Instigator == b.Instigator && a.Action == b.Action;

        private ChainLink Link(Cause node, int repeats)
        {
            string title = _actors.NameOf(node.Instigator);
            if (repeats > 1)
            {
                title += " ×" + repeats;
            }

            return new ChainLink(title, Label(node.Action), _actors.ColorOf(node.Instigator));
        }

        private static string Label(CauseAction action)
        {
            switch (action)
            {
                case CauseAction.HoseSpray: return "полив водою";
                case CauseAction.Ignite: return "підпалив";
                case CauseAction.FireSpread: return "перекинувся";
                case CauseAction.Burn: return "обпік";
                case CauseAction.ShortCircuit: return "замикання";
                case CauseAction.Drown: return "залив";
                case CauseAction.Puddle: return "калюжа";
                case CauseAction.FaultyWiring: return "іскрить";
                case CauseAction.Electrocution: return "удар струмом";
                case CauseAction.KnockOver: return "перекинув";
                case CauseAction.Soak: return "облив";
                default: return action.ToString();
            }
        }
    }
}
