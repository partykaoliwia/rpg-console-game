using RpgGame.Model.Game.Builders;
using RpgGame.Model.Game.Factories;
using RpgGame.Model.Local.KeyBindings;

namespace RpgGame.Model.Local.Instructions;

public sealed class InstructionBuilder : IDungeonBuilder
{
    private static readonly IReadOnlyDictionary<ConsoleKey, string> KeyNames = new Dictionary<ConsoleKey, string>
    {
        { ConsoleKey.D0, "0" },
        { ConsoleKey.D1, "1" },
        { ConsoleKey.D2, "2" },
        { ConsoleKey.D3, "3" },
        { ConsoleKey.D4, "4" },
        { ConsoleKey.D5, "5" },
        { ConsoleKey.D6, "6" },
        { ConsoleKey.D7, "7" },
        { ConsoleKey.D8, "8" },
        { ConsoleKey.D9, "9" },
        { ConsoleKey.NumPad0, "0" },
        { ConsoleKey.NumPad1, "1" },
        { ConsoleKey.NumPad2, "2" },
        { ConsoleKey.NumPad3, "3" },
        { ConsoleKey.NumPad4, "4" },
        { ConsoleKey.NumPad5, "5" },
        { ConsoleKey.NumPad6, "6" },
        { ConsoleKey.NumPad7, "7" },
        { ConsoleKey.NumPad8, "8" },
        { ConsoleKey.NumPad9, "9" },
        { ConsoleKey.Escape, "Esc" },
        { ConsoleKey.Spacebar, "Space" }
    };

    private readonly IDungeonThemeFactory _factory;
    private readonly KeyBindingProfile _keyBindings;
    private readonly List<string> _lines = new();

    public InstructionBuilder(IDungeonThemeFactory factory, KeyBindingProfile keyBindings)
    {
        _factory = factory;
        _keyBindings = keyBindings;
    }

    public InstructionGuide BuildResult()
    {
        return new InstructionGuide(_factory.Title, _lines.ToList());
    }

    public void BuildEmptyDungeon()
    {
        AddIntro();
    }

    public void BuildFilledDungeon()
    {
        AddIntro();
    }

    public void AddCorridors()
    {
        _lines.Add("Before you stretch countless winding and narrow corridors, where it is easy to get lost.");
    }

    public void AddRooms()
    {
        _lines.Add("You have stumbled upon several hidden chambers carved into the rock, which may conceal treasures or danger.");
    }

    public void AddCentralHall(int width, int height)
    {
        _lines.Add("In the very heart of this cursed place lurks a mighty Chamber. That is where the greatest danger awaits.");
    }

    public void AddItems(int count)
    {
        _lines.Add($"The gods have left here {count} ancient artifacts, which you can pick up by pressing [{KeyName(_keyBindings.Collect)}].");
    }

    public void AddWeapons(int count)
    {
        _lines.Add($"Previous adventurers have left {count} pieces of powerful weapons here - use [{KeyName(_keyBindings.Collect)}] to pick up and [{KeyName(_keyBindings.OpenInventory)}] to manage your equipment.");
    }

    public void AddMoney(int count)
    {
        _lines.Add($"There are {count} coins scattered here, collect them to strengthen your position in the world of mortals.");
    }

    public void AddEnemies(int count)
    {
        if (count == 0)
        {
            _lines.Add("A powerful Boss lurks in the shadows, threatening your life.");
            return;
        }

        _lines.Add($"Beware! {count} enemies and a powerful Boss lurk in the shadows, threatening your life.");
    }

    private void AddIntro()
    {
        if (_lines.Count == 0)
        {
            _lines.Add(_factory.IntroMessage);
        }
    }

    private static string KeyName(ConsoleKey key)
    {
        return KeyNames.TryGetValue(key, out var name) ? name : key.ToString();
    }
}
