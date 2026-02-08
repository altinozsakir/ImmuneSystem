using Core.Commander.Common;

namespace Core.Commander.Common
{
    public struct CommanderCooldowns
    {
        public Cooldown Dash;
        public Cooldown Attack;
        public Cooldown Build;
        public Cooldown Repair;

        public void ForceReadyAll(float now)
        {
            Dash.ForceReady(now);
            Attack.ForceReady(now);
            Build.ForceReady(now);
            Repair.ForceReady(now);
        }
    }
}