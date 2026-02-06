using System.Collections;
using UnityEngine;
using Core.TimeSystem;   // BodyClockDirector, BodyPhase
using Core.Waves;
using Core.Enemies; // Add this if EnemySpawner is in Core.Spawning namespace

namespace Core.WavesRuntime
{
    public class WaveDirector : MonoBehaviour
    {
        [Header("Refs")]
        public BodyClockDirector clock;
        [Tooltip("Lane spawners (index by Wave.laneIndex)")]
        public EnemySpawnerNav[] lanes;
        public WaveList waveList;

        [Header("Debug")]
        public bool logStarts = true;

        Coroutine _phaseRoutine;

        void Awake()
        {
            if (!clock) clock = FindAnyObjectByType<BodyClockDirector>();
        }

        void OnEnable()
        {
            if (clock) clock.OnPhaseChanged += OnPhaseChanged;
        }
        void OnDisable()
        {
            if (clock) clock.OnPhaseChanged -= OnPhaseChanged;
        }

        void Start()
        {
            // start current phase if director enabled at runtime
            OnPhaseChanged(clock ? clock.CurrentPhase : BodyPhase.Morning);
        }

        void OnPhaseChanged(BodyPhase phase)
        {
            if (_phaseRoutine != null) StopCoroutine(_phaseRoutine);
            if (!waveList) return;

            var block = waveList.Get(phase);
            if (block == null || block.waves == null || block.waves.Count == 0)
            {
                if (logStarts) Debug.Log($"[Encounter] Phase {phase}: no waves");
                return;
            }

            _phaseRoutine = StartCoroutine(RunPhase(block));
        }

        IEnumerator RunPhase(PhaseBlock block)
        {
            if (logStarts) Debug.Log($"[Encounter] Start {block.phase} ({block.waves.Count} waves)");
            // Launch each wave as its own coroutine so overlaps are allowed
            foreach (var w in block.waves)
                StartCoroutine(RunWave(w));
            yield break;
        }

        IEnumerator RunWave(Wave w)
        {
            if (lanes == null || lanes.Length == 0) yield break;
            int idx = Mathf.Clamp(w.laneIndex, 0, lanes.Length - 1);
            var spawner = lanes[idx];
            if (!spawner) yield break;

            // wait for start offset (scaled time → slowed during planning windows)
            if (w.startAfter > 0f) yield return new WaitForSeconds(w.startAfter);

            if (logStarts) Debug.Log($"[Encounter] Wave \"{w.name}\" lane {idx} x{w.count} every {w.cadence:0.##}s");

            for (int i = 0; i < w.count; i++)
            {
                // decide prefab (elite roll)
                GameObject prefab = w.enemyPrefab;
                var a = Random.value;
                Debug.Log(a);
                if (w.elitePrefab && w.eliteChance > 0f && a < w.eliteChance)
                    prefab = w.elitePrefab;

                // spawn (prefab override)


                spawner.SpawnOne(prefab);         // fallback to its default


                if (w.cadence > 0f) yield return new WaitForSeconds(w.cadence);
            }
        }
    }
}
