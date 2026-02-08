using UnityEngine;

using Core.Commander.Actions;
using Core.Commander.Common;
using Core.Commander.FSM;
using Core.Commander.Movement;
using Core.Commander.Input;
using UnityEditor.Build;
using Core.Commander.Build;


namespace Core.Commander
{
    [RequireComponent(typeof(CommanderInputReader))]
    public class CommanderInstaller:MonoBehaviour
    {
        [SerializeField] private CommanderConfig config;

        public CommanderConfig Config => config;
        public CommanderInputReader InputReader {get; private set;}
        public CommanderMotor Motor {get; private set;}
        public CommanderActionStateMachine FSM {get; private set;}

        public FreeAction FreeAction {get; private set;}
        public DashAction DashAction {get; private set;}

        public AttackAction AttackAction {get; private set;}

        public BuildModeAction BuildModeAction { get; private set; }
        private CommanderCooldowns cooldowns;
        private CommanderLocks locks;

        private GridService grid;
        private BuildPlacementService placement;

        private CharacterController cc;

        private CommanderTargetingService commanderTargetingService;

        private void Awake()
        {
            InputReader = GetComponent<CommanderInputReader>();
            cc = GetComponent<CharacterController>();
            Motor = new CommanderMotor();

            FreeAction = new FreeAction();
            DashAction = new DashAction();
            AttackAction = new AttackAction();
            BuildModeAction = new BuildModeAction();
            float now = Time.time;
            locks = default;
            locks.ClearAll();

            cooldowns = default;
            cooldowns.Dash.Duration   = config != null ? config.dashCooldown   : 0.8f;
            cooldowns.Attack.Duration = config != null ? config.attackCooldown : 0.35f;
            cooldowns.Repair.Duration = config != null ? config.repairCooldown : 0.25f;
            cooldowns.Build.Duration  = config != null ? config.buildCooldown  : 0.1f;
            cooldowns.ForceReadyAll(now);

            commanderTargetingService = new CommanderTargetingService(transform, config!= null ? config.damageableMask : ~0);

            grid = new GridService(config != null ? config.gridCellSize : 1f, Vector3.zero);
            var cam = Camera.main;
placement = new BuildPlacementService(
    transform,
    cam,
    grid,
    config != null ? config.groundMask : ~0,
    config != null ? config.blockingMask : 0,
    config != null ? config.blockingCheckRadius : 0.45f,
    config != null ? config.buildMaxRange : 8f
);
            FSM = new CommanderActionStateMachine(FreeAction);
            



            var ctx = BuildContext(default);
            FSM.TryStart(ref ctx, FreeAction, allowInterrupt: true);
            CommitFromContext(in ctx);
        }

        public CommanderContext BuildContext(CommanderInputSnapshot snapshot)
        {

            CommanderContext ctx = new CommanderContext()
            {
                Transform = transform,
                CharacterController = cc,
                CommanderConfig = Config,
                CommanderMotor = Motor,
                CommanderInput = snapshot,
                CommanderLocks = locks,
                TargetingService = commanderTargetingService,
                Cooldowns = cooldowns,
                BuildPlacementService = placement,
                Now = Time.time,
                Dt = Time.deltaTime
            };


            return ctx;
        }

            public void CommitFromContext(in CommanderContext ctx)
            {
                // Write back runtime state mutated by actions
                locks = ctx.CommanderLocks;
                cooldowns = ctx.Cooldowns;
            }

    }
}