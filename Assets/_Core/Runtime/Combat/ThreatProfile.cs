using Core.Structures;
using UnityEngine;
using System;

namespace Core.Combat{
    public class ThreatProfile : MonoBehaviour, IThreatSource, IHittable
    {
        [SerializeField] private Team team = Team.Player;
        [SerializeField] private ThreatClass threatClass = ThreatClass.Tower;
        [SerializeField][Range(0, 100)] private int staticPriority = 10;
  
        public bool IsAlive = true;
        private StructureHealth structureHealth;

        // Explicit interface implementation for IHittable
        bool IHittable.IsAlive => IsAlive;

        void Awake()
        {
            structureHealth = this.gameObject.GetComponent<StructureHealth>();
            IsAlive = structureHealth.IsAlive;

        }

        public Team Team => team;
        public ThreatClass Class => threatClass;
        public int StaticPriority => staticPriority;

        public static event Action<ThreatProfile> OnAnyThreatDestroyed;

        void OnDestroy()
        {
            OnAnyThreatDestroyed?.Invoke(this);
        }
        

        public void TakeDamage(float amount)
        {
            structureHealth.TakeDamage(amount);
        }
    }
}