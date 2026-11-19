using UnityEngine;

namespace FB.Game.Civilian
{
    /// <summary>
    /// Procedural cartoon animation: swinging limbs scaled by speed, arms up and flailing in panic,
    /// screaming mouth, bobbing head, falling flat when knocked down,
    /// and twitching all over while electrocuted. No animator controller needed.
    /// </summary>
    public sealed class CivilianAnimator
    {
        private const float FallAngle = 80f;
        private const float FallLift = 0.1f;

        private float _phase;
        private float _fall;
        private float _panic;
        private float _shock;

        public void Update(CivilianView view, float speed, bool panicking, bool knocked, bool shocked,
            float deltaTime)
        {
            _shock = Mathf.MoveTowards(_shock, shocked ? 1f : 0f, deltaTime * 8f);
            _phase += deltaTime * (4f + speed * 4.5f);
            _panic = Mathf.MoveTowards(_panic, panicking ? 1f : 0f, deltaTime * 4f);
            _fall = Mathf.MoveTowards(_fall, knocked ? 1f : 0f, deltaTime * (knocked ? 5f : 1.5f));

            float stride = Mathf.Clamp01(speed / 3f) * 45f * (1f - _fall);
            float swing = Mathf.Sin(_phase);
            view.LegLeft.localRotation = Quaternion.Euler(swing * stride + Twitch(30f), 0f, Twitch(15f));
            view.LegRight.localRotation = Quaternion.Euler(-swing * stride + Twitch(30f), 0f, Twitch(15f));

            // Normal walk: arms swing opposite to legs. Panic: arms up, flailing.
            float flail = Mathf.Sin(_phase * 2.3f) * 35f;
            Quaternion walkLeft = Quaternion.Euler(-swing * stride * 0.8f, 0f, -8f);
            Quaternion walkRight = Quaternion.Euler(swing * stride * 0.8f, 0f, 8f);
            Quaternion panicLeft = Quaternion.Euler(-160f + flail + Twitch(40f), 0f, -25f + Twitch(30f));
            Quaternion panicRight = Quaternion.Euler(-160f - flail + Twitch(40f), 0f, 25f + Twitch(30f));
            view.ArmLeft.localRotation = Quaternion.Slerp(walkLeft, panicLeft, _panic);
            view.ArmRight.localRotation = Quaternion.Slerp(walkRight, panicRight, _panic);

            float bob = Mathf.Abs(Mathf.Sin(_phase)) * 0.05f * Mathf.Clamp01(speed);
            view.Head.localRotation = Quaternion.Euler(Mathf.Sin(_phase * 0.5f) * 6f * _panic, 0f,
                Mathf.Sin(_phase) * 4f + Twitch(25f));
            view.Mouth.localScale = new Vector3(1f, Mathf.Lerp(0.3f, 1.6f, _panic), 1f);

            // Knocked down: the body tips backwards around the heels and is lifted a little, so the big head
            // rests on the floor (head radius ~0.3 m at ~1.35 m from the heels) instead of sinking into it.
            view.Body.localPosition = new Vector3(0f, bob + _fall * FallLift + Mathf.Abs(Twitch(0.06f)), 0f);
            view.Body.localRotation = Quaternion.Euler(-FallAngle * _fall, 0f, 0f);
        }

        /// <summary>Random per-frame jolt while the current flows: the cartoon electrocution shake.</summary>
        private float Twitch(float amplitude) => _shock > 0f ? Random.Range(-amplitude, amplitude) * _shock : 0f;
    }
}
