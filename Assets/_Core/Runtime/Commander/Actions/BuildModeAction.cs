using UnityEngine;
using Core.Commander.FSM;
using Core.Commander.Build;

namespace Core.Commander.Actions
{
    public class BuildModeAction : ICommanderAction
    {
        public CommanderActionId Id => CommanderActionId.BuildMode;

        private BuildGhostController ghost;
        private bool exitRequested;

        private Vector3 currentPoint;
        private bool hasPoint;
        private bool isValid;

        public bool CanStart(in CommanderContext ctx)
        {
            if (!ctx.Cooldowns.Build.IsReady(ctx.Now)) return false;
            if (ctx.CommanderConfig == null) return false;
            if (ctx.CommanderConfig.defaultBuildable == null || ctx.CommanderConfig.defaultBuildable.prefab == null) return false;
            if (ctx.BuildPlacementService == null) return false;
            return true;
        }

        public void Start(ref CommanderContext ctx)
        {
            exitRequested = false;
            hasPoint = false;
            isValid = false;

            // Modal: block other actions; allow movement (typical Thronefall)
            ctx.CommanderLocks.ActionLocked = true;

            ghost = new BuildGhostController(
                ctx.CommanderConfig.defaultBuildable.prefab,
                ctx.CommanderConfig.ghostValidMaterial,
                ctx.CommanderConfig.ghostInvalidMaterial
            );
        }

        public void Tick(ref CommanderContext ctx)
        {
            // Update snapped point
            hasPoint = ctx.BuildPlacementService.TryGetSnappedPoint(ctx.CommanderInput.PointerScreen, out currentPoint);

            if (hasPoint)
            {
                Debug.Log("logggg");
                isValid = ctx.BuildPlacementService.IsValidPlacement(currentPoint);
                ghost?.SetPose(currentPoint, Quaternion.identity);
                ghost?.SetValid(isValid);
            }

            // Cancel
            if (ctx.CommanderInput.CancelPressed)
            {
                exitRequested = true;
                return;
            }

            // Confirm (build)
            if (ctx.CommanderInput.ConfirmPressed && hasPoint && isValid)
            {
                ctx.BuildPlacementService.Build(ctx.CommanderConfig.defaultBuildable, currentPoint, Quaternion.identity);
                exitRequested = true;
            }
        }

        public bool IsFinished(in CommanderContext ctx) => exitRequested;

        public void Finish(ref CommanderContext ctx)
        {
            ghost?.Destroy();
            ghost = null;

            ctx.CommanderLocks.ActionLocked = false;

            // Only start cooldown if we actually entered build mode
            ctx.Cooldowns.Build.Arm(ctx.Now);
        }

        public bool CanCancel(in CommanderContext ctx) => true;
        public void Cancel(ref CommanderContext ctx) => exitRequested = true;
    }
}
