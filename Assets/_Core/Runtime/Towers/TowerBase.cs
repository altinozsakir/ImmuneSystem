using UnityEngine;

namespace Core.Towers
{
    /// Minimal shared tower stats + cooldown.
    public class TowerBase : MonoBehaviour
    {
        [Header("Stats")]
        [Min(0.1f)] public float range = 6f;      // meters
        [Min(0.1f)] public float fireRate = 2f;   // shots per second
        public float damage = 2f;                 // optional (proj can override)

        [Header("Build")]
        public int buildCostATP = 25;   // <--- NEW

        [Header("Transforms")]
        public Transform yawPivot;                // rotates to face target (optional)
        public Transform muzzle;                  // projectile spawn point (optional)

        float _cooldown;

        public bool CanFire => _cooldown <= 0f;

        void OnEnable() { _cooldown = 0f; }

        void Update()
        {
            if (_cooldown > 0f) _cooldown -= Time.deltaTime;
        }

        public void ResetCooldown()
        {
            _cooldown = 1f / Mathf.Max(0.01f, fireRate);
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, range);
        }
    }
}