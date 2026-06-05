using RpgGame.Model.Global;

namespace RpgGame.Model.Game;

public sealed class Harpy : Enemy
{
    public Harpy(int id, int x, int y)
        : this(id, "Harpy", 30, 12, 2, x, y)
    {
    }

    public Harpy(int id, string name, int health, int strength, int armor, int x, int y)
        : base(id, name, health, strength, armor, x, y)
    {
        MoveEveryMs = 400;
    }

    public override void Accept(ICharacterVisitor visitor)
    {
        visitor.Visit(this);
    }

    public override void UpdateOnSound(int sourceX, int sourceY, int distance, GameEventLog eventLog)
    {
        eventLog.Add($"{Name} at ({X}, {Y}) heard noise from ({sourceX}, {sourceY}). Distance: {distance}.");
    }

    public override void UpdateOnDeath(GameEventLog eventLog)
    {
        Strength *= 2;
        eventLog.Add($"{Name} at ({X}, {Y}) became enraged after another harpy died.");
    }
}
