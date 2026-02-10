using UnityEngine;
using UnityEngine.AI;
using Core.Enemy.Common;
using Core.Enemy.Movement;
using Core.Enemy.Perception;

namespace Core.Enemy
{
    public struct EnemyContext
    {
        public Transform Transform;
        public NavMeshAgent Agent;
        public EnemyConfig Config;

        public EnemyMotor Motor;
        public EnemyTargetingService Targeting;

        public EnemyBlackboard BB;
        public EnemyCooldowns Cooldowns;

        public float Now;
        public float Dt;
    }
}