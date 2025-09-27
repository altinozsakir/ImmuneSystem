using System;
using UnityEngine;


namespace Core.TimeSystem
{
    public enum BodyPhase { Sleep, Morning, PostMeal, Evening }


    [Serializable]
    public struct PhaseDef
    {
        public BodyPhase phase;
        [Min(1f)] public float durationSec;
        public Color phaseColor;
        [Header("Multipliers (1 = neutral)")]
        [Min(0f)] public float atpIncomeMult; // e.g., Morning 1.2
        [Min(0f)] public float enemySpeedMult; // e.g., Evening 1.1
        [Min(0f)] public float repairCostMult; // e.g., 0.8 means 20% cheaper repairs
        [Header("Planning Window on Phase Enter")]
        public bool startPlanningOnEnter; // Sleep & PostMeal
        [Range(2f, 20f)] public float planningDurationSec; // 6–8s
    }


    [CreateAssetMenu(menuName = "ImmuneTD/BodyClockConfig")]
    public class BodyClockConfig : ScriptableObject
    {
        public PhaseDef[] phases = new PhaseDef[] {
new PhaseDef{ phase=BodyPhase.Sleep, durationSec=30, phaseColor=new Color(0.25f,0.35f,0.8f), atpIncomeMult=0.9f, enemySpeedMult=0.9f, repairCostMult=0.9f, startPlanningOnEnter=true, planningDurationSec=7},
new PhaseDef{ phase=BodyPhase.Morning, durationSec=40, phaseColor=new Color(0.35f,0.8f,0.5f), atpIncomeMult=1.2f, enemySpeedMult=1.0f, repairCostMult=1.0f, startPlanningOnEnter=false, planningDurationSec=0},
new PhaseDef{ phase=BodyPhase.PostMeal, durationSec=30, phaseColor=new Color(0.95f,0.7f,0.2f), atpIncomeMult=1.15f, enemySpeedMult=1.05f, repairCostMult=0.95f, startPlanningOnEnter=true, planningDurationSec=7},
new PhaseDef{ phase=BodyPhase.Evening, durationSec=40, phaseColor=new Color(0.9f,0.4f,0.35f), atpIncomeMult=1.0f, enemySpeedMult=1.1f, repairCostMult=1.0f, startPlanningOnEnter=false, planningDurationSec=0},
};


        public int IndexOf(BodyPhase p)
        {
            for (int i = 0; i < phases.Length; i++) if (phases[i].phase == p) return i;
            return -1;
        }
    }
}