using RpgGame.Model.Game.Behaviors;
using RpgGame.Model.Global;

namespace RpgGame.Model.Game;

public sealed class Satyr : Enemy
{
    public Satyr(int id, int x, int y)
        : this(id, "Satyr", 35, 14, 3, x, y)
    {
    }

    public Satyr(int id, string name, int health, int strength, int armor, int x, int y)
        : base(id, name, health, strength, armor, x, y)
    {
        MoveEveryMs = 800;
        ConfigureBehaviorProfile(EnemyBehaviors.NeutralProfile());
    }

    public override void Accept(ICharacterVisitor visitor)
    {
        visitor.Visit(this);
    }

    public override void UpdateOnSound(int sourceX, int sourceY, int distance, GameEventLog eventLog)
    {
        eventLog.Add($"{Name} at ({X}, {Y}) ignored noise from ({sourceX}, {sourceY}). Distance: {distance}.");
    }

    public override void UpdateAfterAttack(GameEventLog eventLog)
    {
        if (Health * 2 >= MaxHealth)
        {
            ConfigureBehaviorProfile(EnemyBehaviors.AggressiveProfile());
            eventLog.Add($"{Name} at ({X}, {Y}) became aggressive after being attacked.");
            return;
        }

        ConfigureBehaviorProfile(EnemyBehaviors.CowardlyProfile());
        eventLog.Add($"{Name} at ({X}, {Y}) panicked after being badly wounded.");
    }

    public override void UpdateOnDeath(GameEventLog eventLog)
    {
        eventLog.Add($"{Name} at ({X}, {Y}) stayed quiet after another satyr died.");
    }
}
