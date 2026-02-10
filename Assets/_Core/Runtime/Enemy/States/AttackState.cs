using UnityEngine;
using Core.Enemy.FSM;

namespace Core.Enemy.States
{
    public class AttackState : IEnemyState
    {
        public EnemyStateId Id => EnemyStateId.Attack;

        private float strikeAt;
        private float endAt;
        private bool didStrike;

        public void Enter(ref EnemyContext ctx)
        {
            ctx.Motor.Stop();

            float windup = ctx.Config.attackWindup;
            float recover = ctx.Config.attackRecover;

            strikeAt = ctx.Now + windup;
            endAt = strikeAt + recover;
            didStrike = false;
        }

        public void Tick(ref EnemyContext ctx)
        {
            // If target moved away, chase again
            var targetGO = ctx.BB.CurrentTargetGO;
            if (targetGO == null)
            {
                ctx.BB.Request(EnemyStateId.MoveToTarget);
                return;
            }

            Vector3 targetPos = targetGO.transform.position;
            Vector3 a = ctx.Transform.position; a.y = 0f;
            Vector3 b = targetPos; b.y = 0f;
            float range = ctx.Config.attackRange;

            if ((b - a).sqrMagnitude > range * range * 1.2f) // slight hysteresis
            {
                ctx.BB.Request(EnemyStateId.MoveToTarget);
                return;
            }

            // cooldown gate
            if (!ctx.Cooldowns.Attack.IsReady(ctx.Now))
            {
                ctx.BB.Request(EnemyStateId.MoveToTarget);
                return;
            }

            // strike moment
            if (!didStrike && ctx.Now >= strikeAt)
            {
                didStrike = true;

                // SIM: try call TakeDamage(float) if present
                var dmg = targetGO.GetComponentInParent<MonoBehaviour>();
                // Better: a known component like DummyDamageable. For now:
                var dummy = targetGO.GetComponentInParent<Core.Commander.DummyDamageable>();
                if (dummy != null)
                    dummy.TakeDamage(ctx.Config.attackDamage);
                else
                    Debug.Log($"[Enemy] Hit {targetGO.name} for {ctx.Config.attackDamage} (no damage component found).");
            }

            if (ctx.Now >= endAt)
            {
                ctx.Cooldowns.Attack.Arm(ctx.Now);
                ctx.BB.Request(EnemyStateId.MoveToTarget);
            }
        }

        public void Exit(ref EnemyContext ctx)
        {
            // resume moving on next state
        }
    }
}
