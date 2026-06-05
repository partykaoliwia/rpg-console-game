namespace RpgGame.Dto;

public sealed class EnemyDto
{
    public int Id { get; set; }
    public EnemyKind Kind { get; set; }
    public string Name { get; set; } = "";
    public int X { get; set; }
    public int Y { get; set; }
    public int Health { get; set; }
    public int Strength { get; set; }
    public int Armor { get; set; }
}
