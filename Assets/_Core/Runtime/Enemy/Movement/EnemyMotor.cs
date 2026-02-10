
using UnityEngine;
using UnityEngine.AI;

namespace Core.Enemy.Movement
{
    public class EnemyMotor
    {
        private readonly NavMeshAgent agent;

        public EnemyMotor(NavMeshAgent agent) => this.agent = agent;

        public void Configure(float speed, float stoppingDistance)
        {
            if(!agent) return;
            agent.speed = speed;
            agent.stoppingDistance = stoppingDistance;
            agent.updateRotation = true;
        }

        public void SetDestination(Vector3 pos)
        {
            if(!agent) return;
            agent.isStopped = false;
            agent.SetDestination(pos);
        }

        public void Stop()
        {
            if(!agent) return;
            agent.isStopped = true;
        }

        public bool HasArrived(float extra = 0f)
        {
            if(!agent) return true;
            if(agent.pathPending) return false;

            float dist = agent.remainingDistance;
            float threshold = agent.stoppingDistance + extra;
            return dist <= threshold;
        }

        public Vector3 Velocity => agent ? agent.velocity : Vector3.zero;

        public bool HasPath => agent && agent.hasPath;

        public NavMeshPathStatus PathStatus => agent ? agent.pathStatus : NavMeshPathStatus.PathInvalid;
    }
}