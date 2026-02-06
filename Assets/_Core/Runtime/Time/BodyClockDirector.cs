using System;
using UnityEngine;


namespace Core.TimeSystem
{
    public struct PhaseMultipliers
    {
        public float atpIncome, enemySpeed, repairCost;
    }

    public class BodyClockDirector : MonoBehaviour
    {
        public BodyClockConfig config;
        public BodyPhase startPhase = BodyPhase.Sleep;


        // Events (UI & systems subscribe)
        // Other systems can use this events 
        public event Action<BodyPhase> OnPhaseChanged; // phase label/color
        public event Action<PhaseMultipliers> OnMultipliersChanged; // hooks
        public event Action<bool> OnPlanningWindow; // true = start, false = end


        // State
        public BodyPhase CurrentPhase { get; private set; }
        public float PhaseElapsed { get; private set; } // scaled seconds
        public float PhaseDuration { get; private set; }
        public float PhaseProgress => Mathf.Clamp01(PhaseElapsed / Mathf.Max(0.0001f, PhaseDuration));
        public Color PhaseColor { get; private set; }
        public PhaseMultipliers Multipliers { get; private set; }


        // Planning window (uses unscaled time)
        public bool IsPlanningActive { get; private set; }
        public float PlanningWindowElapsedUnscaled { get; private set; }
        public float PlanningWindowDurationUnscaled { get; private set; }


        IDisposable _tsHandle;


        [Header("Debug Keys")]
        public KeyCode nextPhaseKey = KeyCode.F6;
        public KeyCode prevPhaseKey = KeyCode.F5;

        void Start()
        {
            if (!config || config.phases == null || config.phases.Length == 0)
            { Debug.LogError("BodyClockDirector: Missing config"); enabled = false; return; }
            int idx = config.IndexOf(startPhase); if (idx < 0) idx = 0;
            Enter(config.phases[idx]);
        }
        void Update()
        {
            // Phase timing (scaled)
            PhaseElapsed += Time.deltaTime;
            if (PhaseElapsed >= PhaseDuration)
                Advance();


            // Planning window timing (unscaled)
            if (IsPlanningActive)
            {
                PlanningWindowElapsedUnscaled += Time.unscaledDeltaTime;
                if (PlanningWindowElapsedUnscaled >= PlanningWindowDurationUnscaled)
                    EndPlanningWindow();
            }


            // Debug cycle
            if (Input.GetKeyDown(nextPhaseKey)) Advance();
            if (Input.GetKeyDown(prevPhaseKey)) Advance(-1);
        }
        public void Advance(int step = 1)
        {
            var phases = config.phases; int curr = config.IndexOf(CurrentPhase); if (curr < 0) curr = 0;
            int next = ((curr + step) % phases.Length + phases.Length) % phases.Length;
            Enter(phases[next]);
        }
        void Enter(PhaseDef def)
        {
            if (IsPlanningActive) EndPlanningWindow(); // safety


            CurrentPhase = def.phase;
            PhaseDuration = Mathf.Max(0.0001f, def.durationSec);
            PhaseElapsed = 0f;
            PhaseColor = def.phaseColor;
            Multipliers = new PhaseMultipliers
            {
                atpIncome = Mathf.Max(0f, def.atpIncomeMult),
                enemySpeed = Mathf.Max(0f, def.enemySpeedMult),
                repairCost = Mathf.Clamp(def.repairCostMult, 0f, 10f)
            };
            OnPhaseChanged?.Invoke(CurrentPhase);
            OnMultipliersChanged?.Invoke(Multipliers);


            if (def.startPlanningOnEnter && def.planningDurationSec > 0f)
                BeginPlanningWindow(def.planningDurationSec);
        }
        void BeginPlanningWindow(float durationSec)
        {
            if (IsPlanningActive) EndPlanningWindow();
            IsPlanningActive = true;
            PlanningWindowDurationUnscaled = durationSec;
            PlanningWindowElapsedUnscaled = 0f;
            _tsHandle = TimeScaleService.Push(0f);
            OnPlanningWindow?.Invoke(true);
        }
        void EndPlanningWindow()
        {
            IsPlanningActive = false;
            PlanningWindowElapsedUnscaled = 0f;
            PlanningWindowDurationUnscaled = 0f;
            _tsHandle?.Dispose(); _tsHandle = null;
            OnPlanningWindow?.Invoke(false);
        }
    }
}
