namespace RpgGame.Model.Game.Behaviors;

public sealed class EnemyBehaviorDecision
{
    private EnemyBehaviorDecision(int deltaX, int deltaY, int? attackPlayerId)
    {
        DeltaX = deltaX;
        DeltaY = deltaY;
        AttackPlayerId = attackPlayerId;
    }

    public int DeltaX { get; }
    public int DeltaY { get; }
    public int? AttackPlayerId { get; }
    public bool ShouldMove => DeltaX != 0 || DeltaY != 0;
    public bool ShouldAttack => AttackPlayerId.HasValue;

    public static EnemyBehaviorDecision None()
    {
        return new EnemyBehaviorDecision(0, 0, null);
    }

    public static EnemyBehaviorDecision Move(int deltaX, int deltaY)
    {
        return new EnemyBehaviorDecision(deltaX, deltaY, null);
    }

    public static EnemyBehaviorDecision Attack(int playerId)
    {
        return new EnemyBehaviorDecision(0, 0, playerId);
    }
}
