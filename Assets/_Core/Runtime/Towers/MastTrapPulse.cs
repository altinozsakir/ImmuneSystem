using System.Collections.Generic;
using UnityEngine;
using Core.Combat;


namespace Core.Towers
{
    public class MastTrapPulse : MonoBehaviour
    {
        [Header("Pulse")]
        public float radius = 4.5f;
        public float slowMagnitude = 0.40f; // 40% slow
        public float slowDuration = 3.0f;
        public float period = 8.0f; // every 8s
        public LayerMask enemyMask;
        public int maxHits = 32;


        [Header("Degranulate")]
        public bool applyMark = true;
        public int markStacks = 1;


        [Header("FX (optional)")]
        public GameObject pulseVFX;
        public AudioClip pulseSFX;


        float _timer;
        Collider[] _buf;


        void Awake() { _buf = new Collider[Mathf.Max(4, maxHits)]; }


        void Update()
        {
            _timer -= Time.deltaTime;
            if (_timer <= 0f)
            {
                _timer = period;
                DoPulse();
            }
        }


        void DoPulse()
        {
            if (pulseVFX) Instantiate(pulseVFX, transform.position, Quaternion.identity);
            if (pulseSFX) AudioSource.PlayClipAtPoint(pulseSFX, transform.position);


            int hits = Physics.OverlapSphereNonAlloc(transform.position, radius, _buf, enemyMask, QueryTriggerInteraction.Ignore);
            for (int i = 0; i < hits; i++)
            {
                var t = _buf[i].attachedRigidbody ? _buf[i].attachedRigidbody.transform : _buf[i].transform;
                if (!t || !t.gameObject.activeInHierarchy) continue;
                if (t.TryGetComponent<CrowdControl>(out var cc)) cc.AddSlow(slowMagnitude, slowDuration);
                if (applyMark && t.TryGetComponent<StatusEffects>(out var st)) st.AddMarks(markStacks);
            }
        }


        void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(0.8f, 0.6f, 1f, 0.5f);
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }
}