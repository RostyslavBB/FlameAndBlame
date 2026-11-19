using System.Collections.Generic;
using FB.Game.Blame;
using UnityEditor;
using UnityEngine;
using E = FB.Game.Blame.NarrativeEvent;
using F = FB.Game.Blame.ActorFilter;

namespace FB.Editor.Configs
{
    /// <summary>Default actors and feed lines. Filled only into empty configs, so writers' edits survive Build All.</summary>
    internal static class BlameDefaults
    {
        public static void EnsureActors(ActorRegistryConfig config)
        {
            if (config.Actors.Count > 0)
            {
                return;
            }

            config.EditorSetActors(new[]
            {
                new ActorEntry(ActorId.Player, "Ти", Hex("#FF8C1A")),
                new ActorEntry(ActorId.Building, "Стара проводка", Hex("#9B7FE0")),
                new ActorEntry(ActorId.Fire, "Вогонь", Hex("#E63946"))
            });
            EditorUtility.SetDirty(config);
        }

        public static void EnsureTexts(BlameTextConfig config)
        {
            if (config.Templates.Count > 0)
            {
                return;
            }

            config.EditorSetTemplates(Templates());
            EditorUtility.SetDirty(config);
        }

        private static IEnumerable<BlameTemplate> Templates()
        {
            yield return T(E.FireStart, F.Any, "{A} вирішила, що сьогодні її день. «{I}» палає.");
            yield return T(E.FireStart, F.Any, "{A}: «Я лише трохи іскрю». «{I}» горить.");

            // Item names have different grammatical genders: phrase lines so the verb does not depend on {I}/{S}.
            yield return T(E.FireSpread, F.Any, "Від «{S}» вогонь перескочив на «{I}». Щедро.");
            yield return T(E.FireSpread, F.Any, "Черга погрітись: «{I}».");
            yield return T(E.FireSpread, F.Any, "Вогонь перебрався на «{I}». Без запрошення.");
            yield return T(E.FireSpread, F.Any, "Тепер горить ще й «{I}». «{S}» передає привіт.");

            yield return T(E.Burned, F.Any, "Згоріло дотла: «{I}». Головний підозрюваний: {A}.");
            yield return T(E.Burned, F.Any, "Від «{I}» лишились спогади і вугілля.");
            yield return T(E.Burned, F.Any, "«{I}»: було гарно, стало хрустко.");
            yield return T(E.Burned, F.Player, "Поки ти шукав, куди натиснути, від «{I}» лишився попіл.");

            yield return T(E.ShortCircuit, F.Player, "Ти полив «{I}». Струм оцінив.");
            yield return T(E.ShortCircuit, F.Player, "Водні процедури для «{I}». Посмертні.");
            yield return T(E.ShortCircuit, F.Player, "Ти перевірив: «{I}» і вода не дружать.");
            yield return T(E.ShortCircuit, F.World, "Напруги не витримало: «{I}». Як і всі ми.");

            yield return T(E.Drowned, F.Player, "Пожежу «{I}» пережив. Тебе — ні.");
            yield return T(E.Drowned, F.Player, "Ти полив «{I}». Трохи забагато. Літрів на триста.");
            yield return T(E.Drowned, F.Any, "«{I}» утоплено з особливою старанністю.");

            yield return T(E.ShockPlayer, F.World, "Тебе вдарило струмом. Подяка: {A}.");
            yield return T(E.ShockPlayer, F.Any, "Калюжа + розетка = ти танцюєш.");
            yield return T(E.ShockPlayer, F.Any, "Струм оцінив твої мокрі черевики.");
            yield return T(E.ShockPlayer, F.Any, "Мокра підлога, живий подовжувач, щасливий ти.");
            yield return T(E.ShockPlayer, F.Player, "Ти налив калюжу і став у неї. Геніально.");
            yield return T(E.ShockPlayer, F.Player, "Ти сам собі електрик. І пацієнт.");

            yield return T(E.ShockObject, F.Any, "Струм підкинув «{I}».");
            yield return T(E.ShockObject, F.Any, "Заряд бадьорості для «{I}».");
            yield return T(E.ShockObject, F.Any, "«{I}» танцює. Музики немає, струм є.");
        }

        private static BlameTemplate T(E narrativeEvent, F filter, string text) =>
            new BlameTemplate(narrativeEvent, filter, text);

        private static Color Hex(string hex)
        {
            ColorUtility.TryParseHtmlString(hex, out Color color);
            return color;
        }
    }
}
