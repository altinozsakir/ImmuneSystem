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
            if (currentThreat is Component cc && cc == null)
            {
                currentThreat = null;
                OnTargetChanged?.Invoke(null); // brain will fall back to goal
            }

            if (Time.time < nextThink) return;
            nextThink = Time.time + rethinkInterval;

            perception.PruneDead();

            float bestScore = 0f;
            IThreatSource best = null;

            foreach (var tp in perception.Seen) // Seen: List<ThreatProfile>
            {
                if (tp == null || !tp.IsAlive || tp.Team != Team.Player) continue;
                if (!tp.TryGetComponent<IThreatSource>(out var t)) continue;

                float s = Score(tp); // uses safe distance below
                if (s > bestScore) { bestScore = s; best = tp; }
            }
             if (best == null)
            {
                if (currentThreat != null)
                {
                    currentThreat = null;
                    OnTargetChanged?.Invoke(null); // ← tell the brain to go back to goal
                }
                return;
            }
            float curScore = (currentThreat == null) ? 0f : Score(currentThreat);
            bool change = (best != currentThreat) && (bestScore >= curScore + minScoreDeltaToSwitch);

            if (change || (Time.time >= stickUntil && best != currentThreat))
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
            // Guard destroyed
            var comp = t as Component;
            if (comp == null) return 0f;

            // Example weights/biases; tune as you like
            float dist = DistTo(t, transform.position);
            float wStatic = 1.0f, wDistance = 2.0f, wClassBias = 1.0f, wAggro = 1.2f;

            float sStatic = t.StaticPriority * wStatic;
            float sDist = (1f / (1f + Mathf.Max(0.1f, dist))) * wDistance;
            float sClass = ClassBias(t.Class) * wClassBias;
            float sAggro = 0f; // plug your aggro map here if you have one

            return sStatic + sClass + sAggro + sDist;
        }
        float ClassBias(ThreatClass c) => c switch
        {
            ThreatClass.Decoy => 3.0f,
            ThreatClass.Tower => 2.2f,
            ThreatClass.Wall => 1.4f,
            ThreatClass.Unit => 1.2f,
            ThreatClass.Goal => 1.0f,
            _ => 1.0f
        };
        public void AddAggro(IThreatSource src, float amount)
        {
            if (src == null) return;
            aggro.TryGetValue(src, out var a);
            aggro[src] = Mathf.Min(a + amount, 100f);
        }
        static float DistTo(IThreatSource t, Vector3 from)
        {
            var comp = t as Component;
            if (comp == null) return float.PositiveInfinity;
            if (comp.TryGetComponent<Collider>(out var col))
                return Vector3.Distance(from, col.ClosestPoint(from));
            return Vector3.Distance(from, comp.transform.position);
        }
    }
}