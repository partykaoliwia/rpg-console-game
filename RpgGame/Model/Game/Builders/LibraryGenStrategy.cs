namespace RpgGame.Model.Game.Builders;

public sealed class LibraryGenStrategy : IGenerationStrategy
{
    public void GenerateMap(IDungeonBuilder builder)
    {
        builder.BuildFilledDungeon();
        builder.AddCorridors();
        builder.AddRooms();
        builder.AddItems(15);
        builder.AddWeapons(5);
        builder.AddMoney(20);
        builder.AddEnemies(15);
    }
}
