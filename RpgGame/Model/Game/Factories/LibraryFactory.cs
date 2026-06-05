using RpgGame.Model.Game.Builders;
using RpgGame.Model.Game.Items;

namespace RpgGame.Model.Game.Factories;

public sealed class LibraryFactory : IDungeonThemeFactory
{
    private readonly Random _random = new();

    public string ThemeName => "Library of Alexandria";
    public string IntroMessage => "The scent of ancient papyrus fills the air. You have entered the lost Library of Alexandria.";
    public IReadOnlyList<string> Title { get; } =
    [
        " █████╗ ██╗     ███████╗██╗  ██╗ █████╗ ███╗   ██╗██████╗ ██████╗ ██╗  █████╗ ",
        "██╔══██╗██║     ██╔════╝╚██╗██╔╝██╔══██╗████╗  ██║██╔══██╗██╔══██╗██║ ██╔══██╗",
        "███████║██║     █████╗   ╚███╔╝ ███████║██╔██╗ ██║██║  ██║██████╔╝██║ ███████║",
        "██╔══██║██║     ██╔══╝   ██╔██╗ ██╔══██║██║╚██╗██║██║  ██║██╔══██╗██║ ██╔══██║",
        "██║  ██║███████╗███████╗██╔╝ ██╗██║  ██║██║ ╚████║██████╔╝██║  ██║██║ ██║  ██║",
        "╚═╝  ╚═╝╚══════╝╚══════╝╚═╝  ╚═╝╚═╝  ╚═╝╚═╝  ╚═══╝╚═════╝ ╚═╝  ╚═╝╚═╝ ╚═╝  ╚═╝",
        "            ██╗     ██╗ ██████╗ ██████╗  █████╗ ██████╗ ██╗   ██╗             ",
        "            ██║     ██║ ██╔══██╗██╔══██╗██╔══██╗██╔══██╗╚██╗ ██╔╝             ",
        "            ██║     ██║ ██████╔╝██████╔╝███████║██████╔╝ ╚████╔╝              ",
        "            ██║     ██║ ██╔══██╗██╔══██╗██╔══██║██╔══██╗  ╚██╔╝               ",
        "            ███████╗██║ ██████╔╝██║  ██║██║  ██║██║  ██║   ██║                ",
        "            ╚══════╝╚═╝ ╚═════╝ ╚═╝  ╚═╝╚═╝  ╚═╝╚═╝  ╚═╝   ╚═╝                "
    ];

    public IGenerationStrategy GenerationStrategy => new LibraryGenStrategy();

    public IItem CreateRandomJunk()
    {
        return new AthenianModifier(new OldBook());
    }

    public Currency CreateRandomCurrency()
    {
        var values = new[] { 5, 10, 15, 20 };
        return new Coin(values[_random.Next(values.Length)]);
    }

    public IItem CreateRandomWeapon(int index)
    {
        return Decorate(new StymphalianQuill(4 + _random.Next(0, 9)));
    }

    public Enemy CreateEnemy(int id, int x, int y, int distanceFromStart)
    {
        if (distanceFromStart < 25)
        {
            return new Harpy(id, "Archive Harpy", 30, 15, 2, x, y);
        }

        if (distanceFromStart < 40)
        {
            return new Giant(id, "Silent Giant", 40, 20, 4, x, y);
        }

        return new Giant(id, "Guardian of Alexandria", 50, 22, 6, x, y);
    }

    public Enemy CreateBoss(int id, int x, int y)
    {
        return new Harpy(id, "Medusa", 80, 30, 7, x, y);
    }

    private IItem Decorate(IItem item)
    {
        Func<IItem, IItem>[] modifierFactories =
        {
            value => new AthenianModifier(value),
            value => new PrometheanModifier(value),
            value => value
        };

        return modifierFactories[_random.Next(modifierFactories.Length)](item);
    }
}
