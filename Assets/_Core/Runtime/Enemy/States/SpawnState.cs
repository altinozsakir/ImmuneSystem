using Core.Enemy.FSM;

namespace Core.Enemy.States
{
    public class SpawnState : IEnemyState
    {
        public EnemyStateId Id => EnemyStateId.Spawn;

        public void Enter(ref EnemyContext ctx) { }

        public void Tick(ref EnemyContext ctx)
        {
            ctx.Motor.Configure(ctx.Config.moveSpeed, ctx.Config.stoppingDistance);
            ctx.Motor.SetDestination(ctx.BB.Objective ? ctx.BB.Objective.position : ctx.Transform.position);
            // transition
            // (installer will hold FSM; we’ll switch via a callback in controller)
        }

        public void Exit(ref EnemyContext ctx) { }
    }
}