using RpgGame.Model.Game.Builders;
using RpgGame.Model.Game.Items;

namespace RpgGame.Model.Game.Factories;

public sealed class LabyrinthFactory : IDungeonThemeFactory
{
    private readonly Random _random = new();

    public string ThemeName => "Daedalus' labyrinth";
    public string IntroMessage => "You stand before the entrance to a dark labyrinth carved by Daedalus, full of hidden paths and mysteries.";
    public IReadOnlyList<string> Title { get; } =
    [
        "  ██████╗  █████╗ ███████╗██████╗  █████╗ ██╗     ██╗   ██╗███████╗",
        "  ██╔══██╗██╔══██╗██╔════╝██╔══██╗██╔══██╗██║     ██║   ██║██╔════╝",
        "  ██║  ██║███████║█████╗  ██║  ██║███████║██║     ██║   ██║███████╗",
        "  ██║  ██║██╔══██║██╔══╝  ██║  ██║██╔══██║██║     ██║   ██║╚════██║",
        "  ██████╔╝██║  ██║███████╗██████╔╝██║  ██║███████╗╚██████╔╝███████║",
        "  ╚═════╝ ╚═╝  ╚═╝╚══════╝╚═════╝ ╚═╝  ╚═╝╚══════╝ ╚═════╝ ╚══════╝",
        "██╗      █████╗ ██████╗ ██╗   ██╗██████╗ ██╗ ███╗   ██╗████████╗██╗  ██╗",
        "██║     ██╔══██╗██╔══██╗╚██╗ ██╔╝██╔══██╗██║ ████╗  ██║╚══██╔══╝██║  ██║",
        "██║     ███████║██████╔╝ ╚████╔╝ ██████╔╝██║ ██╔██╗ ██║   ██║   ███████║",
        "██║     ██╔══██║██╔══██╗  ╚██╔╝  ██╔══██╗██║ ██║╚██╗██║   ██║   ██╔══██║",
        "███████╗██║  ██║██████╔╝   ██║   ██║  ██║██║ ██║ ╚████║   ██║   ██║  ██║",
        "╚══════╝╚═╝  ╚═╝╚═════╝    ╚═╝   ╚═╝  ╚═╝╚═╝ ╚═╝  ╚═══╝   ╚═╝   ╚═╝  ╚═╝"
    ];

    public IGenerationStrategy GenerationStrategy => new LabyrinthGenStrategy();

    public IItem CreateRandomJunk()
    {
        Func<IItem>[] junkFactories =
        {
            () => new IcarusFeather(),
            () => new CyclopsEye(),
            () => new AriadnesThread()
        };

        return junkFactories[_random.Next(junkFactories.Length)]();
    }

    public Currency CreateRandomCurrency()
    {
        var values = new[] { 10, 10, 10, 10, 25, 25, 25, 35, 35, 50 };
        return _random.Next(0, 2) == 0
            ? new Gold(values[_random.Next(values.Length)])
            : new Coin(values[_random.Next(values.Length)]);
    }

    public IItem CreateRandomWeapon(int index)
    {
        return Decorate(CreateBaseWeapon(index, 6 + _random.Next(0, 8)));
    }

    public Enemy CreateEnemy(int id, int x, int y, int distanceFromStart)
    {
        if (distanceFromStart < 25)
        {
            return new Harpy(id, "Harpy", 30, 20, 2, x, y);
        }

        if (distanceFromStart < 35)
        {
            return new Satyr(id, "Lost Satyr", 35, 14, 3, x, y);
        }

        if (distanceFromStart < 45)
        {
            return new Giant(id, "Giant", 40, 13, 4, x, y);
        }

        return new Giant(id, "Ancient Giant", 50, 18, 8, x, y);
    }

    public Enemy CreateBoss(int id, int x, int y)
    {
        return new Harpy(id, "Minotaur", 80, 15, 5, x, y);
    }

    private IItem CreateBaseWeapon(int index, int damage)
    {
        if (index == 0 || index == 2)
        {
            Func<int, IItem>[] earlyWeaponFactories =
            {
                value => new Xiphos(value),
                value => new Dory(value)
            };

            return earlyWeaponFactories[_random.Next(earlyWeaponFactories.Length)](damage);
        }

        if (index == 1)
        {
            return new Labrys(damage);
        }

        Func<int, IItem>[] weaponFactories =
        {
            value => new Xiphos(value),
            value => new Dory(value),
            value => new Labrys(value)
        };

        return weaponFactories[_random.Next(weaponFactories.Length)](damage);
    }

    private IItem Decorate(IItem item)
    {
        Func<IItem, IItem>[] modifierFactories =
        {
            value => new SpartanModifier(value),
            value => new MidasTouchModifier(value),
            value => new IcarianModifier(new DionysianModifier(value)),
            value => new AthenianModifier(value)
        };

        return modifierFactories[_random.Next(modifierFactories.Length)](item);
    }
}
