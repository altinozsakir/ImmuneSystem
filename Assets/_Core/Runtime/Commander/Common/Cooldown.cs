using UnityEngine;


namespace Core.Commander.Common
{
    public struct Cooldown
    {
        [SerializeField] private float duration;
        private float readyAt;
        public float Duration => duration;
        public bool IsReady(float now) => now >= readyAt;

        public void Arm(float now) => readyAt = now + Mathf.Max(0f, duration);
        public void ForceReady(float now) => readyAt = now;

        public float Remainig(float now) => Mathf.Max(0f, readyAt - now);

    }
}
