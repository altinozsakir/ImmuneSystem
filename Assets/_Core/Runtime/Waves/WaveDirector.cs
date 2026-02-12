using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Waves
{
    public class WaveDirector : MonoBehaviour
    {
        [Header("Refs")]
        [SerializeField] private WaveConfig config;
        [SerializeField] private EnemySpawner spawner;

        [Header("Runtime")]
        [SerializeField] private bool autoStart = true;
        [SerializeField] private bool waitUntilAllDeadBeforeNextWave = true;

        private readonly HashSet<GameObject> alive = new();

        private Coroutine routine;
        private int currentWaveIndex = -1;

        private void Awake()
        {
            if (!spawner) spawner = FindFirstObjectByType<EnemySpawner>();
        }

        private void Start()
        {
            if (autoStart)
                StartWaves();
        }

        public void StartWaves()
        {
            if (routine != null) StopCoroutine(routine);
            routine = StartCoroutine(RunAllWaves());
        }

        public void StopWaves()
        {
            if (routine != null) StopCoroutine(routine);
            routine = null;
        }

        public int AliveCount => alive.Count;
        public int CurrentWaveIndex => currentWaveIndex;

        private IEnumerator RunAllWaves()
        {
            if (!config || config.waves == null || config.waves.Count == 0)
            {
                Debug.LogError("[WaveDirector] Missing WaveConfig or waves empty.");
                yield break;
            }

            if (!spawner)
            {
                Debug.LogError("[WaveDirector] Missing EnemySpawner reference.");
                yield break;
            }

            for (int w = 0; w < config.waves.Count; w++)
            {
                currentWaveIndex = w;
                var wave = config.waves[w];

                if (wave.preDelay > 0f)
                    yield return new WaitForSeconds(wave.preDelay);

                // Spawn each group sequentially (simple, deterministic)
                for (int g = 0; g < wave.groups.Count; g++)
                    yield return SpawnGroupCoroutine(wave.groups[g]);

                // Wait for all dead (optional)
                if (waitUntilAllDeadBeforeNextWave)
                {
                    while (alive.Count > 0)
                        yield return null;
                }

                if (wave.postDelay > 0f)
                    yield return new WaitForSeconds(wave.postDelay);
            }

            routine = null;
            Debug.Log("[WaveDirector] Completed all waves.");
        }

        private IEnumerator SpawnGroupCoroutine(WaveConfig.SpawnGroup group)
        {
            if (group.enemyPrefab == null)
            {
                Debug.LogWarning("[WaveDirector] SpawnGroup missing prefab.");
                yield break;
            }

            for (int i = 0; i < group.count; i++)
            {
                var go = spawner.Spawn(group.enemyPrefab, group.lane);
                if (go != null)
                {
                    alive.Add(go);

                    // Track destruction automatically
                    var tracker = go.AddComponent<AliveTracker>();
                    tracker.Init(this, go);
                }

                yield return new WaitForSeconds(group.interval);
            }
        }

        private void NotifyDead(GameObject go)
        {
            if (go != null) alive.Remove(go);
        }

        /// <summary>
        /// Simple helper that notifies the director when the enemy is destroyed.
        /// Later you’ll replace this with a real Health/OnDied event.
        /// </summary>
        private sealed class AliveTracker : MonoBehaviour
        {
            private WaveDirector director;
            private GameObject owner;

            public void Init(WaveDirector director, GameObject owner)
            {
                this.director = director;
                this.owner = owner;
            }

            private void OnDestroy()
            {
                if (director != null)
                    director.NotifyDead(owner);
            }
        }
    }
}
