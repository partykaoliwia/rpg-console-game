namespace RpgGame.Model.Game.Behaviors;

public interface IEnemyBehavior
{
    string Name { get; }

    EnemyBehaviorDecision Decide(Enemy enemy, EnemyBehaviorContext context);
}
