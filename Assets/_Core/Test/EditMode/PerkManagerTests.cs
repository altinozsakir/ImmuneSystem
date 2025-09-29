#if UNITY_EDITOR
using NUnit.Framework;
using UnityEngine;
using Core.Perks;
using Core.TimeSystem;
using Core.Economy;
using Core.Combat;

public class PerkManagerTests
{
    PerkManager mgr;
    BodyClockDirector clock;
    ResourceBank bank;

    // Helper: make a PerkData in-memory
    static PerkData MakePerk(string id, float dmgAdd=0f, float execAdd=0f, float atpMul=1f, float spdMul=1f, float repMul=1f, int cost=1, int duration=2)
    {
        var p = ScriptableObject.CreateInstance<PerkData>();
        p.id = id; p.baseCytokineCost = cost; p.durationEvents = duration;
        p.effects = new StatDelta[]{ new StatDelta {
            damageMultAdd = dmgAdd, executeBonusAdd = execAdd,
            atpIncomeMultMul = atpMul, enemySpeedMultMul = spdMul, repairCostMultMul = repMul
        }};
        return p;
    }

    [SetUp]
    public void Setup()
    {
        var go = new GameObject("Systems");
        clock = go.AddComponent<BodyClockDirector>();
        bank = go.AddComponent<ResourceBank>();
        mgr = go.AddComponent<PerkManager>();
        mgr.clock = clock; mgr.bank = bank;

        // seed bank with cytokines so picks succeed
        bank.cytokines = 999;

        // ensure subscriptions
        mgr.enabled = true;
    }

    [TearDown] public void Teardown(){ Object.DestroyImmediate(mgr.gameObject); }

    [Test]
    public void CostEscalation_Works()
    {
        var p = MakePerk("A", dmgAdd:0.1f, cost:2);
        Assert.AreEqual(2, mgr.EscalatedCost(p));       // 0 picks
        mgr.TryTake(p);
        Assert.AreEqual(Mathf.CeilToInt(2 * 1.25f), mgr.EscalatedCost(p)); // +25%
        mgr.TryTake(p);
        Assert.AreEqual(Mathf.CeilToInt(2 * (1 + 2*0.25f)), mgr.EscalatedCost(p)); // +50%
    }

    [Test]
    public void Deltas_Compose_SumAndProduct()
    {
        var dmgA = MakePerk("dmg+10", dmgAdd:0.10f);
        var dmgB = MakePerk("dmg+05", dmgAdd:0.05f);
        var spd  = MakePerk("spd95", spdMul:0.95f);

        mgr.TryTake(dmgA);
        mgr.TryTake(dmgB);
        mgr.TryTake(spd);

        // RecomputeTotals already called; check GlobalCombatModifiers
        Assert.That(GlobalCombatModifiers.DamageMult, Is.EqualTo(1f + 0.10f + 0.05f).Within(0.0001f)); // additive
        Assert.That(GlobalCombatModifiers.EnemySpeedMultPerk, Is.EqualTo(0.95f).Within(0.0001f));      // multiplicative
    }

    [Test]
    public void Duration_Expires_After_Events()
    {
        var p = MakePerk("short", dmgAdd:0.20f, duration:2);
        mgr.TryTake(p);

        float start = GlobalCombatModifiers.DamageMult;
        Assert.Greater(start, 1f);

        // Simulate two phase changes
        mgr.SendMessage("OnPhaseChanged", BodyPhase.Morning); // decrement 1
        Assert.Greater(GlobalCombatModifiers.DamageMult, 1f);
        mgr.SendMessage("OnPhaseChanged", BodyPhase.Evening); // decrement 2 => expire

        Assert.That(GlobalCombatModifiers.DamageMult, Is.EqualTo(1f).Within(0.0001f));
    }
}
#endif
