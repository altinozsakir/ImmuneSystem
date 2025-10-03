using UnityEngine;
using Core.Pathing;
using UnityEngine.AI;

namespace Core.Spawn
{
    public class EnemySpawnerNav : MonoBehaviour
    {
        public GameObject enemyPrefab;
        public Transform goal;              // Commander/GoalZone anchor
        public Transform container;         // optional parent for spawned enemies
        public float startYOffset = 0.0f;

        [Header("Optional lane pathing")]
        public Transform[] waypoints;       // if you want lanes, fill with 2–4 points

        public void SpawnOne() => SpawnOne(enemyPrefab);

        public void SpawnOne(GameObject prefabOverride)
        {
            var prefab = prefabOverride ? prefabOverride : enemyPrefab;
            if (!prefab) { Debug.LogWarning("[EnemySpawnerNav] No prefab."); return; }

            Vector3 spawnPos = transform.position + Vector3.up * startYOffset;
            var go = Instantiate(prefab, spawnPos, Quaternion.identity, container ? container : transform);
            go.tag = "Enemy";
            go.layer = LayerMask.NameToLayer("Enemy");

            // Ensure agent exists and is on the mesh
            var agent = go.GetComponent<NavMeshAgent>();
            if (agent)
            {
                // Find a point on the NavMesh near spawnPos (radius 2m, any area)
                if (NavMesh.SamplePosition(spawnPos, out var hit, 2f, NavMesh.AllAreas))
                {
                    agent.Warp(hit.position);            // place cleanly on the mesh (better than setting transform)
                }
                else
                {
                    Debug.LogWarning($"[SpawnerNav] No NavMesh near spawn at {spawnPos}. Check bake/layers.");
                }
            }

            var mover = go.GetComponent<Core.Pathing.NavAgentMover>();
            if (mover)
            {
                if (goal) mover.SetGoal(goal);                  // main path
                else mover.SetGoal(AutoFindGoal());             // fallback

                // If you use waypoints:
                var route = go.GetComponent<Core.Pathing.NavWaypointFollower>();
                if (route && waypoints != null && waypoints.Length > 0)
                    route.SetRoute(waypoints);
            }

            Transform AutoFindGoal()
            {
                // Try Commander first
                var cmd = FindAnyObjectByType<Core.Structures.Commander>();
                return cmd ? cmd.transform : null;
            }

        }
        
    }
}