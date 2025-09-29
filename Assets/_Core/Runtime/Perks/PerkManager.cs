using System.Collections.Generic;
using UnityEngine;
using Core.TimeSystem;
using Core.Economy;
using Core.Combat;


namespace Core.Perks
{
    public class PerkManager : MonoBehaviour
    {
        [Header("Refs")]
public BodyClockDirector clock; public ResourceBank bank;
        [Header("Library")] public List<PerkData> allPerks;
        [Header("Picker Rules")] public float costEscalationPerPick = 0.25f; // +25% each pick
        public int choicesPerPick = 2;


        // Reroll budget (from Dendritic Outpost). UI reads/consumes this.
        public int rerollCharges { get; private set; }


        // Internal state
        class ActivePerk { public PerkData data; public int eventsLeft; public StatDelta[] deltas; }
        readonly List<ActivePerk> _active = new();
        int _pickCount = 0; // for cost escalation


        // Aggregated perk totals
        float _perkDmgMult = 1f, _perkAtpMult = 1f, _perkEnemySpeedMult = 1f, _perkRepairCostMult = 1f, _perkExecAdd = 0f;


        void Awake()
        {
            if (!clock) clock = FindAnyObjectByType<BodyClockDirector>();
            if (!bank) bank = FindAnyObjectByType<ResourceBank>();
        }
        void OnEnable() { if (clock) clock.OnPhaseChanged += OnPhaseChanged; if (bank) bank.OnExternalMultiplierRequest += OnBankMult; }
        void OnDisable() { if (clock) clock.OnPhaseChanged -= OnPhaseChanged; if (bank) bank.OnExternalMultiplierRequest -= OnBankMult; }


        void OnBankMult(ref float mult) { mult *= Mathf.Max(0.0f, _perkAtpMult); }


        void OnPhaseChanged(BodyPhase p)
        {
            Debug.Log($"[PerkManager] Phase changed → {p}");
            // Sleep → open picker
            if (p == BodyPhase.Sleep)
                PerkPickerUI.TryOpen(this);


            // decrement durations (on any phase change after pick)
            for (int i = _active.Count - 1; i >= 0; i--)
            {
                _active[i].eventsLeft--;
                if (_active[i].eventsLeft <= 0)
                {
                    _active.RemoveAt(i);
                    RecomputeTotals();
                }
            }
        }


        public void AddRerollCharges(int amount)
        {
            rerollCharges = Mathf.Clamp(rerollCharges + Mathf.Max(0, amount), 0, 99);
        }
        public void ConsumeReroll() { if (rerollCharges > 0) rerollCharges--; }


        public (PerkData, int)[] GenerateChoices(int qualityBias)
        {
            // Weighted draw with rarity bias
            var res = new (PerkData, int)[choicesPerPick];
            for (int i = 0; i < choicesPerPick; i++)
            {
                var perk = DrawOne(qualityBias);
                res[i] = (perk, EscalatedCost(perk));
            }
            return res;
        }


        PerkData DrawOne(int bias)
        {
            // base weights
            float wC = 70, wU = 25, wR = 5;
            // bias pushes towards higher rarity
            wU += 10 * bias; wR += 10 * bias;
            float total = wC + wU + wR; float roll = Random.Range(0f, total);


            PerkRarity want = (roll < wC) ? PerkRarity.Common : (roll < wC + wU ? PerkRarity.Uncommon : PerkRarity.Rare);
            // pick a random perk of that rarity (fallback if none)
            var pool = allPerks.FindAll(p => p && p.rarity == want);
            if (pool.Count == 0) pool = allPerks;
            return pool[Random.Range(0, Mathf.Max(1, pool.Count))];
        }


        public int EscalatedCost(PerkData p)
        {
            float mult = 1f + _pickCount * costEscalationPerPick;
            return Mathf.CeilToInt(p.baseCytokineCost * mult);
        }


        public bool TryTake(PerkData perk)
        {
            int cost = EscalatedCost(perk);
            if (!bank || !bank.SpendCytokines(cost)) return false;


            if (perk.effects != null)
            {
                var ap = new ActivePerk { data = perk, eventsLeft = Mathf.Max(1, perk.durationEvents), deltas = perk.effects };
                _active.Add(ap);
                ApplyOneTime(perk.effects);
                RecomputeTotals();
            }


            _pickCount++;
            return true;
        }
        void ApplyOneTime(StatDelta[] effects)
        {
            int addCK = 0; foreach (var d in effects) addCK += Mathf.Max(0, d.addCytokinesOnPick);
            if (bank && addCK > 0) bank.AddCytokines(addCK);
        }


        void RecomputeTotals()
        {
            float dmgAdd = 0f, execAdd = 0f; // damage = 1 + add; exec = +abs
            float atpMul = 1f, spdMul = 1f, repMul = 1f;
            foreach (var a in _active)
            {
                foreach (var d in a.deltas)
                {
                    dmgAdd += Mathf.Max(0f, d.damageMultAdd);
                    execAdd += Mathf.Max(0f, d.executeBonusAdd);
                    atpMul *= (d.atpIncomeMultMul <= 0f ? 1f : d.atpIncomeMultMul);
                    spdMul *= (d.enemySpeedMultMul <= 0f ? 1f : d.enemySpeedMultMul);
                    repMul *= (d.repairCostMultMul <= 0f ? 1f : d.repairCostMultMul);
                }
            }
            _perkDmgMult = 1f + dmgAdd; _perkExecAdd = execAdd;
            _perkAtpMult = atpMul; _perkEnemySpeedMult = spdMul; _perkRepairCostMult = repMul;
            GlobalCombatModifiers.SetPerkTotals(_perkDmgMult, _perkExecAdd, _perkEnemySpeedMult, _perkAtpMult, _perkRepairCostMult);
        }
    }
}