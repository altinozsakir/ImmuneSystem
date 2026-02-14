using Core.Towers;

namespace Core.Towers.Targeting
{
    public interface ITowerTargeting
    {
        ITargetable Acquire(in TowerContext ctx );
    }
}