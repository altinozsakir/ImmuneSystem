using System.Collections.Generic;
using UnityEngine;
using Core.Combat;


namespace Core.Towers
{
    public class NETPitAura : MonoBehaviour
    {
        public float radius = 3.5f;
        public float snareDuration = 2.5f;
        public LayerMask enemyMask;
        public int maxHits = 32;
        public float scanInterval = 0.1f;


        readonly HashSet<Transform> _inside = new();
        Collider[] _buf; float _timer;


        void Awake() { _buf = new Collider[Mathf.Max(4, maxHits)]; }


        void Update()
        {
            _timer -= Time.deltaTime; if (_timer > 0f) return; _timer = scanInterval;


            // scan current set
            int hits = Physics.OverlapSphereNonAlloc(transform.position, radius, _buf, enemyMask, QueryTriggerInteraction.Ignore);
            var seen = new HashSet<Transform>();
            for (int i = 0; i < hits; i++)
            {
                var tr = _buf[i].attachedRigidbody ? _buf[i].attachedRigidbody.transform : _buf[i].transform;
                if (!tr || !tr.gameObject.activeInHierarchy) continue;
                seen.Add(tr);
                if (!_inside.Contains(tr))
                {
                    // exposure start → apply snare
                    if (tr.TryGetComponent<CrowdControl>(out var cc)) cc.AddSnare(snareDuration);
                }
            }
            // update inside set
            _inside.Clear();
            foreach (var t in seen) _inside.Add(t);
        }


        void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(0.6f, 0.9f, 1f, 0.5f);
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }
}