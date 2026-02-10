using UnityEngine;
using Core.Enemy.FSM;

namespace Core.Enemy.States
{
    public class MoveToTargetState : IEnemyState
    {
        public EnemyStateId Id => EnemyStateId.MoveToTarget;

        public void Enter(ref EnemyContext ctx)
        {
            ctx.BB.StuckTimer = 0f;
        }

        public void Tick(ref EnemyContext ctx)
        {
            // Choose target: nearest damageable in aggro radius else objective
            var nearest = ctx.Targeting.FindNearestDamageable(ctx.Config.aggroRadius);
            ctx.BB.CurrentTargetGO = nearest ? nearest : (ctx.BB.Objective ? ctx.BB.Objective.gameObject : null);

            Vector3 targetPos;
            if (ctx.BB.CurrentTargetGO != null) targetPos = ctx.BB.CurrentTargetGO.transform.position;
            else targetPos = ctx.Transform.position;

            ctx.BB.LastTargetPos = targetPos;

            // pathing
            ctx.Motor.SetDestination(targetPos);

            // transition to attack if close enough
            float range = ctx.Config.attackRange;
            Vector3 a = ctx.Transform.position; a.y = 0f;
            Vector3 b = targetPos; b.y = 0f;
            if ((b - a).sqrMagnitude <= range * range)
            {
                ctx.BB.Request(EnemyStateId.Attack);
                return;
            }

            // stuck detection (simple)
            if (ctx.Motor.Velocity.sqrMagnitude < 0.01f && ctx.Motor.HasPath)
                ctx.BB.StuckTimer += ctx.Dt;
            else
                ctx.BB.StuckTimer = 0f;

            if (ctx.BB.StuckTimer > 1.0f)
            {
                // forced repath / retarget by clearing current target
                ctx.BB.CurrentTargetGO = null;
                ctx.BB.StuckTimer = 0f;
            }
        }

        public void Exit(ref EnemyContext ctx) { }
    }
}
