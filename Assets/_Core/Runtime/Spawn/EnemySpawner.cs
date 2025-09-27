using UnityEngine;
using Core.Pathing;
using Core.Combat;

namespace Core.Spawn
{
    public class EnemySpawner : MonoBehaviour
    {
        public WaypointPath path;
        public GameObject enemyPrefab;
        public int count = 12;
        public float interval = 0.75f;
        public float startYOffset = 0.0f; // lift if ground clips
        public Transform container;

        float _timer;
        int _spawned;

        void OnValidate() { if (interval < 0.05f) interval = 0.05f; }

        void Update()
        {
            if (!path || !enemyPrefab) return;
            if (_spawned >= count) return;

            _timer -= Time.deltaTime;
            if (_timer <= 0f)
            {
                _timer = interval;
                SpawnOne();
            }
        }

        void SpawnOne()
        {
            var p0 = path.GetPoint(0);
            Vector3 spawnPos = (p0 ? p0.position : transform.position) + new Vector3(0, startYOffset, 0);

            var go = Instantiate(enemyPrefab, spawnPos, Quaternion.identity, container ? container : transform);
            go.tag = "Enemy";
            go.layer = LayerMask.NameToLayer("Enemy");

            
            if (go.TryGetComponent<PathFollower>(out var follower))
            {

                follower.enabled = false;
                follower.path = path;
                follower.enabled = true;
            }

            _spawned++;
        }
    }
}
