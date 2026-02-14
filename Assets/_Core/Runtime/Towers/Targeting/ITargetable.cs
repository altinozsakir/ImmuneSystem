using UnityEngine;


namespace Core.Towers.Targeting
{
    public interface ITargetable
    {
        Transform Transform{get;}
        bool IsAlive {get;}
    }
}