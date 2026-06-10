using System.Text;
using RpgGame.Model.Game;
using RpgGame.Model.Game.Items;
using RpgGame.Model.Local;
using RpgGame.Model.Local.InputModes;
using RpgGame.View.Visitors;

namespace RpgGame.View;

public sealed class ConsoleGameView : IGameView
{
    private const int ScreenWidth = 120;
    private const int ScreenHeight = 29;
    private const int LeftX = 2;
    private const int LeftWidth = 33;
    private const int RightX = 85;
    private const int RightWidth = 32;
    private const int CenterX = 36;
    private const int CenterWidth = 47;
    private const int CombatColumnWidth = 32;
    private const string AnsiReset = "\u001b[0m";
    private static readonly IReadOnlyDictionary<ConsoleColor, string> AnsiColors = new Dictionary<ConsoleColor, string>
    {
        { ConsoleColor.Black, "\u001b[30m" },
        { ConsoleColor.DarkBlue, "\u001b[34m" },
        { ConsoleColor.DarkGreen, "\u001b[32m" },
        { ConsoleColor.DarkCyan, "\u001b[36m" },
        { ConsoleColor.DarkRed, "\u001b[31m" },
        { ConsoleColor.DarkMagenta, "\u001b[35m" },
        { ConsoleColor.DarkYellow, "\u001b[33m" },
        { ConsoleColor.Gray, "\u001b[37m" },
        { ConsoleColor.DarkGray, "\u001b[90m" },
        { ConsoleColor.Blue, "\u001b[94m" },
        { ConsoleColor.Green, "\u001b[92m" },
        { ConsoleColor.Cyan, "\u001b[96m" },
        { ConsoleColor.Red, "\u001b[91m" },
        { ConsoleColor.Magenta, "\u001b[95m" },
        { ConsoleColor.Yellow, "\u001b[93m" },
        { ConsoleColor.White, "\u001b[97m" }
    };

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
        { ConsoleKey.Enter, "Enter" },
        { ConsoleKey.Backspace, "Backspace" },
        { ConsoleKey.Spacebar, "Space" }
    };

    public void Render(GameModel gameModel, LocalModel localModel)
    {
        lock (gameModel.SyncRoot)
        {
            Console.CursorVisible = false;
            TryPrepareConsoleSurface();

            var frame = new ConsoleFrame(GetFrameWidth(), GetFrameHeight());
            if (frame.Width < ScreenWidth || frame.Height < ScreenHeight)
            {
                DrawResizeMessage(frame);
            }
            else
            {
                localModel.CurrentInputMode.Render(new ConsoleInputModeRenderer(frame, gameModel, localModel));
            }

            FlushFrame(frame);
        }
    }

    private static void DrawResizeMessage(ConsoleFrame frame)
    {
        var centerX = Math.Max(0, frame.Width / 2);
        var centerY = Math.Max(0, frame.Height / 2);
        WriteCentered(frame, centerX, Math.Max(0, centerY - 1), "Console window is too small.", ConsoleColor.DarkYellow);
        WriteCentered(frame, centerX, centerY, "Use at least 120 x 29.", ConsoleColor.Gray);
    }

    private static void DrawInstructions(ConsoleFrame frame, LocalModel localModel)
    {
        var centerX = ScreenWidth / 2;
        var maxWidth = Math.Max(0, ScreenWidth - 8);
        var totalHeight = localModel.InstructionGuide.Title.Count + localModel.InstructionGuide.Lines.Count + 5;
        var y = Math.Max(1, (ScreenHeight - totalHeight) / 2);

        foreach (var titleLine in localModel.InstructionGuide.Title)
        {
            WriteCentered(frame, centerX, y++, Trim(titleLine, maxWidth), ConsoleColor.Yellow);
        }

        y++;
        foreach (var line in localModel.InstructionGuide.Lines)
        {
            WriteCentered(frame, centerX, y++, Trim(line, maxWidth), ConsoleColor.Gray);
        }

        y += 2;
        var continueText = $"Press [{KeyName(localModel.KeyBindings.ContinueInstructions)}] to enter the labyrinth...";
        WriteCentered(frame, centerX, y, continueText, ConsoleColor.DarkYellow);

        var localMessage = localModel.GetFreshLocalMessage();
        if (localMessage != null)
        {
            var color = localMessage.StartsWith("Invalid key", StringComparison.OrdinalIgnoreCase)
                ? ConsoleColor.Red
                : ConsoleColor.DarkYellow;
            WriteCentered(frame, centerX, y + 2, Trim(localMessage, maxWidth), color);
        }
    }

    private static void DrawGameOver(ConsoleFrame frame, LocalModel localModel)
    {
        string[] title =
        {
            @"  _____          __  __  ______    ____ __      __ ______  _____  ",
            @" / ____|   /\   |  \/  ||  ____|  / __ \\ \    / /|  ____||  __ \ ",
            @"| |  __   /  \  | \  / || |__    | |  | |\ \  / / | |__   | |__) |",
            @"| | |_ | / /\ \ | |\/| ||  __|   | |  | | \ \/ /  |  __|  |  _  / ",
            @"| |__| |/ ____ \| |  | || |____  | |__| |  \  /   | |____ | | \ \ ",
            @" \_____/_/    \_\_|  |_||______|  \____/    \/    |______||_|  \_\"
        };

        var centerX = ScreenWidth / 2;
        var startY = Math.Max(1, (ScreenHeight - title.Length - 4) / 2);
        if (!string.IsNullOrWhiteSpace(localModel.LogFilePath))
        {
            WriteCentered(frame, centerX, startY - 1, Trim(localModel.LogFilePath, ScreenWidth - 8), ConsoleColor.DarkRed);
        }

        var y = startY;
        foreach (var line in title)
        {
            WriteCentered(frame, centerX, y++, line, ConsoleColor.DarkRed);
        }

        y += 2;
        WriteCentered(frame, centerX, y, "YOU DIED.", ConsoleColor.Red);
        WriteCentered(frame, centerX, startY + title.Length + 8, "Press any key to exit...", ConsoleColor.White);
    }

    private static void WriteCentered(ConsoleFrame frame, int centerX, int y, string text, ConsoleColor color)
    {
        var x = Math.Max(0, centerX - text.Length / 2);
        frame.WriteText(x, y, text, color, text.Length);
    }

    private static void DrawPlayingView(ConsoleFrame frame, GameModel gameModel, LocalModel localModel)
    {
        var player = gameModel.FindPlayer(localModel.LocalPlayerId);
        var boardStartX = GetBoardStartX(gameModel.Board);
        var boardStartY = GetBoardStartY(gameModel.Board);

        DrawBoard(frame, gameModel, localModel, boardStartX, boardStartY);
        DrawLeftColumn(frame, gameModel, localModel, player);

        if (player != null)
        {
            var inventoryCursor = localModel.CurrentInputMode.ShowsInventory ? localModel.InventoryCursor : -1;
            DrawPlayerStatsBoxes(frame, player, inventoryCursor);
        }
    }

    private static void DrawLeftColumn(ConsoleFrame frame, GameModel gameModel, LocalModel localModel, Player? player)
    {
        if (localModel.CurrentInputMode.ShowsSlots && player != null)
        {
            DrawSlotControlsBox(frame, localModel);
        }
        else if (localModel.CurrentInputMode.ShowsInventory && player != null)
        {
            DrawInventoryControlsBox(frame, localModel);
        }
        else if (localModel.CurrentInputMode.ShowsLoot && player != null)
        {
            DrawLootControlsBox(frame, localModel);
        }
        else
        {
            DrawPlayingControlsBox(frame, gameModel, localModel, player);
        }

        if (player != null)
        {
            if (localModel.CurrentInputMode.ShowsSlots)
            {
                DrawSlotBrowserBox(frame, player, localModel, LeftX, 11, LeftWidth, 12);
            }
            else
            {
                DrawGroundItemsBox(frame, gameModel.Board.GetTile(player.X, player.Y), localModel);
            }
        }

        DrawRecentLogBox(frame, gameModel, localModel);
    }

    private static void DrawPlayingControlsBox(
        ConsoleFrame frame,
        GameModel gameModel,
        LocalModel localModel,
        Player? player)
    {
        var keys = localModel.KeyBindings;
        DrawBox(frame, LeftX, 1, LeftWidth, 10, " CONTROLS ");
        var y = 3;
        frame.WriteText(
            LeftX + 2,
            y++,
            $"[{KeyName(keys.MoveUp)}/{KeyName(keys.MoveLeft)}/{KeyName(keys.MoveDown)}/{KeyName(keys.MoveRight)}] Move",
            ConsoleColor.Gray,
            LeftWidth - 4);
        frame.WriteText(LeftX + 2, y++, $"[{KeyName(keys.OpenInventory)}] Open inventory", ConsoleColor.Gray, LeftWidth - 4);
        frame.WriteText(LeftX + 2, y++, $"[{KeyName(keys.OpenLogHistory)}] Show logs", ConsoleColor.Gray, LeftWidth - 4);

        if (player != null && gameModel.FindFirstAdjacentEnemy(player.Id) != null)
        {
            frame.WriteText(LeftX + 2, y++, $"[{KeyName(keys.Attack)}] Attack", ConsoleColor.DarkYellow, LeftWidth - 4);
        }

        if (player != null && gameModel.Board.GetTile(player.X, player.Y).Items.Count > 0)
        {
            frame.WriteText(LeftX + 2, y++, $"[{KeyName(keys.Collect)}] Pick up", ConsoleColor.DarkYellow, LeftWidth - 4);
        }
    }

    private static void DrawInventoryControlsBox(ConsoleFrame frame, LocalModel localModel)
    {
        var keys = localModel.KeyBindings;
        DrawBox(frame, LeftX, 1, LeftWidth, 10, " CONTROLS ");
        var y = 3;
        frame.WriteText(
            LeftX + 2,
            y++,
            $"[{KeyName(keys.MoveUp)}/{KeyName(keys.MoveDown)}] Up/Down",
            ConsoleColor.Gray,
            LeftWidth - 4);
        frame.WriteText(
            LeftX + 2,
            y++,
            $"[{KeyName(keys.EquipLeft)}/{KeyName(keys.EquipRight)}] Equip to L/R hand",
            ConsoleColor.Gray,
            LeftWidth - 4);
        frame.WriteText(
            LeftX + 2,
            y++,
            $"[{KeyName(keys.UnequipLeft)}/{KeyName(keys.UnequipRight)}] Unequip L/R hand",
            ConsoleColor.Gray,
            LeftWidth - 4);
        frame.WriteText(LeftX + 2, y++, $"[{KeyName(keys.DropItem)}] Drop", ConsoleColor.Gray, LeftWidth - 4);
        frame.WriteText(LeftX + 2, y++, $"[{KeyName(keys.OpenSlots)}] Slots", ConsoleColor.Gray, LeftWidth - 4);
        frame.WriteText(LeftX + 2, y++, $"[{KeyName(keys.OpenInventory)}] Return", ConsoleColor.Gray, LeftWidth - 4);
    }

    private static void DrawSlotControlsBox(ConsoleFrame frame, LocalModel localModel)
    {
        var keys = localModel.KeyBindings;
        DrawBox(frame, LeftX, 1, LeftWidth, 10, " SLOT CONTROLS ");
        var y = 3;
        frame.WriteText(
            LeftX + 2,
            y++,
            $"[{KeyName(keys.MoveUp)}/{KeyName(keys.MoveDown)}] Up/Down",
            ConsoleColor.Gray,
            LeftWidth - 4);
        frame.WriteText(LeftX + 2, y++, $"[{KeyName(keys.OpenSlots)}] Open nested slots", ConsoleColor.Gray, LeftWidth - 4);
        frame.WriteText(LeftX + 2, y++, $"[{KeyName(keys.LeaveSlots)}] Level up", ConsoleColor.Gray, LeftWidth - 4);
        frame.WriteText(LeftX + 2, y++, $"[{KeyName(keys.OpenInventory)}] Return", ConsoleColor.Gray, LeftWidth - 4);
    }

    private static void DrawLootControlsBox(ConsoleFrame frame, LocalModel localModel)
    {
        var keys = localModel.KeyBindings;
        DrawBox(frame, LeftX, 1, LeftWidth, 10, " CONTROLS ");
        var y = 3;
        frame.WriteText(
            LeftX + 2,
            y++,
            $"[{KeyName(keys.MoveUp)}/{KeyName(keys.MoveDown)}] Up/Down",
            ConsoleColor.Gray,
            LeftWidth - 4);
        frame.WriteText(LeftX + 2, y++, $"[{KeyName(keys.Collect)}] Pick up", ConsoleColor.Gray, LeftWidth - 4);
        frame.WriteText(LeftX + 2, y++, $"[{KeyName(keys.LootCloseMenu)}] Return", ConsoleColor.Gray, LeftWidth - 4);
    }

    private static void DrawGroundItemsBox(ConsoleFrame frame, ITile tile, LocalModel localModel)
    {
        DrawBox(frame, LeftX, 11, LeftWidth, 12, " ON THE GROUND ");
        if (tile.Items.Count == 0)
        {
            return;
        }

        var maxItems = Math.Min(tile.Items.Count, 8);
        for (var index = 0; index < maxItems; index++)
        {
            var prefix = index == localModel.LootCursor && localModel.CurrentInputMode.ShowsLoot ? " ➤ " : "  ";
            frame.WriteText(
                LeftX + 2,
                13 + index,
                $"{prefix}{Trim(tile.Items[index].Name, LeftWidth - 5)}",
                ConsoleColor.Gray,
                LeftWidth - 4);
        }

        if (tile.Items.Count > maxItems)
        {
            frame.WriteText(LeftX + 2, 21, $"+ {tile.Items.Count - maxItems} more", ConsoleColor.DarkGray, LeftWidth - 4);
        }
    }

    private static void DrawRecentLogBox(ConsoleFrame frame, GameModel gameModel, LocalModel localModel)
    {
        DrawBox(frame, LeftX, 23, LeftWidth, 5, " RECENT LOG ");
        var lastEntry = localModel.GetFreshLocalMessage()
            ?? gameModel.EventLog.GetRecent(1).FirstOrDefault();
        if (lastEntry == null)
        {
            return;
        }

        WriteWrapped(frame, LeftX + 2, 24, LeftWidth - 4, 3, lastEntry, ConsoleColor.DarkGray);
    }

    private static void DrawPlayerStatsBoxes(ConsoleFrame frame, Player player, int inventoryCursor)
    {
        DrawBox(frame, RightX, 1, RightWidth, 12, " PLAYER ");
        frame.WriteText(RightX + 2, 3, $"©: {WalletAmount(player, Coin.CurrencyName)}", ConsoleColor.Yellow, RightWidth - 4);
        frame.WriteText(RightX + 2, 4, $"¤: {WalletAmount(player, Gold.CurrencyName)}", ConsoleColor.Yellow, RightWidth - 4);
        frame.WriteText(RightX + 2, 6, $"Health: {player.Health}", ConsoleColor.Green, RightWidth - 4);
        frame.WriteText(RightX + 2, 7, $"Fortune: {player.Luck}", ConsoleColor.Gray, RightWidth - 4);
        frame.WriteText(RightX + 2, 8, $"Strength: {player.Strength}", ConsoleColor.Gray, RightWidth - 4);
        frame.WriteText(RightX + 2, 9, $"Fury: {player.Aggression}", ConsoleColor.Gray, RightWidth - 4);
        frame.WriteText(RightX + 2, 10, $"Wisdom: {player.Wisdom}", ConsoleColor.Gray, RightWidth - 4);
        frame.WriteText(RightX + 2, 11, $"Dexterity: {player.Dexterity}", ConsoleColor.Gray, RightWidth - 4);

        var inventoryTitle = player.Inventory.Count >= player.MaxInventorySize ? " INVENTORY (FULL) " : " INVENTORY ";
        DrawBox(frame, RightX, 13, RightWidth, 10, inventoryTitle);
        for (var index = 0; index < player.MaxInventorySize && index < 8; index++)
        {
            var prefix = index == inventoryCursor ? " ➤ " : "  ";
            var itemName = index < player.Inventory.Count ? player.Inventory[index].Name : string.Empty;
            frame.WriteText(RightX + 1, 15 + index, $"{prefix}{Trim(itemName, RightWidth - 5)}", ConsoleColor.Gray, RightWidth - 2);
        }

        DrawBox(frame, RightX, 23, RightWidth, 5, " HANDS ");
        frame.WriteText(RightX + 2, 25, $"L: {Trim(ItemName(player.LeftHand), RightWidth - 7)}", ConsoleColor.Gray, RightWidth - 4);
        frame.WriteText(RightX + 2, 26, $"R: {Trim(ItemName(player.RightHand), RightWidth - 7)}", ConsoleColor.Gray, RightWidth - 4);
    }

    private static void DrawBoard(ConsoleFrame frame, GameModel gameModel, LocalModel localModel, int offsetX, int offsetY)
    {
        var enemiesByPosition = CreateEnemyPositionIndex(gameModel);
        var playersByPosition = CreatePlayerPositionIndex(gameModel);

        for (var y = 0; y < gameModel.Board.Height; y++)
        {
            for (var x = 0; x < gameModel.Board.Width; x++)
            {
                var tile = gameModel.Board.GetTile(x, y);
                var icon = GetTileIcon(tile);

                var topItem = tile.GetTopItem();
                if (topItem != null)
                {
                    var itemVisitor = new ItemVisitor();
                    topItem.Accept(itemVisitor);
                    icon = itemVisitor.Icon;
                }

                var point = new BoardPoint(x, y);
                if (enemiesByPosition.TryGetValue(point, out var enemy))
                {
                    var characterVisitor = new CharacterVisitor(localModel.LocalPlayerId);
                    enemy.Accept(characterVisitor);
                    icon = characterVisitor.Icon;
                }

                if (playersByPosition.TryGetValue(point, out var player))
                {
                    var characterVisitor = new CharacterVisitor(localModel.LocalPlayerId);
                    player.Accept(characterVisitor);
                    icon = characterVisitor.Icon;
                }

                frame.WriteIcon(offsetX + x, offsetY + y, icon);
            }
        }
    }

    private static void DrawCombatView(ConsoleFrame frame, GameModel gameModel, LocalModel localModel)
    {
        var player = gameModel.FindPlayer(localModel.LocalPlayerId);
        var target = localModel.PendingAttackTargetId.HasValue
            ? gameModel.FindEnemy(localModel.PendingAttackTargetId.Value)
            : null;

        if (player == null ||
            target == null ||
            !gameModel.HasEncounterWithEnemy(localModel.LocalPlayerId, target.Id))
        {
            DrawPlayingView(frame, gameModel, localModel);
            return;
        }

        DrawCombatTargetBox(frame, target);
        DrawCombatGuideBox(frame);
        DrawCombatBattleLogBox(frame, gameModel, player, target);
        DrawCombatPlayerBox(frame, player);
        DrawCombatHandsBox(frame, player);
        DrawCombatControlsBox(frame, localModel);
    }

    private static void DrawSlotBrowserBox(
        ConsoleFrame frame,
        Player player,
        LocalModel localModel,
        int boxX,
        int boxY,
        int boxWidth,
        int boxHeight)
    {
        DrawBox(frame, boxX, boxY, boxWidth, boxHeight, " ITEM SLOTS ");

        var rootItem = SlotBrowserNavigator.GetRootItem(player, localModel);
        var currentItem = SlotBrowserNavigator.GetCurrentItem(rootItem, localModel.SlotPath);
        var textX = boxX + 2;
        var textWidth = boxWidth - 4;
        if (rootItem == null || currentItem == null)
        {
            frame.WriteText(textX, boxY + 2, "Selected item is no longer available.", ConsoleColor.DarkYellow, textWidth);
            return;
        }

        var y = boxY + 2;
        frame.WriteText(textX, y++, Trim(CreateSlotPathText(rootItem, localModel.SlotPath, textWidth), textWidth), ConsoleColor.DarkYellow, textWidth);
        frame.WriteText(textX, y++, Trim(CreateBonusText(currentItem.Bonuses), textWidth), ConsoleColor.DarkGray, textWidth);
        y++;

        if (currentItem.Slots.Count == 0)
        {
            frame.WriteText(textX, y, "No slots on this level.", ConsoleColor.DarkGray, textWidth);
            return;
        }

        var maxRows = Math.Max(1, boxY + boxHeight - 1 - y);
        var firstRow = Math.Clamp(
            localModel.SlotCursor - maxRows + 1,
            0,
            Math.Max(0, currentItem.Slots.Count - maxRows));
        var lastRow = Math.Min(currentItem.Slots.Count, firstRow + maxRows);

        for (var slotIndex = firstRow; slotIndex < lastRow; slotIndex++)
        {
            var slot = currentItem.Slots[slotIndex];
            var slotItem = slot.Item;
            var selected = slotIndex == localModel.SlotCursor;
            var prefix = selected ? "> " : "  ";
            var slotName = slotItem == null ? "(empty)" : slotItem.Name;
            var nestedMarker = slotItem != null && slotItem.Slots.Count > 0 ? " >" : string.Empty;
            var color = selected
                ? ConsoleColor.Yellow
                : slotItem == null ? ConsoleColor.DarkGray : ConsoleColor.Gray;

            frame.WriteText(
                textX,
                y++,
                Trim($"{prefix}[{slotIndex}] {slotName}{nestedMarker}", textWidth),
                color,
                textWidth);
        }

        if (firstRow > 0)
        {
            frame.WriteText(boxX + boxWidth - 6, boxY + 1, "...", ConsoleColor.DarkGray, 3);
        }

        if (lastRow < currentItem.Slots.Count)
        {
            frame.WriteText(boxX + boxWidth - 6, boxY + boxHeight - 2, "...", ConsoleColor.DarkGray, 3);
        }
    }

    private static void DrawCombatTargetBox(ConsoleFrame frame, Enemy target)
    {
        var characterVisitor = new CharacterVisitor(0);
        target.Accept(characterVisitor);
        var textWidth = CombatColumnWidth - 3;

        DrawBox(frame, LeftX, 1, CombatColumnWidth, 10, $" {target.Name.ToUpperInvariant()} ");
        frame.WriteText(LeftX + 2, 3, $"HP: {Math.Max(0, target.Health)}", ConsoleColor.Red, textWidth);
        frame.WriteText(LeftX + 2, 4, $"Strength: {target.Strength}", ConsoleColor.Gray, textWidth);
        frame.WriteText(LeftX + 2, 5, $"Armor: {target.Armor}", ConsoleColor.Gray, textWidth);

        var enemyIcon = $"[ {characterVisitor.Icon.Symbol} ]";
        var iconX = LeftX + (CombatColumnWidth - enemyIcon.Length) / 2;
        frame.WriteText(iconX, 8, enemyIcon, characterVisitor.Icon.Color, enemyIcon.Length);
    }

    private static void DrawCombatGuideBox(ConsoleFrame frame)
    {
        var textWidth = CombatColumnWidth - 3;
        DrawBox(frame, LeftX, 12, CombatColumnWidth, 15, " COMBAT GUIDE ");
        var y = 14;
        frame.WriteText(LeftX + 2, y++, "Normal Attack", ConsoleColor.DarkGray, textWidth);
        frame.WriteText(LeftX + 2, y++, "Dory: 1", ConsoleColor.Gray, textWidth);
        frame.WriteText(LeftX + 2, y++, "Other: Base", ConsoleColor.Gray, textWidth);
        y++;
        frame.WriteText(LeftX + 2, y++, "Stealth Attack", ConsoleColor.DarkGray, textWidth);
        frame.WriteText(LeftX + 2, y++, "Xiphos/Quill: 2 * base", ConsoleColor.Gray, textWidth);
        frame.WriteText(LeftX + 2, y++, "Labrys/Kopis: 0.5 * base", ConsoleColor.Gray, textWidth);
        frame.WriteText(LeftX + 2, y++, "Dory: 1", ConsoleColor.Gray, textWidth);
        y++;
        frame.WriteText(LeftX + 2, y++, "Magic Attack", ConsoleColor.DarkGray, textWidth);
        frame.WriteText(LeftX + 2, y++, "Other: 1", ConsoleColor.Gray, textWidth);
        frame.WriteText(LeftX + 2, y, "Dory: Base", ConsoleColor.Gray, textWidth);
    }

    private static void DrawCombatBattleLogBox(ConsoleFrame frame, GameModel gameModel, Player player, Enemy target)
    {
        DrawBox(frame, CenterX, 1, CenterWidth, 26, " BATTLE LOG ");
        var y = 3;
        foreach (var entry in CreateBattleLogEntries(gameModel, player, target))
        {
            var color = CreateBattleLogColor(entry);
            y = WriteWrapped(frame, CenterX + 2, y, CenterWidth - 4, 26 - y, entry, color) + 1;
            if (y >= 26)
            {
                return;
            }
        }
    }

    private static IReadOnlyList<string> CreateBattleLogEntries(GameModel gameModel, Player player, Enemy target)
    {
        return gameModel.EventLog.GetRecent(50)
            .Where(IsBattleLogEntry)
            .Where(entry => IsCurrentCombatEntry(entry, player, target))
            .TakeLast(15)
            .ToList();
    }

    private static bool IsCurrentCombatEntry(string entry, Player player, Enemy target)
    {
        return entry.Contains(target.Name, StringComparison.OrdinalIgnoreCase) ||
            entry.Contains($"{player.Name} died", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsBattleLogEntry(string entry)
    {
        return entry.Contains("used", StringComparison.OrdinalIgnoreCase) ||
            entry.Contains("counterattacked", StringComparison.OrdinalIgnoreCase) ||
            entry.Contains("defeated", StringComparison.OrdinalIgnoreCase) ||
            entry.Contains("approached", StringComparison.OrdinalIgnoreCase) ||
            entry.Contains("died", StringComparison.OrdinalIgnoreCase) ||
            entry.Contains("too far", StringComparison.OrdinalIgnoreCase) ||
            entry.Contains("enraged", StringComparison.OrdinalIgnoreCase) ||
            entry.Contains("lost courage", StringComparison.OrdinalIgnoreCase);
    }

    private static ConsoleColor CreateBattleLogColor(string entry)
    {
        if (entry.Contains("counterattacked", StringComparison.OrdinalIgnoreCase))
        {
            return ConsoleColor.DarkRed;
        }

        if (entry.Contains("attacked", StringComparison.OrdinalIgnoreCase))
        {
            return ConsoleColor.DarkYellow;
        }

        if (entry.Contains("used", StringComparison.OrdinalIgnoreCase))
        {
            return ConsoleColor.Yellow;
        }

        if (entry.Contains("died", StringComparison.OrdinalIgnoreCase) ||
            entry.Contains("defeated", StringComparison.OrdinalIgnoreCase))
        {
            return ConsoleColor.Red;
        }

        if (entry.Contains("noise", StringComparison.OrdinalIgnoreCase) ||
            entry.Contains("heard", StringComparison.OrdinalIgnoreCase))
        {
            return ConsoleColor.Cyan;
        }

        return ConsoleColor.DarkGray;
    }

    private static void DrawCombatPlayerBox(ConsoleFrame frame, Player player)
    {
        DrawBox(frame, RightX, 1, RightWidth, 11, " PLAYER ");
        frame.WriteText(RightX + 2, 3, $"HP: {player.Health}", ConsoleColor.Green, RightWidth - 4);
        frame.WriteText(RightX + 2, 4, $"Fortune: {player.Luck}", ConsoleColor.Gray, RightWidth - 4);
        frame.WriteText(RightX + 2, 5, $"Might: {player.Strength}", ConsoleColor.Gray, RightWidth - 4);
        frame.WriteText(RightX + 2, 6, $"Fury: {player.Aggression}", ConsoleColor.Gray, RightWidth - 4);
        frame.WriteText(RightX + 2, 7, $"Wisdom: {player.Wisdom}", ConsoleColor.Gray, RightWidth - 4);
        frame.WriteText(RightX + 2, 8, $"Dexterity: {player.Dexterity}", ConsoleColor.Gray, RightWidth - 4);
        frame.WriteText(RightX + 2, 9, $"Armor: {player.Armor}", ConsoleColor.Gray, RightWidth - 4);
    }

    private static void DrawCombatHandsBox(ConsoleFrame frame, Player player)
    {
        DrawBox(frame, RightX, 13, RightWidth, 5, " HANDS ");
        frame.WriteText(RightX + 2, 15, $"L: {Trim(ItemName(player.LeftHand), RightWidth - 7)}", ConsoleColor.Gray, RightWidth - 4);
        frame.WriteText(RightX + 2, 16, $"R: {Trim(ItemName(player.RightHand), RightWidth - 7)}", ConsoleColor.Gray, RightWidth - 4);
    }

    private static void DrawCombatControlsBox(ConsoleFrame frame, LocalModel localModel)
    {
        var keys = localModel.KeyBindings;
        var textWidth = RightWidth - 3;
        DrawBox(frame, RightX, 19, RightWidth, 8, " CONTROLS ");
        var y = 21;
        if (localModel.IsChoosingAttackWeapon)
        {
            frame.WriteText(RightX + 2, y++, "Choose weapon to strike with:", ConsoleColor.DarkYellow, textWidth);
            frame.WriteText(RightX + 2, y++, $"[{KeyName(keys.ChooseLeftWeapon)}] Left Hand", ConsoleColor.Gray, textWidth);
            frame.WriteText(RightX + 2, y++, $"[{KeyName(keys.ChooseRightWeapon)}] Right Hand", ConsoleColor.Gray, textWidth);
            frame.WriteText(RightX + 2, y + 1, $"[{KeyName(keys.CancelAttack)}] Cancel", ConsoleColor.DarkGray, textWidth);
            return;
        }

        frame.WriteText(RightX + 2, y++, $"[{KeyName(keys.NormalAttack)}] Normal Attack", ConsoleColor.Gray, textWidth);
        frame.WriteText(RightX + 2, y++, $"[{KeyName(keys.StealthAttack)}] Stealth Attack", ConsoleColor.Gray, textWidth);
        frame.WriteText(RightX + 2, y++, $"[{KeyName(keys.MagicAttack)}] Magic Attack", ConsoleColor.Gray, textWidth);
        frame.WriteText(RightX + 2, y + 1, $"[{KeyName(keys.CancelAttack)}] Flee", ConsoleColor.DarkGray, textWidth);
    }

    private static Dictionary<BoardPoint, Enemy> CreateEnemyPositionIndex(GameModel gameModel)
    {
        var index = new Dictionary<BoardPoint, Enemy>();
        foreach (var enemy in gameModel.Enemies)
        {
            if (enemy.IsAlive)
            {
                index[new BoardPoint(enemy.X, enemy.Y)] = enemy;
            }
        }

        return index;
    }

    private static Dictionary<BoardPoint, Player> CreatePlayerPositionIndex(GameModel gameModel)
    {
        var index = new Dictionary<BoardPoint, Player>();
        foreach (var player in gameModel.Players)
        {
            if (player.IsAlive)
            {
                index[new BoardPoint(player.X, player.Y)] = player;
            }
        }

        return index;
    }

    private static RenderIcon GetTileIcon(ITile tile)
    {
        var visitor = new TileVisitor();
        tile.Accept(visitor);
        return visitor.Icon;
    }

    private static void DrawLogHistory(ConsoleFrame frame, GameModel gameModel, LocalModel localModel)
    {
        var boxX = 6;
        var boxY = 2;
        var boxWidth = Math.Max(2, Math.Min(ScreenWidth - 12, frame.Width - 12));
        var boxHeight = Math.Max(2, Math.Min(ScreenHeight - 4, frame.Height - 4));
        DrawBox(frame, boxX, boxY, boxWidth, boxHeight, " LOG HISTORY ");

        var entries = gameModel.EventLog.GetAll()
            .Concat(localModel.GetAllLocalMessages())
            .ToList();
        var maxLines = Math.Max(0, boxHeight - 5);
        var visibleEntries = entries.Skip(Math.Max(0, entries.Count - maxLines)).ToList();
        var y = boxY + 2;

        foreach (var entry in visibleEntries)
        {
            frame.WriteText(boxX + 2, y++, Trim(entry, boxWidth - 4), ConsoleColor.Gray, boxWidth - 4);
        }

        if (visibleEntries.Count == 0)
        {
            frame.WriteText(boxX + 2, y, "(no events)", ConsoleColor.DarkGray, boxWidth - 4);
        }

        var closeText =
            $"Press [{KeyName(localModel.KeyBindings.CloseLogHistory)}] to close log history";
        WriteCentered(frame, boxX + boxWidth / 2, boxY + boxHeight - 2, closeText, ConsoleColor.DarkGray);
    }

    private static int GetBoardStartX(Board board)
    {
        var availableWidth = RightX - (LeftX + LeftWidth);
        return Math.Max(0, LeftX + LeftWidth + (availableWidth - board.Width) / 2);
    }

    private static int GetBoardStartY(Board board)
    {
        return Math.Max(3, (ScreenHeight - board.Height) / 2);
    }

    private static void DrawBox(ConsoleFrame frame, int x, int y, int width, int height, string title)
    {
        if (width < 2 || height < 2)
        {
            return;
        }

        var horizontal = new string('─', width - 2);
        frame.WriteText(x, y, $"┌{horizontal}┐", ConsoleColor.DarkGray, width);
        for (var row = 1; row < height - 1; row++)
        {
            frame.WriteText(x, y + row, "│", ConsoleColor.DarkGray, 1);
            frame.WriteText(x + width - 1, y + row, "│", ConsoleColor.DarkGray, 1);
        }

        frame.WriteText(x, y + height - 1, $"└{horizontal}┘", ConsoleColor.DarkGray, width);
        if (string.IsNullOrWhiteSpace(title))
        {
            return;
        }

        var visibleTitle = Trim(title, width - 2);
        var titleX = x + (width - visibleTitle.Length) / 2;
        frame.WriteText(titleX, y, visibleTitle, ConsoleColor.Gray, visibleTitle.Length);
    }

    private static int WriteWrapped(
        ConsoleFrame frame,
        int x,
        int y,
        int width,
        int maxLines,
        string text,
        ConsoleColor color)
    {
        if (width <= 0 || maxLines <= 0)
        {
            return y;
        }

        var currentY = y;
        foreach (var line in Wrap(text, width).Take(maxLines))
        {
            frame.WriteText(x, currentY++, line, color, width);
        }

        return currentY;
    }

    private static IEnumerable<string> Wrap(string text, int width)
    {
        var words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var line = string.Empty;

        foreach (var word in words)
        {
            if (line.Length == 0)
            {
                line = Trim(word, width);
                continue;
            }

            if (line.Length + word.Length + 1 <= width)
            {
                line = $"{line} {word}";
                continue;
            }

            yield return line;
            line = Trim(word, width);
        }

        if (line.Length > 0)
        {
            yield return line;
        }
    }

    private void FlushFrame(ConsoleFrame frame)
    {
        if (frame.Width <= 0 || frame.Height <= 0)
        {
            return;
        }

        var output = new StringBuilder(frame.Width * frame.Height + frame.Height * 8);
        var currentColor = ConsoleCell.Empty.Color;
        output.Append(AnsiColor(currentColor));

        for (var y = 0; y < frame.Height; y++)
        {
            for (var x = 0; x < frame.Width; x++)
            {
                var cell = frame.Cells[y * frame.Width + x];
                if (cell.Color != currentColor)
                {
                    currentColor = cell.Color;
                    output.Append(AnsiColor(currentColor));
                }

                output.Append(cell.Symbol);
            }

            if (y < frame.Height - 1)
            {
                output.AppendLine();
            }
        }

        output.Append(AnsiReset);
        try
        {
            Console.SetCursorPosition(0, 0);
            Console.Write(output.ToString());
        }
        catch (ArgumentOutOfRangeException)
        {
        }
    }

    private static string AnsiColor(ConsoleColor color)
    {
        return AnsiColors.TryGetValue(color, out var code) ? code : AnsiColors[ConsoleColor.Gray];
    }

    private static void TryPrepareConsoleSurface()
    {
        if (!OperatingSystem.IsWindows())
        {
            return;
        }

        try
        {
            var width = Math.Max(ScreenWidth, Math.Max(Console.BufferWidth, Console.WindowWidth));
            var height = Math.Max(ScreenHeight, Math.Max(Console.BufferHeight, Console.WindowHeight));
            if (Console.BufferWidth < width || Console.BufferHeight < height)
            {
                Console.SetBufferSize(width, height);
            }
        }
        catch (IOException)
        {
        }
        catch (PlatformNotSupportedException)
        {
        }
        catch (ArgumentOutOfRangeException)
        {
        }
    }

    private static int GetFrameWidth()
    {
        return Math.Max(0, Math.Min(ScreenWidth, Math.Min(Console.WindowWidth, Console.BufferWidth)));
    }

    private static int GetFrameHeight()
    {
        return Math.Max(0, Math.Min(ScreenHeight, Math.Min(Console.WindowHeight, Console.BufferHeight)));
    }

    private static string ItemName(IItem? item)
    {
        return item == null ? "-" : item.Name;
    }

    private static string CreateSlotPathText(IItem rootItem, IReadOnlyList<int> slotPath, int maxWidth)
    {
        var names = new List<string> { CreateSlotPathPart(rootItem.Name, 1) };
        var currentItem = rootItem;
        var depth = 1;
        foreach (var slotIndex in slotPath)
        {
            if (slotIndex < 0 || slotIndex >= currentItem.Slots.Count)
            {
                break;
            }

            var slotItem = currentItem.Slots[slotIndex].Item;
            if (slotItem == null)
            {
                break;
            }

            depth++;
            names.Add(CreateSlotPathPart(slotItem.Name, depth));
            currentItem = slotItem;
        }

        var fullPath = string.Join(" > ", names);
        if (fullPath.Length <= maxWidth || names.Count <= 1)
        {
            return Trim(fullPath, maxWidth);
        }

        return Trim($"... > {names[^1]}", maxWidth);
    }

    private static string CreateSlotPathPart(string itemName, int depth)
    {
        return $"{itemName} ({depth})";
    }

    private static string CreateBonusText(PlayerStats bonuses)
    {
        var parts = new List<string>();
        AddBonusPart(parts, "STR", bonuses.Strength);
        AddBonusPart(parts, "HP", bonuses.Health);
        AddBonusPart(parts, "LUCK", bonuses.Luck);
        AddBonusPart(parts, "FURY", bonuses.Aggression);
        AddBonusPart(parts, "WIS", bonuses.Wisdom);
        AddBonusPart(parts, "DEX", bonuses.Dexterity);
        return parts.Count == 0 ? "Bonuses: none" : $"Bonuses: {string.Join(", ", parts)}";
    }

    private static void AddBonusPart(List<string> parts, string label, int value)
    {
        if (value == 0)
        {
            return;
        }

        var sign = value > 0 ? "+" : string.Empty;
        parts.Add($"{label} {sign}{value}");
    }

    private static int WalletAmount(Player player, string currencyName)
    {
        return player.Wallet.TryGetValue(currencyName, out var amount) ? amount : 0;
    }

    private static string KeyName(ConsoleKey key)
    {
        return KeyNames.TryGetValue(key, out var name) ? name : key.ToString();
    }

    private static string Trim(string text, int maxLength)
    {
        if (maxLength <= 0)
        {
            return string.Empty;
        }

        return text.Length <= maxLength ? text : text[..maxLength];
    }

    private sealed class ConsoleInputModeRenderer : IInputModeRenderer
    {
        private readonly ConsoleFrame _frame;
        private readonly GameModel _gameModel;
        private readonly LocalModel _localModel;

        public ConsoleInputModeRenderer(ConsoleFrame frame, GameModel gameModel, LocalModel localModel)
        {
            _frame = frame;
            _gameModel = gameModel;
            _localModel = localModel;
        }

        public void RenderInstructionMode()
        {
            DrawInstructions(_frame, _localModel);
        }

        public void RenderPlayingMode()
        {
            DrawPlayingView(_frame, _gameModel, _localModel);
        }

        public void RenderInventoryMode()
        {
            DrawPlayingView(_frame, _gameModel, _localModel);
        }

        public void RenderSlotMode()
        {
            DrawPlayingView(_frame, _gameModel, _localModel);
        }

        public void RenderLootMode()
        {
            DrawPlayingView(_frame, _gameModel, _localModel);
        }

        public void RenderCombatMode()
        {
            DrawCombatView(_frame, _gameModel, _localModel);
        }

        public void RenderWeaponMode()
        {
            DrawCombatView(_frame, _gameModel, _localModel);
        }

        public void RenderLogHistoryMode()
        {
            DrawLogHistory(_frame, _gameModel, _localModel);
        }

        public void RenderGameOverMode()
        {
            DrawGameOver(_frame, _localModel);
        }
    }

    private readonly record struct BoardPoint(int X, int Y);

    private readonly record struct ConsoleCell(string Symbol, ConsoleColor Color)
    {
        public static ConsoleCell Empty { get; } = new(" ", ConsoleColor.Gray);
    }

    private sealed class ConsoleFrame
    {
        public ConsoleFrame(int width, int height)
        {
            Width = Math.Max(0, width);
            Height = Math.Max(0, height);
            Cells = CreateEmptyCells(Width, Height);
        }

        public int Width { get; }
        public int Height { get; }
        public ConsoleCell[] Cells { get; }

        public static ConsoleCell[] CreateEmptyCells(int width, int height)
        {
            var cells = new ConsoleCell[Math.Max(0, width * height)];
            Array.Fill(cells, ConsoleCell.Empty);
            return cells;
        }

        public void WriteIcon(int x, int y, RenderIcon icon)
        {
            WriteCell(x, y, icon.Symbol, icon.Color);
        }

        public void WriteText(int x, int y, string text, ConsoleColor color, int maxWidth)
        {
            if (!IsInside(x, y))
            {
                return;
            }

            var availableWidth = Math.Min(maxWidth, Width - x);
            if (availableWidth <= 0)
            {
                return;
            }

            var visibleText = Trim(text, availableWidth).PadRight(availableWidth);
            for (var offset = 0; offset < visibleText.Length; offset++)
            {
                WriteCell(x + offset, y, visibleText[offset].ToString(), color);
            }
        }

        private void WriteCell(int x, int y, string symbol, ConsoleColor color)
        {
            if (!IsInside(x, y))
            {
                return;
            }

            Cells[y * Width + x] = new ConsoleCell(NormalizeSymbol(symbol), color);
        }

        private bool IsInside(int x, int y)
        {
            return x >= 0 && y >= 0 && x < Width && y < Height;
        }

        private static string NormalizeSymbol(string symbol)
        {
            return string.IsNullOrEmpty(symbol) ? " " : symbol[..1];
        }
    }
}
