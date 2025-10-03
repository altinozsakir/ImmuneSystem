using UnityEngine;
using Core.Combat;      // Health, DamagePacket
using Core.Pathing;     // NavAgentMover
using Core.Structures;  // StructureTag

namespace Core.Enemies
{
    [RequireComponent(typeof(Health))]
    public class EnemyAttacker : MonoBehaviour
    {
        [Header("Targeting")]
        public LayerMask targetMask;            // set to Structure layer
        [Min(0f)] public float scanRadius = 3.0f;
        [Tooltip("Melee range measured via ClosestPoint between colliders.")]
        [Min(0f)] public float attackRange = 1.8f;
        [Tooltip("Distance to resume movement once we stepped back out.")]
        [Min(0f)] public float resumeRange = 2.1f;
        public float retargetEvery = 0.25f;
        public bool preferCommander = true;

        [Header("Attack")]
        public float damagePerHit = 4f;
        public float attackPeriod = 0.8f;
        public float windup = 0.05f;
        public bool faceTarget = true;

        [Header("Refs (auto-found)")]
        public NavAgentMover mover;
        public Collider selfCollider;

        // runtime
        Transform _target;
        Health _targetHealth;
        Collider _targetCol;
        float _scanT, _atkT;
        bool _inRange;
        readonly Collider[] _buf = new Collider[32];

        void Awake()
        {
            if (!mover) mover = GetComponent<NavAgentMover>();
            if (!selfCollider) selfCollider = GetComponent<Collider>();
        }

        void Update()
        {
            // Retarget on cadence
            _scanT -= Time.deltaTime;
            if (_scanT <= 0f) { _scanT = retargetEvery; AcquireTarget(); }

            if (_target && _targetHealth && _target.gameObject.activeInHierarchy)
            {
                _inRange = InRange();

                // pause mover while in melee
                if (mover) mover.SetPaused(_inRange);

                // face target
                if (faceTarget && _inRange)
                {
                    Vector3 dir = _target.position - transform.position;
                    dir.y = 0f;
                    if (dir.sqrMagnitude > 0.001f)
                        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 12f * Time.deltaTime);
                }

                // attack cadence
                if (_inRange)
                {
                    _atkT -= Time.deltaTime;
                    if (_atkT <= 0f)
                    {
                        _atkT = attackPeriod;
                        if (windup > 0f) Invoke(nameof(DoHit), windup);
                        else DoHit();
                    }
                }
                else
                {
                    // keep timer primed so first swing triggers fast if we step in again
                    _atkT = Mathf.Min(_atkT, 0.05f);
                }
            }
            else
            {
                _inRange = false;
                if (mover) mover.SetPaused(false);
            }
        }

        void DoHit()
        {
            if (!_targetHealth || !_target || !_target.gameObject.activeInHierarchy) return;

            float outgoing = damagePerHit;
            // If you have debuffs like Neutralize, multiply here (outgoing *= ...)
            var pkt = new DamagePacket { amount = outgoing };
            _targetHealth.ApplyDamage(pkt);
        }

        void AcquireTarget()
        {
            int hits = Physics.OverlapSphereNonAlloc(transform.position, scanRadius, _buf, targetMask, QueryTriggerInteraction.Collide);

            Transform best = null; Health bestH = null; Collider bestC = null;
            int bestP = int.MinValue; float bestD = float.MaxValue;

            for (int i = 0; i < hits; i++)
            {
                var col = _buf[i]; if (!col) continue;
                var t = col.attachedRigidbody ? col.attachedRigidbody.transform : col.transform;
                if (!t || !t.gameObject.activeInHierarchy) continue;

                if (!t.TryGetComponent(out Health h)) continue;
                if (!t.TryGetComponent(out StructureTag tag)) continue;
                if (!t.TryGetComponent(out Collider tc)) continue;

                int prio = tag.priority + (preferCommander && tag.type == StructureType.Commander ? 1000 : 0);

                // prioritize higher priority and then closer center distance
                float d = Vector3.Distance(transform.position, t.position);
                if (prio > bestP || (prio == bestP && d < bestD))
                {
                    bestP = prio; bestD = d; best = t; bestH = h; bestC = tc;
                }
            }

            _target = best; _targetHealth = bestH; _targetCol = bestC;
        }

        bool InRange()
        {
            if (!_targetCol || !selfCollider) 
                return Vector3.Distance(transform.position, _target.position) <= attackRange;

            Vector3 a = selfCollider.ClosestPoint(_target.position);
            Vector3 b = _targetCol.ClosestPoint(transform.position);
            float dist = Vector3.Distance(a, b);

            // hysteresis: once in range, stay "in" until we exceed resumeRange
            return _inRange ? (dist <= resumeRange) : (dist <= attackRange);
        }

#if UNITY_EDITOR
        void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(1f, 0.4f, 0.2f, 0.35f);
            Gizmos.DrawWireSphere(transform.position, scanRadius);
            Gizmos.color = new Color(1f, 0.2f, 0.2f, 0.7f);
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
#endif
    }
}
