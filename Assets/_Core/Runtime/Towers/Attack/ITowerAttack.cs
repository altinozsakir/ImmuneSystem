
using Core.Towers;

namespace Core.Towers.Attack
{
    public interface ITowerAttack
    {
        void Tick(ref TowerContext ctx);
    }
}