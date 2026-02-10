using UnityEngine;

namespace Core.Enemy
{
    [CreateAssetMenu(menuName = "ImmuneTD/Enemy/EnemyConfig")]
    public class EnemyConfig: ScriptableObject
    {
        [Header("Movement")]
        public float moveSpeed = 3.5f;
        public float stoppingDistance = 1.2f;

        [Header("Targeting")]
        public float aggroRadius = 10f;

        [Header("Attack (Melee Sim)")]
        public float attackRange = 1.4f;
        public float attackWindup = 0.12f;
        public float attackRecover = 0.18f;
        public float attackDamage = 5f;
        public float attackCooldown = 0.6f;

        [Header("Layers")]
        public LayerMask damageableMask; // for simulation / future IDamageable selection
    }
}