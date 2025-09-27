using UnityEngine;
using Core.Combat;
using Core.Pathing;

namespace Core.Enemies
{
    /// Light glue script: disables the enemy on death.
    [RequireComponent(typeof(Health))]
    [RequireComponent(typeof(PathFollower))]
    public class EnemyAgent : MonoBehaviour
    {
        Health _hp;

        void Awake()
        {
            _hp = GetComponent<Health>();
            _hp.onDeath.AddListener(OnDeath);
        }

        void OnDeath()
        {
            // Pool-friendly: just deactivate
            gameObject.SetActive(false);
        }
    }
}
