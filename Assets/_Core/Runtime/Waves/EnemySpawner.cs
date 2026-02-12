using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Core.Waves
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private List<SpawnPoint> spawnPoints = new();
        [SerializeField] private Transform objective; // pass to EnemyInstaller

        public Transform Objective => objective;

        public void SetObjective(Transform obj) => objective = obj;

        public void RegisterSpawnPoint(SpawnPoint p)
        {
            if (p != null && !spawnPoints.Contains(p))
                spawnPoints.Add(p);
        }

        public SpawnPoint PickSpawnPoint(string lane = null)
        {
            // Filter by lane if provided
            List<SpawnPoint> candidates = spawnPoints;
            if (!string.IsNullOrEmpty(lane))
            {
                candidates = new List<SpawnPoint>();
                for (int i = 0; i < spawnPoints.Count; i++)
                    if (spawnPoints[i] != null && spawnPoints[i].lane == lane)
                        candidates.Add(spawnPoints[i]);
            }

            if (candidates == null || candidates.Count == 0) return null;

            float total = 0f;
            for (int i = 0; i < candidates.Count; i++)
                total += Mathf.Max(0.01f, candidates[i].weight);

            float r = Random.value * total;
            for (int i = 0; i < candidates.Count; i++)
            {
                float w = Mathf.Max(0.01f, candidates[i].weight);
                if (r <= w) return candidates[i];
                r -= w;
            }
            return candidates[candidates.Count - 1];
        }

        public GameObject Spawn(GameObject enemyPrefab, string lane = null)
        {
            if (!enemyPrefab)
            {
                Debug.LogError("[EnemySpawner] Missing enemyPrefab");
                return null;
            }

            SpawnPoint sp = PickSpawnPoint(lane);
            if (!sp)
            {
                Debug.LogError("[EnemySpawner] No spawn points registered.");
                return null;
            }

            Vector3 pos = sp.transform.position;
            Quaternion rot = sp.transform.rotation;

            // Optional: snap to NavMesh if needed
            if (NavMesh.SamplePosition(pos, out var hit, 2f, NavMesh.AllAreas))
                pos = hit.position;

            var go = Instantiate(enemyPrefab, pos, rot);

            // Wire objective into EnemyInstaller if present
            var installer = go.GetComponent<Core.Enemy.EnemyInstaller>();
            if (installer != null && objective != null)
            {
                installer.SetObjective(objective);
            }

            return go;
        }
    }
}
