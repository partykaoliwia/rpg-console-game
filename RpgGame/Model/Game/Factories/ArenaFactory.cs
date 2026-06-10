using RpgGame.Model.Game.Builders;
using RpgGame.Model.Game.Items;

namespace RpgGame.Model.Game.Factories;

public sealed class ArenaFactory : IDungeonThemeFactory
{
    private readonly Random _random = new();

    public string ThemeName => "Ares' arena";
    public string IntroMessage => "The scent of blood and sweat fills the air. You have entered the brutal Arena of Ares.";
    public IReadOnlyList<string> Title { get; } =
    [
        "           █████╗ ██████╗ ███████╗███████╗           ",
        "          ██╔══██╗██╔══██╗██╔════╝██╔════╝           ",
        "          ███████║██████╔╝█████╗  ███████╗           ",
        "          ██╔══██║██╔══██╗██╔══╝  ╚════██║           ",
        "          ██║  ██║██║  ██║███████╗███████║           ",
        "          ╚═╝  ╚═╝╚═╝  ╚═╝╚══════╝╚══════╝           ",
        "     █████╗ ██████╗ ███████╗███╗   ██╗ █████╗        ",
        "    ██╔══██╗██╔══██╗██╔════╝████╗  ██║██╔══██╗       ",
        "    ███████║██████╔╝█████╗  ██╔██╗ ██║███████║       ",
        "    ██╔══██║██╔══██╗██╔══╝  ██║╚██╗██║██╔══██║       ",
        "    ██║  ██║██║  ██║███████╗██║ ╚████║██║  ██║       ",
        "    ╚═╝  ╚═╝╚═╝  ╚═╝╚══════╝╚═╝  ╚═══╝╚═╝  ╚═╝       "
    ];

    public IGenerationStrategy GenerationStrategy => new ArenaGenStrategy();

    public IItem CreateRandomJunk()
    {
        return new SpartanModifier(new ShatteredShield());
    }

    public Currency CreateRandomCurrency()
    {
        var values = new[] { 10, 20, 30, 50 };
        return new Gold(values[_random.Next(values.Length)]);
    }

    public IItem CreateRandomWeapon(int index)
    {
        if (index == 0)
        {
            return SlottedItemRecipes.CreateShowcaseSword(5 + _random.Next(0, 7));
        }

        return Decorate(new Kopis(5 + _random.Next(0, 7)));
    }

    public IItem CreateSlotHandle(int index)
    {
        return SlottedItemRecipes.CreateHandle(index);
    }

    public Enemy CreateEnemy(int id, int x, int y, int distanceFromStart)
    {
        if (distanceFromStart < 25)
        {
            return new Harpy(id, "Arena Harpy", 30, 12, 2, x, y);
        }

        if (distanceFromStart < 35)
        {
            return new Satyr(id, "Arena Satyr", 35, 16, 3, x, y);
        }

        if (distanceFromStart < 45)
        {
            return new Giant(id, "Arena Giant", 45, 18, 5, x, y);
        }

        return new Giant(id, "Champion of Ares", 60, 25, 8, x, y);
    }

    public Enemy CreateBoss(int id, int x, int y)
    {
        return new Giant(id, "Deimos", 80, 15, 5, x, y);
    }

    private IItem Decorate(IItem item)
    {
        Func<IItem, IItem>[] modifierFactories =
        {
            value => new SpartanModifier(value),
            value => new HephaesteanModifier(value),
            value => value
        };

        return modifierFactories[_random.Next(modifierFactories.Length)](item);
    }
}
