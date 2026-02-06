namespace Core.Commander.FSM
{
    public interface ICommanderAction
    {
        CommanderActionId Id {get;}

        bool CanStart(in CommanderContext ctx);
        void Start(ref CommanderContext ctx);
        void Tick(ref CommanderContext ctx);

        bool IsFinished(in CommanderContext ctx);

        void Finish(ref CommanderContext ctx);

        bool CanCancel(in CommanderContext ctx);

        void Cancel(ref CommanderContext ctx);

    }
}