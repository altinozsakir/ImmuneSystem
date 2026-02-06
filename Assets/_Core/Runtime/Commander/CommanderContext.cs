using UnityEngine;

using Core.Commander.Common;
using Core.Commander.Input;


namespace Core.Commander
{
    
    public struct CommanderContext
    {
        public Transform Transform;
        public CharacterController CharacterController;
        public CommanderConfig CommanderConfig;

        public CommanderLocks CommanderLocks;

        public CommanderInputSnapshot CommanderInput;

        public float Now; // Time.time
        public float Dt; // Time.deltaTime

    }

}
