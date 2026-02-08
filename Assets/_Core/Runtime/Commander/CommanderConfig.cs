using UnityEngine;

namespace  Core.Commander
{
    [CreateAssetMenu(menuName ="ImmuneTD/Commander/CommanderConfig")]
    public class CommanderConfig: ScriptableObject
    {

        [Header("Locomotion")] 
        public float moveSpeed = 6f;

        [Header("Dash")]
        public float dashSpeed = 14f;
        public float dashDuration = 0.18f;
        public float dashCooldown = 5.0f;


        [Header("Attack")]
        public float attackCooldown = 0.35f;
        public float attackRange = 1.8f;
        public float attackWindup = 0.12f;
        public float attackRecover = 0.18f;
        public float attackDamage = 10f;


        [Header("Targeting")]
        public LayerMask damageableMask;

        [Header("Repair (future)")]
        public float repairCooldown = 0.25f;

        [Header("Build Mode")]
        public float buildCooldown = 0.25f;
        public float buildMaxRange = 8f;
        public float gridCellSize = 1f;
        public LayerMask groundMask;
        public LayerMask blockingMask; // colliders that prevent build (towers, rocks, etc.)
        public float blockingCheckRadius = 0.45f;

        public Build.BuildableDefinition defaultBuildable;
        public Material ghostValidMaterial;
        public Material ghostInvalidMaterial;
    }
}