namespace RpgGame.Model.Game.Behaviors;

public sealed class EnemyBehaviorProfile
{
    public EnemyBehaviorProfile(
        IEnemyBehavior defaultBehavior,
        IEnemyBehavior soundBehavior,
        IEnemyBehavior sightBehavior)
    {
        DefaultBehavior = defaultBehavior;
        SoundBehavior = soundBehavior;
        SightBehavior = sightBehavior;
    }

    public IEnemyBehavior DefaultBehavior { get; }
    public IEnemyBehavior SoundBehavior { get; }
    public IEnemyBehavior SightBehavior { get; }
}
