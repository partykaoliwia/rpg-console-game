namespace RpgGame.Model.Game.Builders;

public interface IDungeonBuilder
{
    void BuildEmptyDungeon();
    void BuildFilledDungeon();
    void AddCorridors();
    void AddRooms();
    void AddCentralHall(int width, int height);
    void AddItems(int count);
    void AddWeapons(int count);
    void AddMoney(int count);
    void AddEnemies(int count);
}
