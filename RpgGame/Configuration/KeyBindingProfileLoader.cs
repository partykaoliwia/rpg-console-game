using RpgGame.Model.Local.KeyBindings;

namespace RpgGame.Configuration;

public sealed class KeyBindingProfileLoader
{
    private readonly Dictionary<string, Dictionary<string, ConsoleKey>> _keybinds =
        new(StringComparer.OrdinalIgnoreCase);

    public KeyBindingProfile LoadOrDefault(string filePath)
    {
        if (!File.Exists(filePath))
        {
            return new KeyBindingProfile();
        }

        LoadKeybinds(filePath);
        return CreateProfile();
    }

    private void LoadKeybinds(string filePath)
    {
        _keybinds.Clear();
        var currentSection = "Default";

        foreach (var line in File.ReadAllLines(filePath))
        {
            var trimmedLine = line.Trim();
            if (string.IsNullOrWhiteSpace(trimmedLine))
            {
                continue;
            }

            if (trimmedLine.StartsWith("[") && trimmedLine.EndsWith("]"))
            {
                currentSection = trimmedLine[1..^1];
                EnsureSection(currentSection);
                continue;
            }

            var parts = trimmedLine.Split('=', 2);
            if (parts.Length != 2)
            {
                continue;
            }

            var action = parts[0].Trim();
            var keyText = parts[1].Trim();
            if (!Enum.TryParse<ConsoleKey>(keyText, true, out var key))
            {
                continue;
            }

            AddKey(currentSection, action, key, filePath);
        }
    }

    private KeyBindingProfile CreateProfile()
    {
        var defaults = new KeyBindingProfile();
        return new KeyBindingProfile
        {
            MoveUp = GetKey("Playing", defaults.MoveUp, "MoveUp"),
            MoveDown = GetKey("Playing", defaults.MoveDown, "MoveDown"),
            MoveLeft = GetKey("Playing", defaults.MoveLeft, "MoveLeft"),
            MoveRight = GetKey("Playing", defaults.MoveRight, "MoveRight"),
            Collect = GetKey("Playing", defaults.Collect, "Collect"),
            Attack = GetKey("Playing", defaults.Attack, "Attack"),
            NormalAttack = GetKey("Combat", defaults.NormalAttack, "NormalAttack"),
            StealthAttack = GetKey("Combat", defaults.StealthAttack, "StealthAttack"),
            MagicAttack = GetKey("Combat", defaults.MagicAttack, "MagicAttack"),
            CancelAttack = GetKey("Combat", defaults.CancelAttack, "CancelAttack", "Flee"),
            ChooseLeftWeapon = GetKey("Combat", defaults.ChooseLeftWeapon, "ChooseLeftWeapon", "ChooseLeft"),
            ChooseRightWeapon = GetKey("Combat", defaults.ChooseRightWeapon, "ChooseRightWeapon", "ChooseRight"),
            OpenInventory = GetKey("Playing", defaults.OpenInventory, "OpenInventory"),
            CloseMenu = GetKey("Inventory", defaults.CloseMenu, "CloseMenu", "Quit"),
            DropItem = GetKey("Inventory", defaults.DropItem, "DropItem", "Drop"),
            EquipLeft = GetKey("Inventory", defaults.EquipLeft, "EquipLeft"),
            EquipRight = GetKey("Inventory", defaults.EquipRight, "EquipRight"),
            UnequipLeft = GetKey("Inventory", defaults.UnequipLeft, "UnequipLeft"),
            UnequipRight = GetKey("Inventory", defaults.UnequipRight, "UnequipRight"),
            OpenSlots = GetKey(new[] { "Inventory", "Slots" }, defaults.OpenSlots, "OpenSlots", "OpenSlot"),
            LeaveSlots = GetKey("Slots", defaults.LeaveSlots, "LeaveSlots", "Back"),
            OpenLogHistory = GetKey("Playing", defaults.OpenLogHistory, "OpenLogHistory", "LogHistory"),
            InventoryOpenLogHistory = GetKey("Inventory", defaults.InventoryOpenLogHistory, "OpenLogHistory", "LogHistory"),
            LootOpenLogHistory = GetKey(new[] { "Loot", "Looting" }, defaults.LootOpenLogHistory, "OpenLogHistory", "LogHistory"),
            CombatOpenLogHistory = GetKey("Combat", defaults.CombatOpenLogHistory, "OpenLogHistory", "LogHistory"),
            CloseLogHistory = GetKey("LogHistory", defaults.CloseLogHistory, "CloseLogHistory", "CloseLogs"),
            LootCloseMenu = GetKey(new[] { "Loot", "Looting" }, defaults.LootCloseMenu, "CloseMenu", "Quit"),
            ReloadKeyBindings = GetKey("Playing", defaults.ReloadKeyBindings, "ReloadKeyBindings", "Reload"),
            ContinueInstructions = GetKey("Instruction", defaults.ContinueInstructions, "ContinueInstructions", "Continue"),
            Quit = GetKey("Playing", defaults.Quit, "Quit")
        };
    }

    private ConsoleKey GetKey(string sectionName, ConsoleKey defaultKey, params string[] actionNames)
    {
        return GetKey(new[] { sectionName }, defaultKey, actionNames);
    }

    private ConsoleKey GetKey(IReadOnlyList<string> sectionNames, ConsoleKey defaultKey, params string[] actionNames)
    {
        foreach (var sectionName in sectionNames)
        {
            if (!_keybinds.TryGetValue(sectionName, out var section))
            {
                continue;
            }

            foreach (var actionName in actionNames)
            {
                if (section.TryGetValue(actionName, out var key))
                {
                    return key;
                }
            }
        }

        return defaultKey;
    }

    private void AddKey(string sectionName, string actionName, ConsoleKey key, string filePath)
    {
        var section = EnsureSection(sectionName);
        if (section.TryGetValue(actionName, out var existingKey) && existingKey == key)
        {
            return;
        }

        if (section.ContainsValue(key))
        {
            throw new InvalidOperationException(
                $"Key '{key}' is already used in section [{sectionName}]. Fix {filePath}.");
        }

        section[actionName] = key;
    }

    private Dictionary<string, ConsoleKey> EnsureSection(string sectionName)
    {
        if (!_keybinds.TryGetValue(sectionName, out var section))
        {
            section = new Dictionary<string, ConsoleKey>(StringComparer.OrdinalIgnoreCase);
            _keybinds[sectionName] = section;
        }

        return section;
    }
}
