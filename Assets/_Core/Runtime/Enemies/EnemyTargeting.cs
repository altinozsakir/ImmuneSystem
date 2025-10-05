using System;
using System.Collections.Generic;
using UnityEngine;
using Core.Combat;

namespace Core.Enemies
{
    [RequireComponent(typeof(EnemyPerception))]
    [RequireComponent(typeof(EnemyPerception))]
public class EnemyTargeting : MonoBehaviour
{
    public event Action<IHittable> OnTargetChanged;

    [Header("Retargeting")]
    [SerializeField] private float rethinkInterval = 0.5f;
    [SerializeField] private float stickinessSeconds = 0.8f;   // hysteresis (prevents thrash)
    [SerializeField] private float minScoreDeltaToSwitch = 0.35f;

    public bool debugLogs = true; 

    private EnemyPerception perception;
    private float nextThink, stickUntil;
    private IThreatSource currentThreat;
    public IThreatSource CurrentThreat => currentThreat;

    // weights (same as before; keep your scoring)
    [SerializeField] private float wStatic = 1.0f, wDistance = 1.0f, wClassBias = 1.0f, wAggro = 1.5f;
    private readonly Dictionary<IThreatSource, float> aggro = new();

    void Awake() => perception = GetComponent<EnemyPerception>();

    void Update()
    {
        if (Time.time < nextThink) return;
        nextThink = Time.time + rethinkInterval;

        perception.PruneDead();
        var best = PickBest(perception.Seen.ConvertAll(t => (IThreatSource)t), out var bestScore);

        // Hysteresis: don’t switch too eagerly
        if (currentThreat != null && Time.time < stickUntil)
            best = currentThreat;

        float curScore = (currentThreat == null) ? 0f : Score(currentThreat);
        bool change = (best != currentThreat) && (bestScore >= curScore + minScoreDeltaToSwitch);

            if (change)
            {
                currentThreat = best;
                stickUntil = Time.time + stickinessSeconds;
                OnTargetChanged?.Invoke(currentThreat as IHittable);
            if (debugLogs)
                Debug.Log($"[{name}] TargetChanged → {((currentThreat as Component)?.name ?? "Goal")}");
        }
    }

    IThreatSource PickBest(List<IThreatSource> seen, out float bestScore)
    {
        IThreatSource best = null; bestScore = 0f;
        for (int i = 0; i < seen.Count; i++)
        {
            var t = seen[i];
            if (t == null || !t.IsAlive) continue;
            float s = Score(t);
            if (s > bestScore) { bestScore = s; best = t; }
        }
        // light aggro decay
        if (aggro.Count > 0)
        {
            var keys = new List<IThreatSource>(aggro.Keys);
            foreach (var k in keys) aggro[k] = Mathf.Max(0f, aggro[k] - 5f * rethinkInterval);
        }
        return best;
    }

    float Score(IThreatSource t)
    {
        float dist = Vector3.Distance(transform.position, t.transform.position);
        float sStatic = t.StaticPriority * wStatic;
        float sDist = (1f / (1f + dist)) * wDistance;
        float sClass = t.Class switch
        {
            ThreatClass.Decoy => 3f,
            ThreatClass.Tower => 2f,
            ThreatClass.Wall  => 1.5f,
            ThreatClass.Goal  => 1.0f,
            ThreatClass.Unit  => 1.2f,
            _ => 1f
        } * wClassBias;
        float sAggro = (aggro.TryGetValue(t, out var a) ? a : 0f) * wAggro;
        return sStatic + sClass + sAggro + sDist;
    }

    public void AddAggro(IThreatSource src, float amount)
    {
        if (src == null) return;
        aggro.TryGetValue(src, out var a);
        aggro[src] = Mathf.Min(a + amount, 100f);
    }
}
}