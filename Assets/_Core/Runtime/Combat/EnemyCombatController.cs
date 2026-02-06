using UnityEngine;
using Core.Enemies;

using UnityEngine.AI;

namespace Core.Combat
{


    [RequireComponent(typeof(NavMeshAgent))]
    public class EnemyCombatController : MonoBehaviour
    {

        [SerializeField] private MonoBehaviour fallbackGoalBehaviour; // assign StructureHealth on Goal (implements IHittable)
        IHittable goal;
        IHittable current;

        NavMeshAgent agent;


        [SerializeField] public float attackRange;
        [SerializeField] public float attacksPerSecond;
        [SerializeField] public float contactDamage;
        float cd;
        private EnemyAnimatorDriver _anim;
        private float _cooldown;
        void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            _anim = GetComponent<EnemyAnimatorDriver>();
            agent.autoBraking = false;
            agent.stoppingDistance = Mathf.Max(agent.stoppingDistance, attackRange * 0.85f);
            goal = fallbackGoalBehaviour as IHittable;
        }

        public void SetFallbackGoal(IHittable g) => goal = g;

        void Update()
        {
            // choose target (priority system can set 'current' externally; fall back to goal)
            var tgt = (current != null && current.IsAlive) ? current : goal;
            if (tgt == null) return;


            float dist = DistanceTo(tgt);
            bool inRange = dist <= attackRange + 0.02f;
            agent.isStopped = inRange;

            if (dist < attackRange * 1.5f)
            {
                var dir = tgt.transform.position - transform.position; dir.y = 0;
                if (dir.sqrMagnitude > 0.0001f)
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 0.2f);
            }

            cd -= Time.deltaTime;
            if (inRange && cd <= 0f)
            {
                _anim?.PlayAttack();
                tgt.TakeDamage(contactDamage);
                cd = 1f / Mathf.Max(0.01f, attacksPerSecond);
            }
        }

        float DistanceTo(IHittable t)
        {
            if (t is Component c && c.TryGetComponent<Collider>(out var col))
                return Vector3.Distance(transform.position, col.ClosestPoint(transform.position));
            return Vector3.Distance(transform.position, t.transform.position);
        }

        // Called by your targeting system when it picks a new high-priority threat
        public void SetCurrentTarget(IHittable t) => current = t;
    }
}