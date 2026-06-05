using RpgGame.Model.Game.Observers;
using RpgGame.Model.Global;

namespace RpgGame.Model.Game;

public abstract class Enemy : ISpeciesSubscriber, ISoundSubscriber
{
    private SpeciesPublisher? _species;
    private SoundPublisher? _sound;

    protected Enemy(int id, string name, int health, int strength, int armor, int x, int y)
    {
        Id = id;
        Name = name;
        Health = health;
        Strength = strength;
        Armor = armor;
        X = x;
        Y = y;
    }

    public int Id { get; }
    public string Name { get; protected set; }
    public int Health { get; protected set; }
    public int Strength { get; protected set; }
    public int Armor { get; protected set; }
    public int X { get; private set; }
    public int Y { get; private set; }
    public bool IsAlive => Health > 0;
    public int MoveEveryMs { get; protected set; }
    public DateTime LastMovedUtc { get; set; } = DateTime.UtcNow;

    public void SetPosition(int x, int y)
    {
        X = x;
        Y = y;
    }

    public int TakeDamage(int damage)
    {
        if (!IsAlive)
        {
            return 0;
        }

        var actualDamage = Math.Max(0, damage - Armor);
        Health = Math.Max(0, Health - actualDamage);
        return actualDamage;
    }

    public void RestoreState(int x, int y, int health)
    {
        X = x;
        Y = y;
        Health = Math.Max(0, health);
    }

    public void RestoreState(int x, int y, int health, int strength, int armor)
    {
        X = x;
        Y = y;
        Health = Math.Max(0, health);
        Strength = Math.Max(0, strength);
        Armor = Math.Max(0, armor);
    }

    public void RestoreState(string name, int x, int y, int health, int strength, int armor)
    {
        Name = string.IsNullOrWhiteSpace(name) ? Name : name;
        RestoreState(x, y, health, strength, armor);
    }

    public void JoinObservers(SpeciesPublisher species, SoundPublisher sound)
    {
        LeaveObservers();
        _species = species;
        _sound = sound;
        _species.Subscribe(this);
        _sound.Subscribe(this);
    }

    public void LeaveObservers()
    {
        _species?.Unsubscribe(this);
        _sound?.Unsubscribe(this);
        _species = null;
        _sound = null;
    }

    public void Die(GameEventLog eventLog)
    {
        var species = _species;
        _species?.Unsubscribe(this);
        _sound?.Unsubscribe(this);
        _species = null;
        _sound = null;
        species?.NotifySubscribers(eventLog);
    }

    public abstract void UpdateOnSound(int sourceX, int sourceY, int distance, GameEventLog eventLog);
    public abstract void UpdateOnDeath(GameEventLog eventLog);
    public abstract void Accept(ICharacterVisitor visitor);
}
