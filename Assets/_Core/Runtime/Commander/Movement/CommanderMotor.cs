using UnityEngine;

namespace Core.Commander.Movement
{

    public class CommanderMotor
    {
        private Vector3 lastNonZeroMove = Vector3.forward;

        public Vector3 LastMoveDirection => lastNonZeroMove;

        public void TickMove(ref CommanderContext ctx)
        {
            if(ctx.CommanderLocks.MovementLocked) return;

            Vector2 m = ctx.CommanderInput.Move;
            Vector3 move3 = new Vector3(m.x, 0f, m.y);

            if(move3.sqrMagnitude > 0.00001f)
            {
                lastNonZeroMove = move3.normalized;
            }

            float speed = ctx.CommanderConfig != null ? ctx.CommanderConfig.moveSpeed : 6;
            Vector3 delta = move3 * (speed * ctx.Dt);

            if(ctx.CharacterController != null)
            {
                ctx.CharacterController.Move(delta);
            }
            else
            {
                ctx.Transform.position += delta;
            }
        }

        public void MoveRaw(ref CommanderContext ctx, Vector3 worldDelta)
        {
            if(ctx.CharacterController != null)
            {
                ctx.CharacterController.Move(worldDelta);
            }
            else
            {
                ctx.Transform.position += worldDelta;
            }
        }
    }   
}