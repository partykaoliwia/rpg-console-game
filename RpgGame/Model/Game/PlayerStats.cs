namespace RpgGame.Model.Game;

public readonly struct PlayerStats
{
    public int Strength { get; init; }
    public int Health { get; init; }
    public int Luck { get; init; }
    public int Aggression { get; init; }
    public int Wisdom { get; init; }
    public int Dexterity { get; init; }

    public static PlayerStats operator +(PlayerStats a, PlayerStats b)
    {
        return new PlayerStats
        {
            Strength = a.Strength + b.Strength,
            Health = a.Health + b.Health,
            Luck = a.Luck + b.Luck,
            Aggression = a.Aggression + b.Aggression,
            Wisdom = a.Wisdom + b.Wisdom,
            Dexterity = a.Dexterity + b.Dexterity
        };
    }
}
