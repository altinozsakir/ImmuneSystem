using UnityEngine;

namespace Core.Enemies
{
    public enum EnemyBehavior { MeleeCharger, RangedSpitter }

    [CreateAssetMenu(fileName = "EnemyArchetype", menuName = "ImmuneTD/Enemy Archetype")]
    public class EnemyArchetype : ScriptableObject
    {
        [Header("Identity")]
        public string displayName = "Bacteria Drone";
        public EnemyBehavior behavior = EnemyBehavior.MeleeCharger;
        public Color uiColor = Color.white;

        [Header("Stats")]
        public float maxHP = 10f;
        public float baseSpeed = 2.0f;         // read by EnemyMoverNavmesh
        public float contactDamage = 3f;       // for melee on structures/goal
        public float attackRange = 1.0f;       // melee ~1, ranged > 6
        public float attacksPerSecond = 1.0f;  // DPS = contactDamage * APS
        public int atpRewardOnDeath = 2;

        [Header("Resistances")]
        [Range(0f, 0.9f)] public float flatArmor = 0f;     // simple % reduction
        [Range(0.5f, 1.2f)] public float slowResist = 1f;  // multiplies slow (0.8 means 20% less slow)

        [Header("NavMesh Tuning")]
        [Range(0.2f, 0.6f)] public float agentRadius = 0.32f;
        public float agentHeight = 2.0f;
        [Range(0, 99)] public int avoidancePriority = 50;

        [Header("Presentation")]
        public Vector3 scale = Vector3.one;    // quick visual size
        public GameObject visualPrefab;        // optional child mesh/VFX
        public AudioClip hitSfx, deathSfx;
    }
}
