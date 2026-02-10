using System;
using Core.Enemy.FSM;
using UnityEngine;


namespace Core.Enemy.Common
{
    [Serializable]
    public struct EnemyBlackboard
    {
        public Transform Objective;
        public GameObject CurrentTargetGO;
        public Vector3 LastTargetPos;

        public float StuckTimer;
        public float LastRepathTime;

        public EnemyStateId RequestedState;
        public bool HasRequestedState;

        public void Request(EnemyStateId next)
        {
            RequestedState = next;
            HasRequestedState = true;
        }

        public void ClearRequest() => HasRequestedState = false;
    }
    
}