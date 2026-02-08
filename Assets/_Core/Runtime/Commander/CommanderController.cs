using UnityEngine;
using Core.Commander.Input;
using Core.Commander;



public class CommanderController : MonoBehaviour
{
    private CommanderInstaller installer;


    private void Awake()
    {
        installer = GetComponent<CommanderInstaller>();
    }

    private void Update()
    {
        CommanderInputSnapshot snapshot = installer.InputReader.ConsumeSnapshot();
        CommanderContext ctx = installer.BuildContext(snapshot);




        if (snapshot.DashPressed)
        {
            installer.FSM.TryStart(ref ctx, installer.DashAction, allowInterrupt: true);
        }

        if (snapshot.AttackPressed)
        {
            installer.FSM.TryStart(ref ctx, installer.AttackAction, allowInterrupt: true);
        }

        if (snapshot.BuildPressed)
            installer.FSM.TryStart(ref ctx, installer.BuildModeAction, allowInterrupt: true);






        installer.FSM.Tick(ref ctx);

        ctx.CommanderMotor.TickMove(ref ctx);

        installer.CommitFromContext(in ctx);

    }
}