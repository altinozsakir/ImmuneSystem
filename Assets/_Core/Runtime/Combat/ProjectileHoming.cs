using UnityEngine;


namespace Core.Combat
{
    public class ProjectileHoming : MonoBehaviour
    {
        [Header("Motion")]
        public float speed = 22f;
        public float maxLifetime = 3f;
        public float hitRadius = 0.25f;


        [Header("Damage & Effects")]
        public float damage = 2f;
        public bool addMark = false; public int markStacks = 1;
        public bool addNeutralize = false; public int neutralizeStacks = 1;
        [Range(0f, 1f)] public float executeThreshold = 0f; // e.g., 0.15f for Macrophage


        [Header("VFX/SFX (optional)")]
        public GameObject hitVFX;
        public AudioClip hitSFX;


        Transform _target; float _life;


        public void Init(Transform target)
        {
            _target = target; _life = 0f;
        }


        void Update()
        {
            _life += Time.deltaTime;
            if (_life > maxLifetime || !_target || !_target.gameObject.activeInHierarchy)
            { Destroy(gameObject); return; }


            Vector3 pos = transform.position;
            Vector3 to = _target.position - pos; to.y = 0f;
            if (to.sqrMagnitude <= hitRadius * hitRadius)
            {
                OnHit(); return;
            }


            Vector3 dir = to.normalized;
            transform.position = pos + dir * speed * Time.deltaTime;
            if (dir.sqrMagnitude > 0.0001f)
                transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
        }


        void OnHit()
        {
            var hp = _target ? _target.GetComponent<Health>() : null;
            if (hp)
            {

                float dmgOut = damage * Core.Combat.GlobalCombatModifiers.DamageMult;
                float execThresh = Mathf.Clamp01(executeThreshold + Core.Combat.GlobalCombatModifiers.ExecuteBonus);


                bool exec = hp.Fraction <= execThresh && execThresh > 0f;
                var packet = new DamagePacket
                {
                amount = dmgOut,
                execute = exec,
                hitPoint = transform.position,
                markStacksToAdd = addMark ? markStacks : 0,
                neutralizeStacksToAdd = addNeutralize ? neutralizeStacks : 0,
                };

                hp.ApplyDamage(packet);
            }


            if (hitVFX) Instantiate(hitVFX, transform.position, Quaternion.identity);
            if (hitSFX) AudioSource.PlayClipAtPoint(hitSFX, transform.position);
            Destroy(gameObject);
        }
    }
}