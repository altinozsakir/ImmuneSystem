using Core.Commander.FSM;


namespace Core.Commander.Actions
{
    public class FreeAction : ICommanderAction
    {
        public CommanderActionId Id => CommanderActionId.Free;

        public bool CanCancel(in CommanderContext ctx)
        {
            return true;
        }

        public void Cancel(ref CommanderContext ctx)
        {
    
        }

        public bool CanStart(in CommanderContext ctx)
        {
            return true;
        }

        public void Finish(ref CommanderContext ctx)
        {
        }

        public bool IsFinished(in CommanderContext ctx)
        {
            return false;
        }

        public void Start(ref CommanderContext ctx)
        {

        }

        public void Tick(ref CommanderContext ctx)
        {

        }
    }
}