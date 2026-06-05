using RpgGame.Model.Game.Builders;
using RpgGame.Model.Game.Items;

namespace RpgGame.Model.Game.Factories;

public interface IDungeonThemeFactory
{
    string ThemeName { get; }
    string IntroMessage { get; }
    IReadOnlyList<string> Title { get; }
    IGenerationStrategy GenerationStrategy { get; }

    IItem CreateRandomJunk();
    Currency CreateRandomCurrency();
    IItem CreateRandomWeapon(int index);
    Enemy CreateEnemy(int id, int x, int y, int distanceFromStart);
    Enemy CreateBoss(int id, int x, int y);
}
