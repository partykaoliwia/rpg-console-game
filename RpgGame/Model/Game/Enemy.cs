using RpgGame.Model.Game.Observers;
using RpgGame.Model.Game.Behaviors;
using RpgGame.Model.Global;

namespace RpgGame.Model.Game;

public abstract class Enemy : ISpeciesSubscriber, ISoundSubscriber
{
    private SpeciesPublisher? _species;
    private SoundPublisher? _sound;
    private IEnemyBehavior _defaultBehavior = EnemyBehaviors.RandomWalk();
    private IEnemyBehavior _soundBehavior = EnemyBehaviors.RandomWalk();
    private IEnemyBehavior _sightBehavior = EnemyBehaviors.RandomWalk();
    private IEnemyBehavior _currentBehavior = EnemyBehaviors.RandomWalk();
    private int? _heardSoundX;
    private int? _heardSoundY;

    protected Enemy(int id, string name, int health, int strength, int armor, int x, int y)
    {
        Id = id;
        Name = name;
        Health = health;
        MaxHealth = health;
        Strength = strength;
        Armor = armor;
        X = x;
        Y = y;
    }

    public int Id { get; }
    public string Name { get; protected set; }
    public int Health { get; protected set; }
    public int MaxHealth { get; private set; }
    public int Strength { get; protected set; }
    public int Armor { get; protected set; }
    public int X { get; private set; }
    public int Y { get; private set; }
    public bool IsAlive => Health > 0;
    public int MoveEveryMs { get; protected set; }
    public int VisionRange { get; protected set; } = 6;
    public DateTime LastMovedUtc { get; set; } = DateTime.UtcNow;
    public bool HasHeardSound => _heardSoundX.HasValue && _heardSoundY.HasValue;
    public int HeardSoundX => _heardSoundX ?? X;
    public int HeardSoundY => _heardSoundY ?? Y;
    public string CurrentBehaviorName => _currentBehavior.Name;

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

    public virtual void UpdateAfterAttack(GameEventLog eventLog)
    {
    }

    public void RestoreState(int x, int y, int health)
    {
        X = x;
        Y = y;
        Health = Math.Max(0, health);
        MaxHealth = Math.Max(MaxHealth, Health);
    }

    public void RestoreState(int x, int y, int health, int strength, int armor)
    {
        X = x;
        Y = y;
        Health = Math.Max(0, health);
        MaxHealth = Math.Max(MaxHealth, Health);
        Strength = Math.Max(0, strength);
        Armor = Math.Max(0, armor);
    }

    public void RestoreState(string name, int x, int y, int health, int strength, int armor)
    {
        Name = string.IsNullOrWhiteSpace(name) ? Name : name;
        RestoreState(x, y, health, strength, armor);
    }

    public EnemyBehaviorDecision DecideBehavior(EnemyBehaviorContext context)
    {
        var visiblePlayers = context.FindVisiblePlayers(this);
        if (visiblePlayers.Count > 0)
        {
            ForgetSound();
            return UseBehavior(_sightBehavior, context);
        }

        if (HasHeardSound)
        {
            return UseBehavior(_soundBehavior, context);
        }

        return UseBehavior(_defaultBehavior, context);
    }

    public void ForgetSound()
    {
        _heardSoundX = null;
        _heardSoundY = null;
    }

    protected void ConfigureBehaviorProfile(EnemyBehaviorProfile profile)
    {
        _defaultBehavior = profile.DefaultBehavior;
        _soundBehavior = profile.SoundBehavior;
        _sightBehavior = profile.SightBehavior;
        _currentBehavior = _defaultBehavior;
    }

    protected void RememberSound(int sourceX, int sourceY)
    {
        _heardSoundX = sourceX;
        _heardSoundY = sourceY;
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

    private EnemyBehaviorDecision UseBehavior(IEnemyBehavior behavior, EnemyBehaviorContext context)
    {
        _currentBehavior = behavior;
        return behavior.Decide(this, context);
    }
}
