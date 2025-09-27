using UnityEngine;
using UnityEngine.Events;


namespace Core.Combat
{
    /// HP with resistance and mark/neutralize integration.
    public class Health : MonoBehaviour
    {
        [Min(1f)] public float maxHP = 10f;
        [Range(0f, 0.95f)] public float resist = 0f; // % damage reduction


        public UnityEvent<float> onDamaged; // remaining HP
        public UnityEvent onDeath;


        float _hp; public float Current => _hp; public float Fraction => _hp / Mathf.Max(0.0001f, maxHP);
        StatusEffects _status;


        void Awake() { _status = GetComponent<StatusEffects>(); }
        void OnEnable() { _hp = maxHP; }


        public void ApplyDamage(in DamagePacket packet)
        {

            if (_hp <= 0f) return;


            if (packet.execute)
            {
                _hp = 0f; onDamaged?.Invoke(_hp); onDeath?.Invoke(); return;
            }


            // Order: resist -> mark multiplier -> HP
            float dmg = Mathf.Max(0f, packet.amount);
            dmg *= (1f - resist);
            if (_status) dmg *= _status.DamageTakenMultiplier();


            _hp -= dmg;
            Core.VFX.DamagePopupSpawner.Spawn(
                dmg,
                Mathf.Max(0f, _hp),
                maxHP,
                packet.hitPoint != Vector3.zero ? packet.hitPoint : transform.position,
                packet.execute
            );
            if (packet.markStacksToAdd > 0 && _status) _status.AddMarks(packet.markStacksToAdd);
            if (packet.neutralizeStacksToAdd > 0 && _status) _status.AddNeutralize(packet.neutralizeStacksToAdd);


            onDamaged?.Invoke(Mathf.Max(0f, _hp));
            if (_hp <= 0f) onDeath?.Invoke();

        }
    }
}