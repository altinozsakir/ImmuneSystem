using UnityEngine;
using UnityEngine.AI;
using Core.Enemies;
using Core.Combat;
using Core.Structures;

namespace Core.Enemies
{
    // EnemySpawnerNav.cs  (only on the ONE NavMesh lane)
    public class EnemySpawnerNav : MonoBehaviour
    {
        [Header("Assign in Inspector")]   // Nav variant or unified prefab
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private Transform laneGoal;     
        [Header("Optional")]
        [SerializeField] private float spawnJitterRadius = 0.15f;


        public void SpawnOne(GameObject enemyPrefab)
        {

            var pos = spawnPoint ? spawnPoint.position : transform.position;
            pos = Jitter(pos, spawnJitterRadius);

            var go = Instantiate(enemyPrefab, pos, Quaternion.identity);
            var brain = go.GetComponent<EnemyBrain>();
            if (brain)
            {
                var goalH = laneGoal ? laneGoal.GetComponent<IHittable>() : null; // StructureHealth implements IHittable
                brain.fallbackGoal = goalH;              // one-time
                brain.SetChase(goalH);
            }

            // Prepare Nav agent/mover
            // var nav = go.GetComponent<EnemyMoverNavmesh>();
            var agent = go.GetComponent<NavMeshAgent>();

            if (!agent) agent = go.AddComponent<NavMeshAgent>(); // safety
            // if (!nav) nav = go.AddComponent<EnemyMoverNavmesh>();

            // Anti-jitter presets (light)
            agent.autoBraking = false;
            agent.stoppingDistance = Mathf.Max(agent.stoppingDistance, 0.08f);
            agent.obstacleAvoidanceType = ObstacleAvoidanceType.LowQualityObstacleAvoidance;
            agent.avoidancePriority = Random.Range(30, 70);

            var rb = go.GetComponent<Rigidbody>();
            if (rb) { rb.isKinematic = true; rb.interpolation = RigidbodyInterpolation.Interpolate; }

            // Pass the goal (this was the missing piece)
            //nav.enabled = true;
            //nav.SetGoal(laneGoal);
        }

        private Vector3 Jitter(Vector3 p, float r)
        {
            var o = Random.insideUnitCircle * r;
            return new Vector3(p.x + o.x, p.y, p.z + o.y);
        }
    }

}