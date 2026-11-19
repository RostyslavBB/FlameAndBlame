using UnityEngine;

namespace FB.Game.Hose
{
    /// <summary>
    /// Initial rope shape at full length: tap → down to the floor → spiral coil → up to the nozzle.
    /// Starting uncompressed avoids the accordion zig-zag a straight, too-short line would collapse into.
    /// </summary>
    public static class HoseRopeLayout
    {
        private const float CoilOffset = 0.7f;
        private const float InnerRadius = 0.25f;
        private const float OuterRadius = 0.55f;

        public static void Coil(Vector3[] points, Vector3 anchor, Vector3 end, float segment, float floorY)
        {
            int count = points.Length;
            Vector3 toEnd = end - anchor;
            toEnd.y = 0f;
            Vector3 direction = toEnd.sqrMagnitude > 0.01f ? toEnd.normalized : Vector3.forward;
            Vector3 floorUnderAnchor = new Vector3(anchor.x, floorY, anchor.z) + direction * 0.15f;
            Vector3 center = new Vector3(anchor.x, floorY, anchor.z) + direction * CoilOffset;

            int drop = Mathf.Clamp(Mathf.CeilToInt((anchor.y - floorY) / segment), 1, count / 4);
            int rise = Mathf.Clamp(Mathf.CeilToInt(Vector3.Distance(center, end) / segment), 1, count / 3);
            int coil = Mathf.Max(0, count - 1 - drop - rise);

            int index = 0;
            points[index++] = anchor;
            for (int k = 1; k <= drop; k++)
            {
                points[index++] = Vector3.Lerp(anchor, floorUnderAnchor, k / (float)drop);
            }

            Vector3 offset = floorUnderAnchor - center;
            float angle = Mathf.Atan2(offset.z, offset.x);
            Vector3 last = floorUnderAnchor;
            for (int k = 0; k < coil; k++)
            {
                float radius = Mathf.Lerp(InnerRadius, OuterRadius, k / (float)Mathf.Max(1, coil - 1));
                angle += segment / radius;
                last = center + new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * radius;
                points[index++] = last;
            }

            for (int k = 1; k <= rise && index < count; k++)
            {
                points[index++] = Vector3.Lerp(last, end, k / (float)rise);
            }

            while (index < count)
            {
                points[index++] = end;
            }
        }
    }
}
