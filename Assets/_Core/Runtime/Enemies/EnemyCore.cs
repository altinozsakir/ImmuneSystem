using UnityEngine;
using UnityEngine.AI;

namespace Core.Enemies
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class EnemyCore : MonoBehaviour
    {
        public EnemyArchetype archetype;

        [HideInInspector] public float HP;

        private NavMeshAgent agent;
        private EnemyMoverNavmesh mover;   // your existing mover
        private EnemyMelee melee;
        private EnemyRangedSpitter spitter;

        void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            mover = GetComponent<EnemyMoverNavmesh>();
            melee = GetComponent<EnemyMelee>();
            spitter = GetComponent<EnemyRangedSpitter>();

            ApplyArchetype();
        }

        private void ApplyArchetype()
        {
            if (!archetype)
            {
                Debug.LogError($"{name}: Missing EnemyArchetype");
                return;
            }

            HP = archetype.maxHP;

            // NavMesh setup
            agent.radius = archetype.agentRadius;
            agent.height = archetype.agentHeight;
            agent.avoidancePriority = archetype.avoidancePriority;
            agent.autoBraking = false;

            // feed base speed to your mover
            if (mover) typeof(EnemyMoverNavmesh)
                .GetField("baseSpeed", System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance)
                ?.SetValue(mover, archetype.baseSpeed);

            // scale / visuals
            transform.localScale = archetype.scale;
            if (archetype.visualPrefab)
            {
                var vis = Instantiate(archetype.visualPrefab, transform);
                vis.transform.localPosition = Vector3.zero;
            }

            // wire behavior components
            if (archetype.behavior == EnemyBehavior.MeleeCharger)
            {
                if (melee == null) melee = gameObject.AddComponent<EnemyMelee>();
                melee.damage = archetype.contactDamage;
                melee.attacksPerSecond = archetype.attacksPerSecond;
                melee.range = archetype.attackRange;
                if (spitter) spitter.enabled = false;
                melee.enabled = true;
            }
            else
            {
                if (spitter == null) spitter = gameObject.AddComponent<EnemyRangedSpitter>();
                spitter.damage = archetype.contactDamage;
                spitter.attacksPerSecond = archetype.attacksPerSecond;
                spitter.range = Mathf.Max(archetype.attackRange, 7f);
                if (melee) melee.enabled = false;
                spitter.enabled = true;
            }
        }

        // Simple damage with armor & slow resistance hooks
        public void TakeDamage(float amount)
        {
            float reduced = Mathf.Max(0f, amount * (1f - archetype.flatArmor));
            HP -= reduced;
            if (HP <= 0f) Die();
        }

        public float ApplySlowMultiplier(float slow)
        {
            // e.g., slow=0.6 means 40% slow; resist <1 weakens slow
            return Mathf.Lerp(1f, slow, archetype.slowResist);
        }

        private void Die()
        {
            // TODO: ResourceBank.AddATP(archetype.atpRewardOnDeath);
            if (archetype.deathSfx) AudioSource.PlayClipAtPoint(archetype.deathSfx, transform.position);
            Destroy(gameObject);
        }
    }
}
