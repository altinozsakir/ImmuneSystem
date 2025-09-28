using System;
using UnityEngine;
using Core.TimeSystem;


namespace Core.Economy
{

    [DefaultExecutionOrder(-500)]
    public class ResourceBank : MonoBehaviour
    {
        public BodyClockDirector clock; // phase multipliers
        [Header("Amounts")]
        public float atp = 0f;
        public int cytokines = 0;
        public delegate void ExternalMultiplierRequestHandler(ref float multiplier);
        public event ExternalMultiplierRequestHandler OnExternalMultiplierRequest;

        [Header("ATP Tick")]
        [Min(0f)] public float atpPerTick = 15f; // base value per tick
        [Min(0.1f)] public float tickIntervalSec = 6f; // unscaled seconds


        public event Action OnChanged; // HUD can subscribe


        float _tickTimer; // unscaled seconds accumulator
        float _atpMult = 1f;


        void Awake()
        {
            if (!clock) clock = FindAnyObjectByType<BodyClockDirector>();
        }
        void OnMultChanged(Core.TimeSystem.PhaseMultipliers m)
        {
            _atpMult = Mathf.Max(0f, m.atpIncome);
        }

        public bool SpendCytokines(int amount)
        {
        if (cytokines < amount) return false;
        cytokines -= amount; OnChanged?.Invoke(); return true;
        }
        void OnEnable()
        {
            if (clock)
            {
                clock.OnMultipliersChanged += OnMultChanged;
                _atpMult = clock.Multipliers.atpIncome;
            }
        }
        void OnDisable() { if (clock) clock.OnMultipliersChanged -= OnMultChanged; }
        void Update()
        {
            // Tick on unscaled time so planning slow-time doesn't delay income pacing
            _tickTimer += Time.unscaledDeltaTime;
            if (_tickTimer >= tickIntervalSec)
            {
                _tickTimer -= tickIntervalSec;
                float m = Mathf.Max(0f, _atpMult);
                OnExternalMultiplierRequest?.Invoke(ref m);
                GainATP(atpPerTick * m);
            }
        }


        public void GainATP(float amount)
        {
            atp += Mathf.Max(0f, amount);
            OnChanged?.Invoke();
        }
        public bool SpendATP(float amount)
        {
            if (atp < amount) return false;
            atp -= amount; OnChanged?.Invoke(); return true;
        }


        public void AddCytokines(int amount) { cytokines += Mathf.Max(0, amount); OnChanged?.Invoke(); }
        
        
        
    }
}