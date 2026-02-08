using Core.Commander.Targeting;
using UnityEngine;

namespace Core.Commander
{
    public class DummyDamageable: MonoBehaviour, IDamageable
    {
        [SerializeField] private float hp = 50f;

        public Transform Transform => transform;
        public bool IsAlive => hp > 0f;

        public void TakeDamage(float amount)
        {
            if(!IsAlive) return;

            hp -= Mathf.Max(0f, amount);
            Debug.Log($"{name} took {amount} dmg. HP={hp}");

            if (hp <= 0f)
                Debug.Log($"{name} died.");
        }
    }
}