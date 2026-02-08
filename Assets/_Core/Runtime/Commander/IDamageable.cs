using UnityEngine;

namespace Core.Commander.Targeting
{
    public interface IDamageable
    {
        Transform Transform {get;}
        bool IsAlive {get;}

        void TakeDamage(float amount);
    }
}