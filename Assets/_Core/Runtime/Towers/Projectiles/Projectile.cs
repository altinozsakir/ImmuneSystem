using UnityEngine;

namespace Core.Towers.Projectiles
{
    public class Projectile: MonoBehaviour
    {
        private Transform target;
        private float damage;
        private float speed;
        private float dieAt;

        private Vector3 targetPoint;

        private bool usePoint;
    

        public void Init(Transform target, float damage, float speed, float lifetime)
        {
            this.target = target;
            this.damage = damage;
            this.speed = speed;
            usePoint = false;
            dieAt = Time.time + Mathf.Max(0.1f, lifetime);
        }

        public void InitPoint(Vector3 point, float damage, float speed, float lifetime)
        {
            targetPoint = point;
            this.damage = damage;
            this.speed = speed;
            usePoint = true;
            dieAt = Time.time + Mathf.Max(0.1f, lifetime);
        }

        private void Update()
        {
            if(Time.time >= dieAt)
            {
                Destroy(gameObject);
                return;
            }

            Vector3 dest;

            if (usePoint)
            {
                dest = targetPoint;
            }
            else{
                if(target == null)
                {
                    Destroy(gameObject);
                    return;
                }
                dest = target.position;
            }

            Vector3 dir = (dest - transform.position);
            float dist = dir.magnitude;
            if(dist < 0.15f)
            {
                if(!usePoint)
                    OnHit(target);
                Destroy(gameObject);
                return;
            }

            dir /= dist;
            transform.position += dir * (speed * Time.deltaTime);
        }

        private void OnHit(Transform t)
        {
            var dummy = t.GetComponentInParent<Core.Commander.DummyDamageable>();
            if (dummy != null)
            {
                dummy.TakeDamage(damage);
                return;
            }

            Debug.Log($"[Projectile] Hit {t.name} for {damage} (no damage receiver).");

        }
    }
}