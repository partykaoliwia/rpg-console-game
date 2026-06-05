using RpgGame.Model.Global;
using RpgGame.Model.Game.Behaviors;

namespace RpgGame.Model.Game;

public sealed class Giant : Enemy
{
    public Giant(int id, int x, int y)
        : this(id, "Giant", 45, 18, 5, x, y)
    {
    }

    public Giant(int id, string name, int health, int strength, int armor, int x, int y)
        : base(id, name, health, strength, armor, x, y)
    {
        MoveEveryMs = 1500;
        ConfigureBehaviorProfile(EnemyBehaviors.CowardlyProfile());
    }

    public override void Accept(ICharacterVisitor visitor)
    {
        visitor.Visit(this);
    }

    public override void UpdateOnSound(int sourceX, int sourceY, int distance, GameEventLog eventLog)
    {
        RememberSound(sourceX, sourceY);
        eventLog.Add($"{Name} at ({X}, {Y}) heard noise from ({sourceX}, {sourceY}). Distance: {distance}.");
    }

    public override void UpdateOnDeath(GameEventLog eventLog)
    {
        Strength = Math.Max(1, (int)(Strength * 0.33));
        eventLog.Add($"{Name} at ({X}, {Y}) lost courage after another giant died.");
    }
}
