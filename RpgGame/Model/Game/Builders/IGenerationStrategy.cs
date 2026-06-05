namespace RpgGame.Model.Game.Builders;

public interface IGenerationStrategy
{
    void GenerateMap(IDungeonBuilder builder);
}
