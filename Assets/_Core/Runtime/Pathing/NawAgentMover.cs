using UnityEngine;
using UnityEngine.AI;

namespace Core.Pathing
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class NavAgentMover : MonoBehaviour
    {
        public Transform goal;     // assign your Goal/Commander, or set at spawn
        public float baseSpeed = 3.5f;

        NavMeshAgent _agent;
        bool _paused;

        void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _agent.updateRotation = true;
            _agent.updateUpAxis = true;
            _agent.speed = baseSpeed;
            _agent.autoBraking = false;
        }
void Start()
{
    if (!goal)
    {
        var cmd = FindAnyObjectByType<Core.Structures.Commander>();
        if (cmd) SetGoal(cmd.transform);
    }
}
void OnEnable()
{
    if (!_agent) _agent = GetComponent<NavMeshAgent>();
    _agent.isStopped = false;                 // ensure not paused
    if (goal) _agent.SetDestination(goal.position);
}

        public void SetDestination(Vector3 pos)
        {
            if (!_agent.isOnNavMesh) return;
            _agent.SetDestination(pos);
        }

public void SetGoal(Transform t)
{
    goal = t;
    if (_agent && goal) _agent.SetDestination(goal.position);
}

        public void SetPaused(bool paused)
        {
            _paused = paused;
            if (!_agent) return;
            _agent.isStopped = paused;
        }

        public void SetSpeedMultiplier(float mult)
        {
            _agent.speed = Mathf.Max(0f, baseSpeed * Mathf.Max(0f, mult));
        }

        // Optional: call if obstacles carve and we need to repath
        public void NudgeRepath()
        {
            if (goal) _agent.SetDestination(goal.position);
        }
    }
}