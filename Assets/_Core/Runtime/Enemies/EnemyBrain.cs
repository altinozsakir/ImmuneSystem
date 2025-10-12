using UnityEngine;
using UnityEngine.AI;
// Add the correct namespace for IHittable below. Replace 'Core.Interfaces' with the actual namespace if different.

using Core.Combat;

namespace Core.Enemies
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(EnemyTargeting))]
    [RequireComponent(typeof(EnemyCombatController))]
    public class EnemyBrain : MonoBehaviour
    {
        [Header("Fallback Goal (IHittable)")]
        [SerializeField] private MonoBehaviour fallbackGoalBehaviour; // StructureHealth (implements IHittable)
        public IHittable fallbackGoal { set; get; }

        [Header("Pathing")]
        [SerializeField] private float repathInterval = 0.4f;
        [SerializeField] private float minMoveToRepath = 0.25f;

        NavMeshAgent agent;
        EnemyTargeting targeting;
        EnemyCombatController combat;
        IHittable chase;                 // current chase target (threat or goal)
        Vector3 lastChasePos;
        float nextRepath;

        void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            targeting = GetComponent<EnemyTargeting>();
            combat = GetComponent<EnemyCombatController>();
            fallbackGoal = fallbackGoalBehaviour as IHittable;

            // sane anti-jitter defaults
            agent.autoBraking = false;
            agent.updateRotation = false;
            agent.stoppingDistance = Mathf.Max(agent.stoppingDistance, combat.GetComponentInParent<EnemyCore>().archetype.attackRange * 0.85f);

            targeting.OnTargetChanged += OnTargetChanged;
        }

        void Start()
        {
            // Initialize chase with goal if nothing selected yet
            SetChase(fallbackGoal);
        }

        void OnDestroy()
        {
            if (targeting) targeting.OnTargetChanged -= OnTargetChanged;
        }

        void Update()
        {
            // If the threat died/disappeared, fall back to goal
            if (chase == null || (chase is IHittable h && !h.IsAlive))
                SetChase(fallbackGoal);

            // Throttled re-pathing (and only if target moved meaningfully)
            if (Time.time >= nextRepath && chase != null)
            {
                var p = chase.transform.position;
                if ((p - lastChasePos).sqrMagnitude >= minMoveToRepath * minMoveToRepath)
                {
                    agent.SetDestination(p);
                    lastChasePos = p;
                }
                nextRepath = Time.time + repathInterval;
            }

            // let combat handle stopping/attacking; we just face velocity
            var v = agent.velocity; v.y = 0f;
            if (v.sqrMagnitude > 0.001f)
            {
                var look = Quaternion.LookRotation(v, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, look, 0.2f);
            }
        }

        void OnTargetChanged(IHittable newTarget)
        {
            // prefer new threat; fall back to goal if null
            SetChase(newTarget ?? fallbackGoal);
        }

        public void SetChase(IHittable t)
        {
            if (!agent || !combat) { Debug.LogError($"{name}: missing Agent/Combat"); return; }

            if (t == null)
            {
                if (fallbackGoal == null) { Debug.LogError($"{name}: no fallback goal"); return; }
                t = fallbackGoal;
            }

            var comp = t as Component;
            if (!comp) { Debug.LogError($"{name}: IHittable is not a Component"); return; }

            // make sure we're on a NavMesh before setting destination
            if (!agent.isOnNavMesh)
            {
                if (NavMesh.SamplePosition(transform.position, out var hit, 2f, agent.areaMask))
                    agent.Warp(hit.position);
                else
                    return; // try again next frame (Update can re-call SetChase if you cache pending)
            }

            chase = t;
            combat.SetCurrentTarget(chase);
            agent.isStopped = false;
            agent.SetDestination(comp.transform.position);
        }
    }
}

