using UnityEngine;
using Core.Meta;    // InflammationMeter
using Core.Combat;  // Health

namespace Core.MetaHooks
{
    /// Raises Inflammation when this enemy dies.
    /// Optional: adds extra if the enemy has EnemyElite.
    public class InflammationOnDeath : MonoBehaviour
    {
        [Header("Amounts")]
        [Min(0)] public int amount = 1;
        [Min(0)] public int extraIfElite = 1;

        [Header("Refs (auto-found if empty)")]
        public InflammationMeter inflammation;
        public Health health;

        void Awake()
        {
            if (!health) health = GetComponent<Health>();
            if (!inflammation) inflammation = FindAnyObjectByType<InflammationMeter>();
        }

        void OnEnable()
        {
            if (health) health.onDeath.AddListener(OnDeath);
        }

        void OnDisable()
        {
            if (health) health.onDeath.RemoveListener(OnDeath);
        }

        void OnDeath()
        {
            if (!inflammation) return;

            int add = amount;

            // Optional bonus for elites (component is just a tag)

            if (add > 0) inflammation.Add(add);
        }
    }
}
