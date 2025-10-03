using UnityEngine;
using Core.Pathing; // NavAgentMover

namespace Core.Combat
{
    /// Aggregates slows and snares, applies to NavAgentMover.
    public class CrowdControl : MonoBehaviour
    {
        [Header("Resist & Caps")]
        [Tooltip("Fraction of slow resisted. 0.15 = 15% less effective.")]
        [Range(0f, 0.9f)] public float slowResist = 0.15f;
        [Tooltip("Clamp final speed to at least this fraction of base after all slows & resist.")]
        [Range(0f, 1f)] public float minSpeedFromSlows = 0.30f;

        [Header("Snare")]
        [Tooltip("If snared, we pause the mover (agent.isStopped = true).")]
        public bool allowSnare = true;

        // runtime
        float _slowSum;          // summed slow magnitudes before resist (e.g., 0.4 + 0.25)
        float _slowTimer;        // remaining time for the current slow window (seconds)
        float _snareTimer;       // remaining snare time (seconds)

        NavAgentMover _mover;

        void Awake()
        {
            _mover = GetComponent<NavAgentMover>();
        }

        void Update()
        {
            float dt = Time.deltaTime;

            // decay timers
            if (_slowTimer > 0f) _slowTimer -= dt; else _slowSum = 0f;
            if (_snareTimer > 0f) _snareTimer -= dt;

            ApplyToMover();
        }

        /// <summary>Add/refresh a slow. slow=0.4 means -40% speed for 'duration' seconds.</summary>
        public void AddSlow(float slow, float duration)
        {
            if (slow <= 0f || duration <= 0f) return;
            _slowSum += Mathf.Clamp01(slow);
            _slowTimer = Mathf.Max(_slowTimer, duration);
            ApplyToMover();
        }

        /// <summary>Apply/refresh a snare (root). While active, agent is paused.</summary>
        public void AddSnare(float duration)
        {
            if (!allowSnare || duration <= 0f) return;
            _snareTimer = Mathf.Max(_snareTimer, duration);
            ApplyToMover();
        }

        void ApplyToMover()
        {
            if (!_mover) return;

            // Snare: hard stop while active
            bool snared = _snareTimer > 0f;
            _mover.SetPaused(snared);

            // Slows: convert summed slow to multiplier with resist & cap
            float slowBefore = Mathf.Max(0f, _slowSum);
            float effectiveSlow = slowBefore * (1f - slowResist);
            float speedMult = Mathf.Clamp01(1f - effectiveSlow);
            speedMult = Mathf.Max(speedMult, minSpeedFromSlows);

            // If snared and you want total stop, you can force 0.0 here.
            // We'll let pause handle stopping, but set mult anyway for displays.
            _mover.SetSpeedMultiplier(speedMult);
        }
    }
}
