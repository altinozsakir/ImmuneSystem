using System.Collections.Generic;

namespace Core.Enemy.FSM
{
    public class EnemyStateMachine
    {
        private readonly Dictionary<EnemyStateId, IEnemyState> states = new();
        private IEnemyState current;

        public EnemyStateId CurrentId => current != null ? current.Id : EnemyStateId.Spawn;

        public void Register(IEnemyState state) => states[state.Id] = state;

        public void Start(ref EnemyContext ctx, EnemyStateId start)
        {
            current = states[start];
            current.Enter(ref ctx);
        }

        public void Tick(ref EnemyContext ctx) => current?.Tick(ref ctx);

        public void Change(ref EnemyContext ctx, EnemyStateId next)
        {
            if (current != null && current.Id == next) return;
            current?.Exit(ref ctx);
            current = states[next];
            current.Enter(ref ctx);
        }
    }
}
