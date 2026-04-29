using System;
using System.Collections.Generic;
using System.Text;
using Pastel;
using RpgGame.Characters;
using RpgGame.Common;
using RpgGame.Engine.Input;
using RpgGame.Engine.Logging;
using RpgGame.Gameboard;
using RpgGame.Items.Money;

namespace RpgGame.Engine
{
    public class ConsoleRenderer
    {
        private const int ScreenWidth = 120;
        private const int ScreenHeight = 29;
        private const int StatX = 45; 
        private const int StatY = 0;
        private const int BottomX = 0;
        private const int BottomY = 21;
        private const int LeftX = 2;
        private const int LeftWidth = 33;
        private const int BoardOffsetX = 35;
        private const int BoardOffsetY = 4;
        private const int RightX = 85;
        private const int RightWidth = 32;

        private readonly string[,] _buffer = new string[ScreenWidth, ScreenHeight];
        private readonly KeybindManager _keybinds;

        public ConsoleRenderer(KeybindManager keybinds)
        {
            _keybinds = keybinds;
            Console.CursorVisible = false;
            ClearBuffer();
        }

        public void ClearBuffer()
        {
            for (int y = 0; y < ScreenHeight; y++)
            {
                for (int x = 0; x < ScreenWidth; x++)
                {
                    _buffer[x, y] = " ";
                }
            }
        }

        // RYSOWANIE PLANSZY
        public void DrawBoard(Board board, Player player)
        {
            int availableWidth = RightX - (LeftX + LeftWidth);
            int boardStartX = (LeftX + LeftWidth) + (availableWidth - board.Width) / 2;
            int boardStartY = Math.Max(3, (ScreenHeight - board.Height) / 2);
            for (int y = 0; y < board.Height; y++)
            {
                for (int x = 0; x < board.Width; x++)
                {
                    int screenX = boardStartX + x;
                    int screenY = boardStartY + y;

                    if (player.X == x && player.Y == y)
                    {
                        DrawAt(screenX, screenY, player);
                    }
                    else
                    {
                        var tile = board.GetTile(x, y);
                        IDrawable thingToDraw;
                        if(tile.Items.Count > 0)
                        {
                            thingToDraw = tile.Items[0];
                        }
                        else
                        {
                            thingToDraw = tile;
                        }
                        if (tile.Entity != null)
                        {
                            thingToDraw = tile.Entity;
                        }
                        DrawAt(screenX, screenY, thingToDraw);
                    }
                }
            }
        }

        // RYSOWANIE STATYSTYK GRACZA
        public int DrawPlayerStats(Player player, int inventoryCursorIndex = -1)
        {
            // BOX 1: GRACZ
            int currentY = 1;
            DrawBox(RightX, currentY, RightWidth, 12, " PLAYER ");

            int coins = player.Wallet.ContainsKey(Coin.CurrencyName) ? player.Wallet[Coin.CurrencyName] : 0;
            int gold = player.Wallet.ContainsKey(Gold.CurrencyName) ? player.Wallet[Gold.CurrencyName] : 0;
            WriteToBuffer(RightX + 2, currentY + 2, $"©: {coins}");
            WriteToBuffer(RightX + 2, currentY + 3, $"¤: {gold}");

            WriteToBuffer(RightX + 2, currentY + 5, $"Health: {player.Health}");
            WriteToBuffer(RightX + 2, currentY + 6, $"Fortune: {player.Luck}");
            WriteToBuffer(RightX + 2, currentY + 7, $"Strength: {player.Strength}");
            WriteToBuffer(RightX + 2, currentY + 8, $"Fury: {player.Aggression}");
            WriteToBuffer(RightX + 2, currentY + 9, $"Wisdom: {player.Wisdom}");
            WriteToBuffer(RightX + 2, currentY + 10, $"Dexterity: {player.Dexterity}");

            // BOX 2: INVENTORY
            currentY += 12;
            if(player.Inventory.Count >= player.MaxInventorySize)
            {
                DrawBox(RightX, currentY, RightWidth, 10, " INVENTORY (FULL) ");

            }
            else
            {
                DrawBox(RightX, currentY, RightWidth, 10, " INVENTORY ");

            }
            for (int i = 0; i < player.MaxInventorySize && i < 10; i++)
            {
                string prefix = (i == inventoryCursorIndex) ? " ➤ " : "  ";
                string itemStr = i < player.Inventory.Count ? player.Inventory[i].Name : "";
                WriteToBuffer(RightX + 1, currentY + 2 + i, $"{prefix}{itemStr}");
            }

            // BOX 3: RĘCE
            currentY += 10;
            DrawBox(RightX, currentY, RightWidth, 5, " HANDS ");
            WriteToBuffer(RightX + 2, currentY + 2, $"L: {(player.LeftHand != null ? player.LeftHand.Name : "")}");
            WriteToBuffer(RightX + 2, currentY + 3, $"R: {(player.RightHand != null ? player.RightHand.Name : "")}");

            return currentY + 5;
        }

        // RYSOWANIE ITEMÓW NA ZIEMI
        public void DrawGroundItems(ITile tile, int currentY, int lootCursorIndex = -1)
        {
            int startY = 11;
            DrawBox(LeftX, startY, LeftWidth, 12, " ON THE GROUND ");
            if (tile.Items.Count > 0)
            {
                for (int i = 0; i < tile.Items.Count; i++)
                {
                    string prefix = (i == lootCursorIndex) ? " ➤ " : "  ";
                    WriteToBuffer(LeftX + 2, startY + 2 + i, $"{prefix}{tile.Items[i].Name}");
                }
            }
        }

        // RYSOWANIE MENU INVENTORY
        public void DrawInventoryMenu(Player player, int cursorIndex, ITile currentTile)
        {
            DrawBox(LeftX, 1, LeftWidth, 10, " CONTROLS ");
            string up = _keybinds.GetKeyName("Inventory", "MoveUp");
            string down = _keybinds.GetKeyName("Inventory", "MoveDown");
            string quit = _keybinds.GetKeyName("Inventory", "Quit");
            string drop = _keybinds.GetKeyName("Inventory", "Drop");
            string eqL = _keybinds.GetKeyName("Inventory", "EquipLeft");
            string eqR = _keybinds.GetKeyName("Inventory", "EquipRight");
            string uneqL = _keybinds.GetKeyName("Inventory", "UnequipLeft");
            string uneqR = _keybinds.GetKeyName("Inventory", "UnequipRight");


            int y = 3;
            WriteToBuffer(LeftX + 2, y++, $"[{up}/{down}] Up/Down");
            WriteToBuffer(LeftX + 2, y++, $"[{eqL}/{eqR}] Equip to L/R hand");
            WriteToBuffer(LeftX + 2, y++, $"[{uneqL}/{uneqR}] Unequip L/R hand");
            WriteToBuffer(LeftX + 2, y++, $"[{drop}] Drop");
            WriteToBuffer(LeftX + 2, y++, $"[{quit}] Return");

            DrawPlayerStats(player, cursorIndex); 
            DrawGroundItems(currentTile, 0);
            DrawRecentLogs();

        }

        // RYSOWANIE LOOTING MENU
        public void DrawLootingMenu(Player player, ITile currentTile, int cursorIndex)
        {
            DrawBox(LeftX, 1, LeftWidth, 10, " CONTROLS ");
            string up = _keybinds.GetKeyName("Looting", "MoveUp");
            string down = _keybinds.GetKeyName("Looting", "MoveDown");
            string quit = _keybinds.GetKeyName("Looting", "Quit");
            string collect = _keybinds.GetKeyName("Looting", "Collect");

            int y = 3;
            WriteToBuffer(LeftX + 2, y++, $"[{up}/{down}] Up/Down");
            WriteToBuffer(LeftX + 2, y++, $"[{collect}] Pick up");
            WriteToBuffer(LeftX + 2, y++, $"[{quit}] Return");

            DrawPlayerStats(player);
            DrawGroundItems(currentTile, 0, cursorIndex);
            DrawRecentLogs();

        }

        // CONTROLS + BOARD + STATS + GROUND ITEMS, CALY DEFAULT SCREEN
        public void DrawPlayingView(Board board, Player player)
        {
            DrawBox(LeftX, 1, LeftWidth, 10, " CONTROLS ");
            string up = _keybinds.GetKeyName("Playing", "MoveUp");
            string down = _keybinds.GetKeyName("Playing", "MoveDown");
            string left = _keybinds.GetKeyName("Playing", "MoveLeft");
            string right = _keybinds.GetKeyName("Playing", "MoveRight");
            string inv = _keybinds.GetKeyName("Playing", "OpenInventory");
            string collect = _keybinds.GetKeyName("Playing", "Collect");  
            string logs = _keybinds.GetKeyName("Playing", "LogHistory");
            int y = 3;
            WriteToBuffer(LeftX + 2, y++, $"[{up}/{left}/{down}/{right}] Move");
            WriteToBuffer(LeftX + 2, y++, $"[{inv}] Open inventory");
            WriteToBuffer(LeftX + 2, y++, $"[{logs}] Show logs");

            var tile = board.GetTile(player.X, player.Y);
            if (tile.Items.Count >= 1)
                WriteToBuffer(LeftX + 2, y++, $"[{collect}] Pick up");

            DrawBoard(board, player);
            DrawPlayerStats(player); 
            DrawGroundItems(tile, 0);
            DrawRecentLogs();
        }
        public void DrawRecentLogs()
        {
            DrawBox(LeftX, 23, LeftWidth, 5, " RECENT LOG ");

            // Pobieramy 1 najnowszy log z Singletona (upewnij się, że masz u góry: using RpgGame.Engine.Logging;)
            var logs = Logger.Instance.GetRecentLogs(1);
            if (logs.Count == 0) return;

            string lastLog = logs[0];
            int maxLen = LeftWidth - 4; // Bezpieczna szerokość dla tekstu (ok. 29 znaków)
            int maxLines = 3; // Mamy miejsce na 3 linijki w boxie o wysokości 5

            List<string> wrappedLines = new List<string>();
            string[] words = lastLog.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            string currentLine = "";

            // Algorytm Word-Wrap (zawijanie wierszy po słowach)
            foreach (var word in words)
            {
                // Sprawdzamy, czy jeśli dodamy to słowo (plus ewentualną spację), to przekroczymy limit
                if (currentLine.Length + word.Length + (currentLine.Length > 0 ? 1 : 0) <= maxLen)
                {
                    currentLine += (currentLine.Length > 0 ? " " : "") + word;
                }
                else
                {
                    // Linijka jest pełna, zapisujemy ją
                    if (currentLine.Length > 0)
                    {
                        wrappedLines.Add(currentLine);
                    }

                    // Zaczynamy nową linijkę od bieżącego słowa
                    currentLine = word;

                    // Zabezpieczenie: jeśli pojedyncze słowo jest dłuższe niż cały box (mało prawdopodobne, ale chroni przed błędem)
                    if (currentLine.Length > maxLen)
                    {
                        currentLine = currentLine.Substring(0, maxLen);
                    }
                }
            }

            // Dodajemy to, co zostało na końcu
            if (currentLine.Length > 0)
            {
                wrappedLines.Add(currentLine);
            }

            // Rysowanie wyliczonych linijek na ekranie
            int startY = 24; // Y zaraz pod górną ramką (która jest na 23)
            for (int i = 0; i < wrappedLines.Count && i < maxLines; i++)
            {
                WriteToBufferColored(LeftX + 2, startY + i, wrappedLines[i], System.Drawing.Color.DarkGray);
            }
        }
        // LOG HISTORY
        public void DrawLogHistoryScreen(IReadOnlyList<string> logs)
        {
            DrawBox(6,2,ScreenWidth-12,ScreenHeight-4," LOG HISTORY ");
            int startY = 4;
            int maxLines = ScreenHeight - 8;
            // Jeśli logów jest więcej niż mieści się na ekranie, pokaż tylko najnowsze z końca
            var logsToShow = logs.Skip(Math.Max(0, logs.Count - maxLines)).ToList();
            for(int i =0; i<logsToShow.Count; i++)
            {
                WriteToBuffer(12, startY + i, logsToShow[i]);
            }
            string closeKey = _keybinds.GetKeyName("LogHistory", "CloseLogs");
            string closeText = $"Press [{closeKey}] to close log history";
            WriteToBufferColored((ScreenWidth - closeText.Length) / 2, ScreenHeight - 2, closeText, System.Drawing.Color.DarkGray);
        }

        // MESSAGES
        public void DrawInstructionScreen(List<string> instructions, string[] title)
        {
            int totalHeight = title.Length + 1 + instructions.Count + 2 + 1;
            int currentY = Math.Max(1, (ScreenHeight - totalHeight) / 2);
            foreach (string line in title)
            {
                int startX = (ScreenWidth - line.Length) / 2;
                WriteToBufferColored(Math.Max(0, startX), currentY++, line, System.Drawing.Color.Gold);
            }

            currentY++;

            foreach (var line in instructions)
            {
                int startX = (ScreenWidth - line.Length) / 2;
                WriteToBuffer(Math.Max(0, startX), currentY++, line);
            }

            currentY += 2;
            string continueKey = _keybinds.GetKeyName("Instruction", "Continue");
            string continueText = $"Press [{continueKey}] to enter the labyrinth...";
            
            WriteToBufferColored((ScreenWidth - continueText.Length) / 2, currentY, continueText, System.Drawing.Color.DarkGoldenrod);
            var logs = Logger.Instance.GetRecentLogs(1);
            if (logs.Count > 0 && logs[0].Contains("Invalid key"))
            {
                string errorMsg = logs[0];
                WriteToBufferColored((ScreenWidth - errorMsg.Length) / 2, currentY + 2, errorMsg, System.Drawing.Color.Red);
            }
        }

        private void DrawBox(int x, int y, int width, int height, string title)
        {
            if (width < 2 || height < 2) return;

            string horizontal = new string('─', width - 2);
            WriteToBuffer(x, y, $"┌{horizontal}┐");
            
            for (int i = 1; i < height - 1; i++)
            {
                WriteToBuffer(x, y + i, "│");
                WriteToBuffer(x + width - 1, y + i, "│");
            }
            WriteToBuffer(x, y + height - 1, $"└{horizontal}┘");

            if (!string.IsNullOrEmpty(title))
            {
                int titlePos = x + (width - title.Length) / 2;
                WriteToBuffer(titlePos, y, title);
            }
        }
        private void DrawAt(int x, int y, IDrawable drawable)
        {
            if (x >= 0 && x < ScreenWidth && y >= 0 && y < ScreenHeight)
            {
                _buffer[x, y] = drawable.Symbol.ToString().Pastel(drawable.ItemColor);
            }
        }
        private void WriteToBuffer(int startX, int startY, string text)
        {
            for (int i = 0; i < text.Length; i++)
            {
                int x = startX + i;
                if (x >= 0 && x < ScreenWidth && startY >= 0 && startY < ScreenHeight)
                {
                    _buffer[x, startY] = text[i].ToString();
                }
            }
        }
        private void WriteToBufferColored(int startX, int startY, string text, System.Drawing.Color color)
        {
            for (int i = 0; i < text.Length; i++)
            {
                int x = startX + i;
                if (x >= 0 && x < ScreenWidth && startY >= 0 && startY < ScreenHeight)
                {
                    _buffer[x, startY] = text[i].ToString().Pastel(color);
                }
            }
        }
        // RENDEROWANIE
        public void RenderToConsole()
        {
            var sb = new StringBuilder(ScreenWidth * ScreenHeight + ScreenHeight);

            for (int y = 0; y < ScreenHeight; y++)
            {
                for (int x = 0; x < ScreenWidth; x++)
                {
                    sb.Append(_buffer[x, y]);
                }
                sb.AppendLine();
            }

            Console.SetCursorPosition(0, 0);
            Console.Write(sb.ToString());

            ClearBuffer();
        }

        // RYSOWANIE EKRANU WALKI
        public void DrawCombatScreen(Player player, IAttackable target, bool isChoosingWeapon)
        {
            int leftColX = LeftX;
            int rightColX = RightX;
            int colWidth = 32;
            int centerColX = 36;
            int centerColWidth = 47;

            // KOLUMNA LEWA

            // 1. LEWY GÓRNY - PRZECIWNIK
            DrawBox(leftColX, 1, colWidth, 10, $" {target.Name} ".ToUpper());
            WriteToBufferColored(leftColX + 2, 3, $"❤️: {Math.Max(0, target.Health)}", System.Drawing.Color.Red);
            WriteToBuffer(leftColX + 2, 4, $"Strength: {target.Strength}");
            WriteToBuffer(leftColX + 2, 5, $"Armor: {target.Armor}");

            string enemyIcon = $"[ {target.Symbol} ]";
            WriteToBufferColored(leftColX + (colWidth - enemyIcon.Length) / 2, 8, enemyIcon, target.ItemColor);

            // 2. LEWY DOLNY - COMBAT GUIDE
            string atk1 = _keybinds.GetKeyName("Combat", "NormalAttack");
            string atk2 = _keybinds.GetKeyName("Combat", "StealthAttack");
            string atk3 = _keybinds.GetKeyName("Combat", "MagicAttack");
            DrawBox(leftColX, 12, colWidth, 15, " COMBAT GUIDE ");
            int infoY = 14;
            WriteToBufferColored(leftColX + 2, infoY++, $"Normal Attack", System.Drawing.Color.DarkGray);
            WriteToBuffer(leftColX + 2, infoY++, "Dory: 1");
            WriteToBuffer(leftColX + 2, infoY++, "Other: Base");
            infoY++;
            WriteToBufferColored(leftColX + 2, infoY++, $"Stealth Attack", System.Drawing.Color.DarkGray);
            WriteToBuffer(leftColX + 2, infoY++, "Xiphos/Quill: 2 * base");
            WriteToBuffer(leftColX + 2, infoY++, "Labrys/Kopis: 0.5 * base");
            WriteToBuffer(leftColX + 2, infoY++, "Dory: 1");
            infoY++;
            WriteToBufferColored(leftColX + 2, infoY++, $"Magic Attack", System.Drawing.Color.DarkGray);
            WriteToBuffer(leftColX + 2, infoY++, "Other: 1");
            WriteToBuffer(leftColX + 2, infoY++, "Dory: Base");

            // PRAWA KOLUMNA

            // PRAWY GÓRNY - PLAYER
            DrawBox(rightColX, 1, colWidth, 11, " PLAYER ");
            WriteToBufferColored(rightColX + 2, 3, $"❤️: {player.Health}", System.Drawing.Color.Green);
            WriteToBuffer(rightColX + 2, 4, $"Fortune: {player.Luck}");
            WriteToBuffer(rightColX + 2, 5, $"Might: {player.Strength}");
            WriteToBuffer(rightColX + 2, 6, $"Fury: {player.Aggression}");
            WriteToBuffer(rightColX + 2, 7, $"Wisdom: {player.Wisdom}");

            // ŚRODEK - HANDS
            DrawBox(rightColX, 13, colWidth, 5, " HANDS ");
            string leftHandName = player.LeftHand != null ? player.LeftHand.Name : "Empty";
            string rightHandName = player.RightHand != null ? player.RightHand.Name : "Empty";

            if (leftHandName.Length > 25) leftHandName = leftHandName.Substring(0, 22) + "...";
            if (rightHandName.Length > 25) rightHandName = rightHandName.Substring(0, 22) + "...";

            WriteToBuffer(rightColX + 2, 15, $"L: {leftHandName}");
            WriteToBuffer(rightColX + 2, 16, $"R: {rightHandName}");

            // PRAWY DOLNY - CONTROLS
            DrawBox(rightColX, 19, colWidth, 8, " CONTROLS ");
            int ctrlY = 21;
            if (isChoosingWeapon)
            {
                string left = _keybinds.GetKeyName("Combat", "ChooseLeft");
                string right = _keybinds.GetKeyName("Combat", "ChooseRight");
                string esc = _keybinds.GetKeyName("Combat", "Flee");

                WriteToBufferColored(rightColX + 2, ctrlY++, "Choose weapon to strike with:", System.Drawing.Color.Gold);
                WriteToBuffer(rightColX + 2, ctrlY++, $"[{left}] Left Hand");
                WriteToBuffer(rightColX + 2, ctrlY++, $"[{right}] Right Hand");
                ctrlY++;
                WriteToBuffer(rightColX + 2, ctrlY++, $"[{esc}] Cancel");
            }
            else
            {
                string esc = _keybinds.GetKeyName("Combat", "Flee");

                WriteToBuffer(rightColX + 2, ctrlY++, $"[{atk1}] Normal Attack");
                WriteToBuffer(rightColX + 2, ctrlY++, $"[{atk2}] Stealth Attack");
                WriteToBuffer(rightColX + 2, ctrlY++, $"[{atk3}] Magic Attack");
                ctrlY++;
                WriteToBuffer(rightColX + 2, ctrlY++, $"[{esc}] Flee");
            }

            // ŚRODEK BATTLE LOG

            // FILE: ConsoleRenderer.cs (fragment z DrawCombatScreen)

            // ŚRODEK BATTLE LOG
            DrawBox(centerColX, 1, centerColWidth, 26, " BATTLE LOG ");
            var allLogs = Logger.Instance.GetRecentLogs(50);
            var battleLogs = new List<string>();

            foreach (var log in allLogs)
            {
                // Akceptujemy tylko logi zawierające te słowa:
                if (log.Contains("attacked") || log.Contains("counterattacks") || log.Contains("defeated") || log.Contains("approached"))
                {
                    battleLogs.Add(log);
                }
            }

            int maxBattleLogs = 15;
            var logsToDraw = battleLogs.Skip(Math.Max(0, battleLogs.Count - maxBattleLogs)).ToList();

            int logY = 3;
            int maxLen = centerColWidth - 4;

            foreach (var log in logsToDraw)
            {
                if (logY >= 26) break;

                // Przypisanie koloru na podstawie akcji
                var color = System.Drawing.Color.DarkGray;
                if (log.Contains("attacked")) color = System.Drawing.Color.Gold;
                else if (log.Contains("counterattacks")) color = System.Drawing.Color.LightCoral;
                else if (log.Contains("approached")) color = System.Drawing.Color.Cyan;

                string remaining = log.Trim();
                while (remaining.Length > 0 && logY < 26)
                {
                    string lineToPrint = remaining;
                    if (remaining.Length > maxLen)
                    {
                        int lastSpace = remaining.Substring(0, maxLen).LastIndexOf(' ');
                        if (lastSpace > 0)
                        {
                            lineToPrint = remaining.Substring(0, lastSpace);
                            remaining = remaining.Substring(lastSpace + 1).Trim();
                        }
                        else
                        {
                            lineToPrint = remaining.Substring(0, maxLen);
                            remaining = remaining.Substring(maxLen).Trim();
                        }
                    }
                    else
                    {
                        remaining = "";
                    }

                    WriteToBufferColored(centerColX + 2, logY++, lineToPrint, color);
                }
            }
        }
        public void DrawGameOverScreen(bool animate, string logFilePath)
        {
            string[] asciiDeath = new string[]
            {
                @"  _____          __  __  ______    ____ __      __ ______  _____  ",
                @" / ____|   /\   |  \/  ||  ____|  / __ \\ \    / /|  ____||  __ \ ",
                @"| |  __   /  \  | \  / || |__    | |  | |\ \  / / | |__   | |__) |",
                @"| | |_ | / /\ \ | |\/| ||  __|   | |  | | \ \/ /  |  __|  |  _  / ",
                @"| |__| |/ ____ \| |  | || |____  | |__| |  \  /   | |____ | | \ \ ",
                @" \_____/_/    \_\_|  |_||______|  \____/    \/    |______||_|  \_\"
            };

            int startY = (ScreenHeight - asciiDeath.Length - 4) / 2;
            int startX = (ScreenWidth - asciiDeath[0].Length) / 2;

            string youDied = "YOU DIED.";
            int youDiedX = (ScreenWidth - youDied.Length) / 2;
            int logX = (ScreenWidth - logFilePath.Length) / 2;

            string info = "Press any key to exit...";
            int infoX = (ScreenWidth - info.Length) / 2;

            if (animate)
            {
                ClearBuffer();
                Console.Clear();

                for (int i = 0; i < asciiDeath.Length; i++)
                {
                    Console.SetCursorPosition(startX, startY + i);
                    Console.Write(asciiDeath[i].Pastel(System.Drawing.Color.DarkRed));
                    Thread.Sleep(150);
                }

                Thread.Sleep(500);

                Console.SetCursorPosition(youDiedX, startY + asciiDeath.Length + 2);
                foreach (char c in youDied)
                {
                    Console.Write(c.ToString().Pastel(System.Drawing.Color.Red));
                    Thread.Sleep(90);
                }
                //Console.SetCursorPosition(logX, startY -2);
                //foreach(char c in logFilePath)
                //{
                //    Console.Write(c.ToString().Pastel(System.Drawing.Color.Maroon));
                //    Thread.Sleep(50);
                //}


                Console.SetCursorPosition(infoX, startY + asciiDeath.Length + 8);
                foreach (char c in info)
                {
                    Console.Write(c.ToString().Pastel(System.Drawing.Color.White));
                    Thread.Sleep(30);
                }
            }

            Thread.Sleep(200);
            for (int i = 0; i < asciiDeath.Length; i++)
            {
                WriteToBufferColored(startX, startY + i, asciiDeath[i], System.Drawing.Color.DarkRed);
            }
            WriteToBufferColored(youDiedX, startY + asciiDeath.Length + 2, youDied, System.Drawing.Color.Red);
            WriteToBufferColored(logX, startY -1, logFilePath, System.Drawing.Color.Maroon);
            WriteToBufferColored(infoX, startY + asciiDeath.Length + 8, info, System.Drawing.Color.White);
        }
    }
}