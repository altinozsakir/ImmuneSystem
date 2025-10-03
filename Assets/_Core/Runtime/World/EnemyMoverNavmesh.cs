using UnityEngine;
using UnityEngine.AI;

namespace ImmuneSystem.World
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class EnemyMoverNavmesh : MonoBehaviour
    {
        [SerializeField] private Transform goal;
        [SerializeField, Range(0.5f, 10f)] private float baseSpeed = 2f;

        private NavMeshAgent agent;
        private float phaseSpeedMultiplier = 1f;   // set by EnemySpeedPhaseAdapter
        private float ccSpeedMultiplier = 1f;      // set by CrowdControl
        public float DistanceToGoal { get; private set; }

        void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            agent.updateRotation = true;
            agent.autoBraking = false;
            agent.obstacleAvoidanceType = ObstacleAvoidanceType.LowQualityObstacleAvoidance;
        }

        void OnEnable()
        {
            if (goal) agent.SetDestination(goal.position);
            // TODO: subscribe to phase change & CC speed events you already fire
            // BodyClockDirector.OnMultipliersChanged += OnPhaseMultipliers;
            // CrowdControl.OnSpeedChanged += OnCCSpeedChanged;
            ApplySpeed();
        }

        void Update()
        {
            // Cheap progress metric for tower targeting
            DistanceToGoal = ComputeRemainingPath(agent);

            // Repath occasionally for stability (lightweight)
            if (Time.frameCount % 20 == 0 && goal)
                agent.SetDestination(goal.position);
        }

        private void ApplySpeed()
        {
            agent.speed = Mathf.Max(0.01f, baseSpeed * phaseSpeedMultiplier * ccSpeedMultiplier);
        }

        private static float ComputeRemainingPath(NavMeshAgent a)
        {
            if (a.pathPending || a.path == null) return Mathf.Infinity;
            var path = a.path;
            float d = 0f;
            if (path.corners.Length < 2) return (a.destination - a.transform.position).magnitude;
            for (int i = 1; i < path.corners.Length; i++)
                d += Vector3.Distance(path.corners[i - 1], path.corners[i]);
            return d;
        }

        // Example hooks
        public void OnPhaseMultipliers(float enemySpeedMultiplier)
        {
            phaseSpeedMultiplier = enemySpeedMultiplier;
            ApplySpeed();
        }

        public void OnCCSpeedChanged(float ccMult)
        {
            ccSpeedMultiplier = ccMult;
            ApplySpeed();
        }
    }
}
