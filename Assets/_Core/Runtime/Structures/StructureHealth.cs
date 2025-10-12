using Core.Combat;
using UnityEngine;
using System;

namespace Core.Structures
{
    public class StructureHealth : MonoBehaviour, IHittable
    {
        [SerializeField] private float maxHP = 200f;
        [SerializeField] private string onDeathMessage = "Structure destroyed!";
        public float hp;
        public event Action<StructureHealth> OnDied;

        public bool IsAlive { get; private set; } = true;

        void Awake() => hp = maxHP;

        public void TakeDamage(float amount)
        {
            hp -= Mathf.Max(0f, amount);
            if (hp <= 0f)
            {
                Debug.LogWarning(onDeathMessage);
                IsAlive = false;
                OnDied?.Invoke(this);
                enabled = false;
                Destroy(this.gameObject);
            }
        }
    }
}