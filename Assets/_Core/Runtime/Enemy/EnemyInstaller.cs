using UnityEngine;
using UnityEngine.AI;
using Core.Enemy.Common;
using Core.Enemy.FSM;
using Core.Enemy.Movement;
using Core.Enemy.Perception;
using Core.Enemy.States;
using Core.Commander.Common; // Cooldown

namespace Core.Enemy
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class EnemyInstaller : MonoBehaviour
    {
        [SerializeField] private EnemyConfig config;
        [SerializeField] private Transform objective; // assign base/core/commander

        public EnemyStateMachine FSM { get; private set; }

        private NavMeshAgent agent;
        private EnemyMotor motor;
        private EnemyTargetingService targeting;

        private EnemyBlackboard bb;
        private EnemyCooldowns cooldowns;

        private SpawnState spawn;
        private MoveToTargetState move;
        private AttackState attack;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();

            motor = new EnemyMotor(agent);
            targeting = new EnemyTargetingService(transform, config ? config.damageableMask : ~0);

            // persistent runtime state
            bb = default;
            bb.Objective = objective;

            cooldowns = default;
            cooldowns.Attack = new Cooldown { Duration = config ? config.attackCooldown : 0.6f };
            cooldowns.Attack.ForceReady(Time.time);

            // states + fsm
            spawn = new SpawnState();
            move = new MoveToTargetState();
            attack = new AttackState();

            FSM = new EnemyStateMachine();
            FSM.Register(spawn);
            FSM.Register(move);
            FSM.Register(attack);

            // start in Move (Spawn can be used later for delays)
            var ctx = BuildContext();
            FSM.Start(ref ctx, EnemyStateId.MoveToTarget);
            CommitFromContext(in ctx);
        }

        public EnemyContext BuildContext()
        {
            return new EnemyContext
            {
                Transform = transform,
                Agent = agent,
                Config = config,
                Motor = motor,
                Targeting = targeting,
                BB = bb,
                Cooldowns = cooldowns,
                Now = Time.time,
                Dt = Time.deltaTime
            };
        }

        public void CommitFromContext(in EnemyContext ctx)
        {
            bb = ctx.BB;
            cooldowns = ctx.Cooldowns;
        }

        public void SetObjective(Transform obj)
        {
            objective = obj;
            bb.Objective = obj;
        } 
    }
}
