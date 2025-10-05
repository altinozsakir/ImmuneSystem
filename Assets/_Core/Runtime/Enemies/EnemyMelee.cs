using UnityEngine;

namespace Core.Enemies
{
    public class EnemyMelee : MonoBehaviour
    {
        public float range = 1.0f;
        public float attacksPerSecond = 1f;
        public float damage = 3f;

        float _cooldown;
        Transform _goal; // assign from spawner or a GoalZone

        public void SetGoal(Transform t) => _goal = t;

        void Update()
        {
            if (!_goal) return;
            var d = Vector3.Distance(transform.position, _goal.position);
            _cooldown -= Time.deltaTime;
            if (d <= range && _cooldown <= 0f)
            {
                // TODO: call GoalZone.TakeDamage(damage);
                _cooldown = 1f / attacksPerSecond;
            }
        }
    }
}
