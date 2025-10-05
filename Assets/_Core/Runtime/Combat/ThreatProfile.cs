using UnityEngine;

namespace Core.Combat{
    public class ThreatProfile : MonoBehaviour, IThreatSource
    {
        [SerializeField] private Team team = Team.Player;
        [SerializeField] private ThreatClass threatClass = ThreatClass.Tower;
        [SerializeField][Range(0, 100)] private int staticPriority = 10;
        [SerializeField] private float maxHP = 100f;

        private float hp;

        void Awake() { hp = maxHP; }

        public Team Team => team;
        public ThreatClass Class => threatClass;
        public int StaticPriority => staticPriority;
        public bool IsAlive => hp > 0f;

        public void TakeDamage(float amount)
        {
            if (!IsAlive) return;
            hp -= Mathf.Max(0f, amount);
            if (hp <= 0f)
            { 
                
            }
        }
    }
}