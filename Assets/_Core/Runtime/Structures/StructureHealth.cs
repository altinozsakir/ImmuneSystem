using Core.Combat;
using UnityEngine;

namespace Core.Structures
{
public class StructureHealth : MonoBehaviour, IHittable
{
    [SerializeField] private float maxHP = 200f;
    [SerializeField] private string onDeathMessage = "Structure destroyed!";
    private float hp;

        public bool IsAlive { get; private set; } = false;

        void Awake() => hp = maxHP;

    public void TakeDamage(float amount)
    {
        hp -= Mathf.Max(0f, amount);
        if (hp <= 0f)
        {
            Debug.Log(onDeathMessage);
            IsAlive = true;
            enabled = false;
        }
    }
}
}