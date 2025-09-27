using System.Collections.Generic;
using UnityEngine;


namespace Core.Combat
{
    /// Aggregates slows and snares on a target. Separate numbers from VFX/SFX.
    public class CrowdControl : MonoBehaviour
    {
        [Header("Resistance & Caps")]
        [Range(0f, 0.9f)] public float slowResist = 0f; // 0.25 = reduces slow by 25%
        [Range(0.5f, 1f)] public float minSpeedFromSlows = 0.30f; // cap = −70% after resist


        readonly List<SlowEntry> _slows = new();
        readonly List<SnareEntry> _snares = new();


        struct SlowEntry { public float mag; public float t; }
        struct SnareEntry { public float t; }


        public bool HasSnare { get; private set; }
        public float SpeedMultiplier { get; private set; } = 1f; // 0..1


        void Update()
        {
            float dt = Time.deltaTime;
            HasSnare = false;


            // decay snares
            for (int i = _snares.Count - 1; i >= 0; i--)
            {
                var e = _snares[i]; e.t -= dt; if (e.t <= 0f) _snares.RemoveAt(i); else _snares[i] = e;
            }
            if (_snares.Count > 0) HasSnare = true;


            // decay slows
            float sum = 0f;
            for (int i = _slows.Count - 1; i >= 0; i--)
            {
                var e = _slows[i]; e.t -= dt; if (e.t <= 0f) { _slows.RemoveAt(i); continue; }
                _slows[i] = e;
                sum += Mathf.Max(0f, e.mag);
            }


            if (HasSnare)
            {
                SpeedMultiplier = 0f;
            }
            else
            {
                // apply resist to total slow then clamp to cap
                float effectiveSlow = sum * (1f - Mathf.Clamp01(slowResist));
                float m = 1f - effectiveSlow; // 1 = full speed
                float minM = Mathf.Clamp01(minSpeedFromSlows);
                SpeedMultiplier = Mathf.Max(minM, m);
            }
        }
        public void AddSlow(float magnitude, float duration)
        {
            if (magnitude <= 0f || duration <= 0f) return;
            _slows.Add(new SlowEntry { mag = Mathf.Clamp01(magnitude), t = duration });
        }
        public void AddSnare(float duration)
        {
            if (duration <= 0f) return;
            _snares.Add(new SnareEntry { t = duration });
        }


        public void ClearAll()
        {
            _slows.Clear(); _snares.Clear(); HasSnare = false; SpeedMultiplier = 1f;
        }
    }
}