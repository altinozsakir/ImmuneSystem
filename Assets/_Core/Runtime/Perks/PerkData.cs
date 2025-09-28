using System;
using UnityEngine;


namespace Core.Perks
{
    public enum PerkRarity { Common, Uncommon, Rare }


    [Serializable]
    public struct StatDelta
    {
        [Header("Combat")]
        [Tooltip("+0.10 = +10% outgoing damage (multiplicative)")] public float damageMultAdd;
        [Tooltip("+0.02 = +2% absolute execute threshold")] public float executeBonusAdd;


        [Header("Economy/World")]
        [Tooltip("×1.10 = +10% ATP tick")] public float atpIncomeMultMul;
        [Tooltip("×0.95 = enemies 5% slower globally")] public float enemySpeedMultMul;
        [Tooltip("×0.90 = repairs 10% cheaper")] public float repairCostMultMul;


        [Header("One-time rewards")]
        public int addCytokinesOnPick;
    }


    [CreateAssetMenu(menuName = "ImmuneTD/PerkData")]
    public class PerkData : ScriptableObject
    {
        public string id;
        public string title;
        [TextArea] public string description;
        public PerkRarity rarity = PerkRarity.Common;
        [Min(0)] public int baseCytokineCost = 2;
        [Min(1)] public int durationEvents = 2; // number of BodyClock phase changes to persist
        public StatDelta[] effects;
    }
}