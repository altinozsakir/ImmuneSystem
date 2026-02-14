using Core.Commander.Common;
using Core.Towers.Attack;
using Core.Towers.Common;
using Core.Towers.Targeting;
using Core.Towsers.Targeting;
using UnityEngine;


namespace Core.Towers
{
    public class TowerInstaller : MonoBehaviour
    {
        
        [SerializeField] private TowerConfig config;
        
        private ITowerTargeting targeting;
        private ITowerAttack attack;
        private TowerCooldowns cooldowns;
        public TowerConfig Config => config;

        private void Awake()
        {
            cooldowns= default;
            cooldowns.Fire = new Cooldown{Duration=config ? config.fireCooldown: 0.7f};
            cooldowns.Fire.ForceReady(Time.time);

            targeting = new NearestTargeting();
            attack = new ProjectileAttack();
        }

                public TowerContext BuildContext()
        {
            return new TowerContext
            {
                Transform = transform,
                Config = config,
                Target = null,
                Cooldowns = cooldowns,
                Now = Time.time,
                Dt = Time.deltaTime
            };
        }

        public void Commit(in TowerContext ctx)
        {
            cooldowns = ctx.Cooldowns;
        }

        public ITowerTargeting Targeting => targeting;
        public ITowerAttack Attack => attack;

    }
}