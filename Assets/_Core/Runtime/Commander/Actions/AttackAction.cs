using Core.Commander.FSM;
using Core.Commander.Targeting;

namespace Core.Commander.Actions
{
    
    public class AttackAction : ICommanderAction
    {
        
        public CommanderActionId Id => CommanderActionId.Attack;

        private float strikeAt;
        private float endAt;

        private bool didStrike;

        private IDamageable target;

        public bool CanStart(in CommanderContext ctx)
        {
            if(!ctx.Cooldowns.Attack.IsReady(ctx.Now)) return false;
            if(ctx.CommanderConfig == null) return true;

            var t = ctx.TargetingService != null ? ctx.TargetingService.FindNearestDamageable(ctx.CommanderConfig.attackRange) : null;
            return t != null;
        }

        public void Start(ref CommanderContext ctx)
        {
            ctx.CommanderLocks.MovementLocked = true;
            ctx.CommanderLocks.ActionLocked = true;

            float windup = ctx.CommanderConfig != null ? ctx.CommanderConfig.attackWindup : 0.12f;
            float recover = ctx.CommanderConfig != null ? ctx.CommanderConfig.attackRecover : 0.18f;

            strikeAt = ctx.Now + windup;
            endAt = strikeAt + recover;

            didStrike = false;

            target = ctx.TargetingService != null && ctx.CommanderConfig != null ? ctx.TargetingService.FindNearestDamageable(ctx.CommanderConfig.attackRange) : null;
        }

        public void Tick(ref CommanderContext ctx)
        {
            if(!didStrike && ctx.Now >= strikeAt)
            {
                didStrike = true;

                if(target != null && target.IsAlive && ctx.CommanderConfig != null)
                    target.TakeDamage(ctx.CommanderConfig.attackDamage);
            }
        }

        public bool IsFinished(in CommanderContext ctx) => ctx.Now >= endAt;

        public void Finish(ref CommanderContext ctx)
        {
            ctx.CommanderLocks.MovementLocked = false;
            ctx.CommanderLocks.ActionLocked = false;

            ctx.Cooldowns.Attack.Arm(ctx.Now);
        }

        public bool CanCancel(in CommanderContext ctx) => false;

        public void Cancel(ref CommanderContext ctx) {}

    }
}