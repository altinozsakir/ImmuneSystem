using UnityEngine;
using Core.Combat;

namespace Core.Structures
{
    [RequireComponent(typeof(Health))]
    public class Commander : MonoBehaviour
    {
        public System.Action OnDefeat; // hook your game over UI here
        Health _hp;

        void Awake(){ _hp = GetComponent<Health>(); }
        void OnEnable(){ _hp.onDeath.AddListener(Defeated); }
        void OnDisable(){ _hp.onDeath.RemoveListener(Defeated); }

        void Defeated()
        {
            Debug.LogWarning("[Commander] Defeated!");
            OnDefeat?.Invoke();
        }
    }
}
