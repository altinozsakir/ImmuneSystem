using UnityEngine;
using Core.Combat; // CrowdControl, StatusEffects
using Core.Meta;   // InflammationMeter

namespace Core.Towers
{
    /// Periodic degranulation pulse:
    /// - Applies an AoE slow to enemies in radius
    /// - Optionally adds Mark stacks
    /// - Optionally bumps Inflammation based on enemies hit (capped per pulse)
    public class MastTrapPulse : MonoBehaviour
    {
        [Header("Pulse Timing")]
        [Tooltip("Seconds between pulses.")]
        [Min(0.2f)] public float period = 8.0f;
        [Tooltip("Fire once immediately on enable.")]
        public bool pulseOnEnable = false;

        [Header("Area")]
        [Min(0f)] public float radius = 4.5f;
        [Tooltip("Which layers count as enemies. Set to your Enemy layer.")]
        public LayerMask enemyMask = ~0;
        [Tooltip("Max colliders processed per pulse (perf guard).")]
        [Min(1)] public int maxHits = 32;

        [Header("Slow")]
        [Tooltip("0.40 = 40% slow. Slows are aggregated in CrowdControl and capped after resist.")]
        [Range(0f, 1f)] public float slowMagnitude = 0.40f;
        [Tooltip("Duration in seconds for the slow.")]
        [Min(0.05f)] public float slowDuration = 3.0f;

        [Header("Degranulate (Mark)")]
        public bool applyMark = true;
        [Min(0)] public int markStacks = 1;

        [Header("Inflammation (optional)")]
        public InflammationMeter inflammation;
        [Min(0)] public int inflPerEnemyHit = 1;
        [Min(0)] public int inflMaxPerPulse = 3;

        [Header("FX (optional)")]
        public GameObject pulseVFX;
        public AudioClip pulseSFX;

        float _timer;
        Collider[] _buf;

        void Awake()
        {
            _buf = new Collider[Mathf.Max(4, maxHits)];
            if (!inflammation) inflammation = FindAnyObjectByType<InflammationMeter>();
        }

        void OnEnable()
        {
            _timer = pulseOnEnable ? 0f : period;
        }

        void Update()
        {
            _timer -= Time.deltaTime;
            if (_timer <= 0f)
            {
                _timer = Mathf.Max(0.01f, period);
                DoPulse();
            }
        }

        /// <summary>Manually trigger a pulse (for tests or events).</summary>
        public void ForcePulse()
        {
            DoPulse();
            _timer = Mathf.Max(0.01f, period);
        }

        void DoPulse()
        {
            if (pulseVFX) Instantiate(pulseVFX, transform.position, Quaternion.identity);
            if (pulseSFX) AudioSource.PlayClipAtPoint(pulseSFX, transform.position);

            int hits = Physics.OverlapSphereNonAlloc(
                transform.position, radius, _buf, enemyMask, QueryTriggerInteraction.Ignore);

            int inflGained = 0;

            for (int i = 0; i < hits && i < _buf.Length; i++)
            {
                var col = _buf[i];
                if (!col) continue;

                // Get the unit Transform (prefer Rigidbody root if present)
                var tr = col.attachedRigidbody ? col.attachedRigidbody.transform : col.transform;
                if (!tr || !tr.gameObject.activeInHierarchy) continue;

                // Apply slow via CrowdControl
                if (tr.TryGetComponent<CrowdControl>(out var cc))
                    cc.AddSlow(slowMagnitude, slowDuration);

                // Apply Mark stacks
                if (applyMark && markStacks > 0 && tr.TryGetComponent<StatusEffects>(out var st))
                    st.AddMarks(markStacks);

                // Count for Inflammation (capped)
                if (inflPerEnemyHit > 0 && inflGained < inflMaxPerPulse)
                    inflGained += inflPerEnemyHit;
            }

            // Raise Inflammation once per pulse (capped)
            if (inflammation && inflGained > 0)
                inflammation.Add(Mathf.Min(inflGained, inflMaxPerPulse));
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(0.8f, 0.6f, 1f, 0.5f);
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }
}
