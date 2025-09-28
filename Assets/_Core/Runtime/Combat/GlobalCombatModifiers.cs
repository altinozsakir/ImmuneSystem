using UnityEngine;
using Core.Meta;
namespace Core.Combat
{
    public static class GlobalCombatModifiers
    {
        // FROM Inflammation
        static float _inflDamageMult = 1f; public static float ExecuteBonusInfl { get; private set; } = 0f;
        // FROM Perks (multiplicative where sensible)
        public static float DamageMultPerk { get; private set; } = 1f;
        public static float ExecuteBonusPerk { get; private set; } = 0f;
        public static float EnemySpeedMultPerk { get; private set; } = 1f;
        public static float AtpIncomeMultPerk { get; private set; } = 1f;
        public static float RepairCostMultPerk { get; private set; } = 1f;

        static Core.Meta.InflammationMeter _bound;
        static System.Action<int> _handler;
        // Totals used by gameplay
        public static float DamageMult => _inflDamageMult * DamageMultPerk;
        public static float ExecuteBonus => ExecuteBonusInfl + ExecuteBonusPerk;


        public static void Bind(Core.Meta.InflammationMeter infl)
        {
            if (!infl) return;

            // idempotent: unbind old
            if (_bound != null && _handler != null) _bound.OnChanged -= _handler;

            _bound = infl;
            _handler = _ =>
            {
                _inflDamageMult = infl.DamageBonusMult; // e.g., 1.00 → 1.15
                ExecuteBonusInfl = infl.ExecuteBonus;   // e.g., +0.00 → +0.03
            };
            infl.OnChanged += _handler;

            // initialize now
            _inflDamageMult = infl.DamageBonusMult;
            ExecuteBonusInfl = infl.ExecuteBonus;
        }


        // Called by PerkManager when totals change
        public static void SetPerkTotals(float dmgMult, float execAdd, float enemySpeedMult, float atpMult, float repairCostMult)
        {
            DamageMultPerk = Mathf.Max(0f, dmgMult);
            ExecuteBonusPerk = Mathf.Max(0f, execAdd);
            EnemySpeedMultPerk = Mathf.Max(0f, enemySpeedMult);
            AtpIncomeMultPerk = Mathf.Max(0f, atpMult);
            RepairCostMultPerk = Mathf.Max(0f, repairCostMult);
        }
    }
}