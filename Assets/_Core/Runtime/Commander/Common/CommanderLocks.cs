namespace Core.Commander.Common
{
    public struct CommanderLocks
    {
        public bool MovementLocked;
        public bool ActionLocked;
        public bool RotationLocked;

        public void ClearAll()
        {
            MovementLocked = false;
            ActionLocked = false;
            RotationLocked = false;
        }
    }
}