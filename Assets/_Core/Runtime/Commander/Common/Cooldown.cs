using UnityEngine;


namespace Core.Commander.Common
{
    public struct Cooldown
    {
        [Min(0f)] public float Duration;
        private float readyAt;
        public bool IsReady(float now) => now >= readyAt;

        public void Arm(float now) => readyAt = now + Mathf.Max(0f, Duration);
        public void ForceReady(float now) => readyAt = now;

        public void ForceNotReady(float now) => readyAt = now + Mathf.Max(0f,Duration);

        public float Remainig(float now) => Mathf.Max(0f, readyAt - now);

    }
}
