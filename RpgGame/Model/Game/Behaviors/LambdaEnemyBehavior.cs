namespace RpgGame.Model.Game.Behaviors;

public sealed class LambdaEnemyBehavior : IEnemyBehavior
{
    private readonly Func<Enemy, EnemyBehaviorContext, EnemyBehaviorDecision> _decide;

    public LambdaEnemyBehavior(
        string name,
        Func<Enemy, EnemyBehaviorContext, EnemyBehaviorDecision> decide)
    {
        Name = name;
        _decide = decide;
    }

    public string Name { get; }

    public EnemyBehaviorDecision Decide(Enemy enemy, EnemyBehaviorContext context)
    {
        return _decide(enemy, context);
    }
}
