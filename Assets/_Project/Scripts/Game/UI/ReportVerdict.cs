using System.Collections.Generic;
using System.Text;
using FB.Game.Blame;
using UnityEngine;

namespace FB.Game.UI
{
    /// <summary>Model of the report: who is guilty (by blame points) and the most expensive chains.</summary>
    public sealed class ReportVerdict
    {
        private readonly IActorRegistry _actors;

        public ReportVerdict(IActorRegistry actors)
        {
            _actors = actors;
        }

        public VerdictResult Decide(IBlameLedger ledger)
        {
            if (ledger.TotalPoints <= 0f)
            {
                return new VerdictResult("Винних немає. Підозріло.", "ЧИСТО", new Color(0.3f, 0.7f, 0.35f));
            }

            ActorId guilty = ActorId.Player;
            float best = -1f;
            for (int i = 0; i < _actors.Count; i++)
            {
                float points = ledger.PointsOf((ActorId)i);
                if (points > best)
                {
                    best = points;
                    guilty = (ActorId)i;
                }
            }

            int percent = Mathf.RoundToInt(best / ledger.TotalPoints * 100f);
            return new VerdictResult($"Винен: {_actors.NameOf(guilty)} ({percent}%)", "ВИННИЙ",
                _actors.ColorOf(guilty));
        }

        public string TopChains(IBlameLedger ledger, int count)
        {
            List<BlameRecord> sorted = new List<BlameRecord>(ledger.Records);
            sorted.Sort((a, b) => b.Damage.CompareTo(a.Damage));

            StringBuilder builder = new StringBuilder();
            int shown = 0;
            foreach (BlameRecord record in sorted)
            {
                if (shown == count || record.Damage <= 0)
                {
                    break;
                }

                shown++;
                builder.Append(shown).Append(". <b>").Append(Money.Of(record.Damage)).Append("</b>  ")
                    .Append(record.Text).Append('\n')
                    .Append("<size=70%><color=#8A8A8A>").Append(record.Chain).Append("</color></size>\n");
            }

            return shown == 0 ? "Нічого не зіпсовано. Ви точно пожежник?" : builder.ToString();
        }
    }
}
