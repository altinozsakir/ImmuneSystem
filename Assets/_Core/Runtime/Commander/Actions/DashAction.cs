using UnityEngine;
using Core.Commander.Common;
using Core.Commander.FSM;


namespace Core.Commander.Actions
{
    public class DashAction : ICommanderAction
    {
        public CommanderActionId Id => CommanderActionId.Dash;

        private float endAt;
        private Vector3 dashDir;

        public bool CanStart(in CommanderContext ctx)
        {
            return ctx.Cooldowns.Dash.IsReady(ctx.Now);
        }

        public void Start(ref CommanderContext ctx)
        {
            ctx.CommanderLocks.MovementLocked = true;
            ctx.CommanderLocks.ActionLocked= true;

            Vector2 m = ctx.CommanderInput.Move;

            Vector3 move3 = new Vector3(m.x,0f,m.y);

            if(move3.sqrMagnitude > 0.00001f)
            {
                dashDir = move3.normalized;
            }
            else{
                dashDir = ctx.CommanderMotor.LastMoveDirection.normalized;
            }

            float dur = ctx.CommanderConfig != null ? ctx.CommanderConfig.dashDuration : 0.18f;
            endAt = ctx.Now + dur;
        }


        public void Tick(ref CommanderContext ctx)
        {
            float speed = ctx.CommanderConfig != null ? ctx.CommanderConfig.dashSpeed : 14f;
            Vector3 delta = dashDir * (speed * ctx.Dt);
            ctx.CommanderMotor.MoveRaw(ref ctx, delta);
        }

        public bool IsFinished(in CommanderContext ctx) => ctx.Now >= endAt;

        public void Finish(ref CommanderContext ctx)
        {
            ctx.CommanderLocks.MovementLocked = false;
            ctx.CommanderLocks.RotationLocked = false;

            ctx.Cooldowns.Dash.Arm(ctx.Now);
        }

        public bool CanCancel(in CommanderContext ctx) => false;

        public void Cancel(ref CommanderContext ctx){}
    }
}