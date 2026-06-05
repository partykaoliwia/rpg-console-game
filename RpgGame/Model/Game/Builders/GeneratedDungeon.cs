namespace RpgGame.Model.Game.Builders;

public sealed class GeneratedDungeon
{
    public GeneratedDungeon(Board board, IReadOnlyList<Enemy> enemies)
    {
        Board = board;
        Enemies = enemies;
    }

    public Board Board { get; }
    public IReadOnlyList<Enemy> Enemies { get; }
}
