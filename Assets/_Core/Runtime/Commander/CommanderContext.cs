using UnityEngine;

using Core.Commander.Common;
using Core.Commander.Input;
using Core.Commander.Movement;
using Core.Commander.Build;


namespace Core.Commander
{
    
    public struct CommanderContext
    {
        public Transform Transform;
        public CharacterController CharacterController;
        public CommanderConfig CommanderConfig;

        public CommanderLocks CommanderLocks;

        public CommanderInputSnapshot CommanderInput;

        public CommanderCooldowns Cooldowns;

        public CommanderMotor CommanderMotor;

        public BuildPlacementService BuildPlacementService;

        public CommanderTargetingService TargetingService;
        public float Now; // Time.time
        public float Dt; // Time.deltaTime

    }

}
