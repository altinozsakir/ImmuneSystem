using UnityEngine;

namespace Core.Commander.FSM
{
    public class CommanderActionStateMachine
    {
        private ICommanderAction current;
        private readonly ICommanderAction free;
        
        public CommanderActionId CurrentId => current != null ? current.Id : CommanderActionId.Free;

        public CommanderActionStateMachine(ICommanderAction freeAction)
        {
            free = freeAction;
            current = free;
        }

        public void Tick(ref CommanderContext ctx)
        {
            if(current == null) current = free;

            current.Tick(ref ctx);

            if(current.IsFinished(in ctx))
            {
                current.Finish(ref ctx);
                current = free;
                current.Start(ref ctx);
            }
        }

        public bool TryStart(ref CommanderContext ctx, ICommanderAction next, bool allowInterrupt=true)
        {

            if(next==null) return false;
            if(!next.CanStart(in ctx)) return false;

            if(ctx.CommanderLocks.ActionLocked && allowInterrupt == false) 
                return false;

            if(allowInterrupt && current != null && current != free)
            {
                if(current.CanCancel(in ctx))
                    current.Cancel(ref ctx);
                current.Finish(ref ctx);
            }
            current = next;
            current.Start(ref ctx);
            
            return true;
            

        }

    }
}