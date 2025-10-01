using UnityEngine;
using Core.Combat;      // Health, DamagePacket
using Core.Structures; // StructureTag
using Core.Pathing;    // PathFollower (pause/resume)

namespace Core.Enemies
{
    [RequireComponent(typeof(Health))]
    public class EnemyAttacker : MonoBehaviour
    {
        [Header("Targeting")]
        public LayerMask targetMask;          // set to Structure
        public float scanRadius = 2.0f;
        public float attackRange = 1.6f;
        public float retargetEvery = 0.25f;
        public bool preferCommander = true;

        [Header("Attack")]
        public float damagePerHit = 4f;
        public float attackPeriod = 0.8f;     // seconds between hits
        public float windup = 0.05f;          // small delay before first hit after in-range
        public bool faceTarget = true;

        [Header("Refs (auto-find okay)")]
        public EnemyMoverSpline mover;

        // runtime
        Transform _target;
        Health _targetHealth;
        float _scanT, _atkT;
        Collider[] _buf = new Collider[32];

        void Awake()
        {
            if (!mover) mover = GetComponent<EnemyMoverSpline>();
        }

        void Update()
        {
            // Retarget periodically
            _scanT -= Time.deltaTime;
            if (_scanT <= 0f) { _scanT = retargetEvery; AcquireTarget(); }

            if (_target && _targetHealth && _target.gameObject.activeInHierarchy)
            {
                float d = Vector3.Distance(transform.position, _target.position);
                bool inRange = d <= attackRange;

                // Pause/Resume pathing
                if (mover) mover.SetPaused(inRange);

                // Face target
                if (faceTarget && inRange)
                {
                    Vector3 f = _target.position - transform.position;
                    f.y = 0;
                    if (f.sqrMagnitude > 0.001f)
                        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(f), 12f * Time.deltaTime);
                }

                // Attack cadence
                if (inRange)
                {
                    _atkT -= Time.deltaTime;
                    if (_atkT <= 0f)
                    {
                        // first hit after entering range gets a tiny windup
                        _atkT = attackPeriod;
                        Invoke(nameof(DoHit), windup);
                    }
                }
                else
                {
                    // not in melee → keep timer primed
                    _atkT = Mathf.Min(_atkT, 0.05f);
                }
            }
            else
            {
                if (mover) mover.SetPaused(false);
                _target = null; _targetHealth = null;
            }
        }

        void DoHit()
        {
            if (!_targetHealth || !_target || !_target.gameObject.activeInHierarchy) return;

            float outgoing = damagePerHit;

            // (Optional) if you later expose enemy debuffs (Neutralize), multiply here:
            // outgoing *= GetOutgoingDamageMultFromStatusEffects();

            var pkt = new DamagePacket { amount = outgoing };
            _targetHealth.ApplyDamage(pkt);
        }

        void AcquireTarget()
        {
            int hits = Physics.OverlapSphereNonAlloc(transform.position, scanRadius, _buf, targetMask, QueryTriggerInteraction.Ignore);
            Transform best = null; Health bestH = null; int bestP = int.MinValue; float bestD = float.MaxValue;

            for (int i = 0; i < hits; i++)
            {
                var col = _buf[i];
                if (!col) continue;
                var t = col.attachedRigidbody ? col.attachedRigidbody.transform : col.transform;
                if (!t || !t.gameObject.activeInHierarchy) continue;

                if (!t.TryGetComponent<Health>(out var h)) continue;
                if (!t.TryGetComponent<StructureTag>(out var tag)) continue;

                int prio = tag.priority + (preferCommander && tag.type == StructureType.Commander ? 1000 : 0);
                float d = Vector3.Distance(transform.position, t.position);

                // prefer higher priority, then closer
                if (prio > bestP || (prio == bestP && d < bestD))
                {
                    bestP = prio; bestD = d; best = t; bestH = h;
                }
            }

            _target = best; _targetHealth = bestH;
        }

#if UNITY_EDITOR
        void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(1,0.4f,0.2f,0.35f);
            Gizmos.DrawWireSphere(transform.position, scanRadius);
            Gizmos.color = new Color(1,0.2f,0.2f,0.7f);
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
#endif
    }
}
