using UnityEngine;
using UnityEngine.AI;

namespace Core.Enemies
{
    /// <summary>
    /// Drives an enemy Animator using NavMeshAgent velocity + attack triggers.
    /// Attach to the enemy ROOT (recommended).
    /// </summary>
    public sealed class EnemyAnimatorDriver : MonoBehaviour
    {
               [Header("Wiring (can be left empty)")]
        [SerializeField] private Animator animator;
        [SerializeField] private NavMeshAgent agent;

        [Header("Tuning")]
        [Tooltip("If 0, will auto-use agent.speed when available.")]
        [SerializeField] private float walkSpeedReference = 0f;

        [SerializeField] private float speedDampTime = 0.10f;

        private static readonly int SpeedHash  = Animator.StringToHash("Speed");
        private static readonly int AttackHash = Animator.StringToHash("Attack");

        private Vector3 _prevPos;
        private float _cachedRefSpeed = 3.5f;

        private void Reset()
        {
            agent = GetComponent<NavMeshAgent>();
            animator = GetComponentInChildren<Animator>(true);
        }

        private void Awake()
        {
            if (agent == null) agent = GetComponent<NavMeshAgent>();
            if (animator == null) animator = GetComponentInChildren<Animator>(true);

            _prevPos = transform.position;

            if (animator != null)
                animator.applyRootMotion = false;

            _cachedRefSpeed = ResolveRefSpeed();
        }

        private void Update()
        {
            // In case visuals/animator are spawned after Awake
            if (animator == null)
            {
                animator = GetComponentInChildren<Animator>(true);
                if (animator != null)
                    animator.applyRootMotion = false;
            }

            if (animator == null) return;

            float dt = Time.deltaTime;
            if (dt <= 0f) return;

            float speed = 0f;

            // Prefer NavMeshAgent desired velocity if available
            if (agent != null && agent.enabled)
            {
                speed = agent.desiredVelocity.magnitude;

                // Some setups yield near-zero desiredVelocity; fallback to velocity
                if (speed < 0.01f)
                    speed = agent.velocity.magnitude;
            }

            // Fallback: measure actual transform movement
            if (speed < 0.01f)
            {
                speed = (transform.position - _prevPos).magnitude / dt;
            }

            _prevPos = transform.position;

            float refSpeed = ResolveRefSpeed();
            float speed01 = (refSpeed <= 0.001f) ? 0f : Mathf.Clamp01(speed / refSpeed);

            animator.SetFloat(SpeedHash, speed01, speedDampTime, dt);
        }

        public void PlayAttack()
        {
            if (animator == null) return;
            animator.ResetTrigger(AttackHash);
            animator.SetTrigger(AttackHash);
        }

        private float ResolveRefSpeed()
        {
            if (walkSpeedReference > 0.001f) return walkSpeedReference;
            if (agent != null) return Mathf.Max(0.1f, agent.speed);
            return _cachedRefSpeed;
        }
    }
}