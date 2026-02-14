using Core.Towers.Targeting;
using Core.Towers.Common;
using UnityEngine;

namespace Core.Towers
{
    public struct TowerContext
    {
        public Transform Transform;
        public TowerConfig Config;

        public ITargetable Target;
        public TowerCooldowns Cooldowns;

        public float Now;
        public float Dt;
    }
}