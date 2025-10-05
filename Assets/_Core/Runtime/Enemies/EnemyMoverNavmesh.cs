using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMoverNavmesh : MonoBehaviour
{
    [SerializeField] private Transform goal;
    [SerializeField, Range(0.5f, 10f)] private float baseSpeed = 2f;
    [SerializeField, Range(0.05f, 1f)] private float repathInterval = 0.45f;
    [SerializeField, Range(0.01f, 1f)] private float minGoalMoveToRepath = 0.25f;

    private NavMeshAgent agent;
    private float phaseSpeedMultiplier = 1f;
    private float ccSpeedMultiplier = 1f;
    private float nextRepathAt;
    private Vector3 lastGoalPos;
    public float DistanceToGoal { get; private set; }

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        // Anti-jitter presets
        agent.updatePosition = true;      // if you use root motion, see the Animator section below
        agent.updateRotation = false;     // we'll smooth-rotate ourselves
        agent.autoBraking = false;
        agent.obstacleAvoidanceType = ObstacleAvoidanceType.LowQualityObstacleAvoidance;
        agent.acceleration = Mathf.Max(agent.acceleration, 10f);
        agent.angularSpeed = Mathf.Clamp(agent.angularSpeed, 160f, 220f);
        agent.stoppingDistance = Mathf.Max(agent.stoppingDistance, 0.075f);

        ApplySpeed();
    }

    public void SetGoal(Transform t)
    {
        goal = t;
        if (goal != null)
        {
            lastGoalPos = goal.position;
            agent.SetDestination(lastGoalPos);
            nextRepathAt = Time.time + Random.Range(0f, repathInterval); // stagger
        }
    }

    void OnEnable()
    {
        if (goal != null) SetGoal(goal); // ensure destination set
    }

    void Update()
    {
        if (goal != null && Time.time >= nextRepathAt)
        {
            // Repath only if goal moved noticeably
            if ((goal.position - lastGoalPos).sqrMagnitude >= minGoalMoveToRepath * minGoalMoveToRepath)
            {
                lastGoalPos = goal.position;
                agent.SetDestination(lastGoalPos);
            }
            nextRepathAt = Time.time + repathInterval;
        }

        // Distance for tower targeting
        DistanceToGoal = ComputeRemainingPath(agent);

        // Smooth face velocity direction (prevents zig-zag)
        var v = agent.velocity;
        v.y = 0f;
        if (v.sqrMagnitude > 0.0001f)
        {
            var targetRot = Quaternion.LookRotation(v, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 0.2f);
        }
    }

    private void ApplySpeed()
    {
        agent.speed = Mathf.Max(0.01f, baseSpeed * phaseSpeedMultiplier * ccSpeedMultiplier);
    }

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

    private static float ComputeRemainingPath(NavMeshAgent a)
    {
        if (!a || a.pathPending) return Mathf.Infinity;
        var p = a.path;
        if (p == null || p.corners == null || p.corners.Length < 2) return Mathf.Infinity;
        float d = 0f;
        for (int i = 1; i < p.corners.Length; i++)
            d += Vector3.Distance(p.corners[i - 1], p.corners[i]);
        return d;
    }
}
