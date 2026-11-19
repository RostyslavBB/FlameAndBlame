using UnityEngine;

namespace FB.Game.Civilian
{
    /// <summary>
    /// Procedural cartoon animation: swinging limbs scaled by speed, arms up and flailing in panic,
    /// screaming mouth, bobbing head, and falling flat when knocked down. No animator controller needed.
    /// </summary>
    public sealed class CivilianAnimator
    {
        private float _phase;
        private float _fall;
        private float _panic;

        public void Update(CivilianView view, float speed, bool panicking, bool knocked, float deltaTime)
        {
            _phase += deltaTime * (4f + speed * 4.5f);
            _panic = Mathf.MoveTowards(_panic, panicking ? 1f : 0f, deltaTime * 4f);
            _fall = Mathf.MoveTowards(_fall, knocked ? 1f : 0f, deltaTime * (knocked ? 5f : 1.5f));

            float stride = Mathf.Clamp01(speed / 3f) * 45f * (1f - _fall);
            float swing = Mathf.Sin(_phase);
            view.LegLeft.localRotation = Quaternion.Euler(swing * stride, 0f, 0f);
            view.LegRight.localRotation = Quaternion.Euler(-swing * stride, 0f, 0f);

            // Normal walk: arms swing opposite to legs. Panic: arms up, flailing.
            float flail = Mathf.Sin(_phase * 2.3f) * 35f;
            Quaternion walkLeft = Quaternion.Euler(-swing * stride * 0.8f, 0f, -8f);
            Quaternion walkRight = Quaternion.Euler(swing * stride * 0.8f, 0f, 8f);
            Quaternion panicLeft = Quaternion.Euler(-160f + flail, 0f, -25f);
            Quaternion panicRight = Quaternion.Euler(-160f - flail, 0f, 25f);
            view.ArmLeft.localRotation = Quaternion.Slerp(walkLeft, panicLeft, _panic);
            view.ArmRight.localRotation = Quaternion.Slerp(walkRight, panicRight, _panic);

            float bob = Mathf.Abs(Mathf.Sin(_phase)) * 0.05f * Mathf.Clamp01(speed);
            view.Head.localRotation = Quaternion.Euler(Mathf.Sin(_phase * 0.5f) * 6f * _panic, 0f,
                Mathf.Sin(_phase) * 4f);
            view.Mouth.localScale = new Vector3(1f, Mathf.Lerp(0.3f, 1.6f, _panic), 1f);

            // Knocked down: the whole body tips backwards onto the floor.
            view.Body.localPosition = new Vector3(0f, bob - _fall * 0.35f, 0f);
            view.Body.localRotation = Quaternion.Euler(-85f * _fall, 0f, 0f);
        }
    }
}
