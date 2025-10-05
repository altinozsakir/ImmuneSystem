using UnityEngine;

namespace Core.Enemies
{
    public class EnemyRangedSpitter : MonoBehaviour
    {
        public float range = 8f;
        public float attacksPerSecond = 0.66f;
        public float damage = 2f;
        public GameObject projectilePrefab;  // optional

        float _cooldown;
        Transform _target; // usually Goal or nearest structure

        public void SetTarget(Transform t) => _target = t;

        void Update()
        {
            if (!_target) return;
            var dir = _target.position - transform.position; dir.y = 0;
            var dist = dir.magnitude;
            _cooldown -= Time.deltaTime;

            if (dist <= range)
            {
                if (_cooldown <= 0f)
                {
                    // TODO: spawn projectile; for now, just pretend we hit:
                    // _target.GetComponent<GoalZone>()?.TakeDamage(damage);
                    _cooldown = 1f / attacksPerSecond;
                }
            }
        }
    }
}
