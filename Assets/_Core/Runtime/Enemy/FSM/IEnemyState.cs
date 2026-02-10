namespace Core.Enemy.FSM
{
    public interface IEnemyState
    {
        EnemyStateId Id { get; }
        void Enter(ref EnemyContext ctx);
        void Tick(ref EnemyContext ctx);
        void Exit(ref EnemyContext ctx);
    }
}
