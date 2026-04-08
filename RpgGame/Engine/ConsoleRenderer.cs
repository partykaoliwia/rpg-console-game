using System;
using System.Collections.Generic;
using System.Text;
using Pastel;
using RpgGame.Characters;
using RpgGame.Common;
using RpgGame.Engine.Input;
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
        private const int LeftWidth = 30;
        private const int BoardOffsetX = 35;
        private const int BoardOffsetY = 4;
        private const int RightX = 85;
        private const int RightWidth = 32;

        private readonly string[,] _buffer = new string[ScreenWidth, ScreenHeight];

        public ConsoleRenderer()
        {

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
            WriteToBuffer(RightX + 2, currentY + 9, $"Dexterity: {player.Dexterity}");

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
            int startY = 12;
            DrawBox(LeftX, startY, LeftWidth, 14, " ON THE GROUND ");
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
        public void DrawInventoryMenu(Player player, int cursorIndex, string errorMessage, ITile currentTile)
        {
            DrawBox(LeftX, 1, LeftWidth, 10, " CONTROLS ");
            string up = KeybindManager.Instance.GetKey("Inventory", "MoveUp").ToString();
            string down = KeybindManager.Instance.GetKey("Inventory", "MoveDown").ToString();
            string quit = KeybindManager.Instance.GetKey("Inventory", "Quit").ToString();
            string drop = KeybindManager.Instance.GetKey("Inventory", "Drop").ToString();
            string eqL = KeybindManager.Instance.GetKey("Inventory", "EquipLeft").ToString();
            string eqR = KeybindManager.Instance.GetKey("Inventory", "EquipRight").ToString();
            string uneqL = KeybindManager.Instance.GetKey("Inventory", "UnequipLeft").ToString();
            string uneqR = KeybindManager.Instance.GetKey("Inventory", "UnequipRight").ToString();

            int y = 3;
            WriteToBuffer(LeftX + 2, y++, $"[{up}/{down}] Up/Down");
            WriteToBuffer(LeftX + 2, y++, $"[{eqL}/{eqR}] Equip to L/R hand");
            WriteToBuffer(LeftX + 2, y++, $"[{uneqL}/{uneqR}] Unequip L/R hand");
            WriteToBuffer(LeftX + 2, y++, $"[{drop}] Drop");
            WriteToBuffer(LeftX + 2, y++, $"[{quit}] Return");

            DrawPlayerStats(player, cursorIndex); 
            DrawGroundItems(currentTile, 0);
            DrawMessage(errorMessage, ScreenHeight - 1);
        }

        // DRAWING LOOTING MENU
        public void DrawLootingMenu(Player player, ITile currentTile, int cursorIndex, string errorMessage)
        {
            DrawBox(LeftX, 1, LeftWidth, 10, " CONTROLS ");
            string up = KeybindManager.Instance.GetKey("Looting", "MoveUp").ToString();
            string down = KeybindManager.Instance.GetKey("Looting", "MoveDown").ToString();
            string quit = KeybindManager.Instance.GetKey("Looting", "Quit").ToString();
            string collect = KeybindManager.Instance.GetKey("Looting", "Collect").ToString();

            int y = 3;
            WriteToBuffer(LeftX + 2, y++, $"[{up}/{down}] Up/Down");
            WriteToBuffer(LeftX + 2, y++, $"[{collect}] Pick up");
            WriteToBuffer(LeftX + 2, y++, $"[{quit}] Return");

            DrawPlayerStats(player);
            DrawGroundItems(currentTile, 0, cursorIndex);
            DrawMessage(errorMessage, ScreenHeight - 1);
        }

        // MAIN DRAWING OF CONTROLS, STATES ETC
        public void DrawPlayingView(Board board, Player player, string errorMessage = "")
        {
            DrawBox(LeftX, 1, LeftWidth, 10, " CONTROLS ");
            string up = KeybindManager.Instance.GetKey("Playing", "MoveUp").ToString();
            string down = KeybindManager.Instance.GetKey("Playing", "MoveDown").ToString();
            string left = KeybindManager.Instance.GetKey("Playing", "MoveLeft").ToString();
            string right = KeybindManager.Instance.GetKey("Playing", "MoveRight").ToString();
            string inv = KeybindManager.Instance.GetKey("Playing", "OpenInventory").ToString();
            string collect = KeybindManager.Instance.GetKey("Playing", "Collect").ToString();

            int y = 3;
            WriteToBuffer(LeftX + 2, y++, $"[{up}/{left}/{down}/{right}] Move");
            WriteToBuffer(LeftX + 2, y++, $"[{inv}] Open inventory");

            var tile = board.GetTile(player.X, player.Y);
            if (tile.Items.Count >= 1)
                WriteToBuffer(LeftX + 2, y++, $"[{collect}] Pick up");

            DrawBoard(board, player);
            DrawPlayerStats(player); 
            DrawGroundItems(tile, 0); 
            DrawMessage(errorMessage, ScreenHeight - 1);
        }

        // DRAWING MESSAGES
        public void DrawMessage(string message, int line)
        {
            if (!string.IsNullOrEmpty(message))
            {
                string formatted = $"  {message}  ";
                int startX = (ScreenWidth - formatted.Length) / 2;
                WriteToBufferColored(startX, line, formatted, System.Drawing.Color.Red);
            }
        }
        public void DrawInstructionScreen(List<string> instructions, string errorMessage = "")
        {
            string[] asciiTitle = new string[]
            {
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
            };
            int totalHeight = asciiTitle.Length + 1 + instructions.Count + 2 + 1;
            int currentY = Math.Max(1, (ScreenHeight - totalHeight) / 2);
            foreach (string line in asciiTitle)
            {
                int startX = (ScreenWidth - line.Length) / 2;
                WriteToBufferColored(Math.Max(0, startX), currentY++, line, System.Drawing.Color.Gold);
            }

            currentY++; // Pusta linia pod tytułem

            // 3. Dynamiczne centrowanie każdej linii z listy instrukcji
            foreach (var line in instructions)
            {
                int startX = (ScreenWidth - line.Length) / 2;
                WriteToBuffer(Math.Max(0, startX), currentY++, line);
            }

            currentY += 2;
            string continueKey = KeybindManager.Instance.GetKey("Instruction", "Continue").ToString();
            string continueText = $"Press [{continueKey}] to enter the labyrinth...";
            
            WriteToBufferColored((ScreenWidth - continueText.Length) / 2, currentY, continueText, System.Drawing.Color.DarkGoldenrod);
            // Display errors (DrawMessage has built-in centering)
            DrawMessage(errorMessage, currentY + 2);
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
        public void DrawCombatScreen(Player player, Enemy enemy, string battleLog, bool isChoosingWeapon, string errorMessage = "")
        {
            // Wymiary i pozycje poszczególnych kolumn
            int leftColX = LeftX;           // 2
            int rightColX = RightX;         // 85
            int colWidth = 32;
            int centerColX = 36;            // Pomiędzy lewą a prawą (2+32+2 = 36)
            int centerColWidth = 47;        // Szerokość środka (85 - 36 - 2 = 47)

            // ==========================================
            // KOLUMNA LEWA
            // ==========================================

            // 1. LEWY GÓRNY - PRZECIWNIK
            DrawBox(leftColX, 1, colWidth, 10, $" {enemy.Name} ".ToUpper());
            WriteToBufferColored(leftColX + 2, 3, $"❤️: {Math.Max(0, enemy.HP)}", System.Drawing.Color.Red);
            WriteToBuffer(leftColX + 2, 4, $"Strength: {enemy.Strength}");
            WriteToBuffer(leftColX + 2, 5, $"Armor: {enemy.Armor}");

            string enemyIcon = $"[ {enemy.Symbol} ]";
            WriteToBufferColored(leftColX + (colWidth - enemyIcon.Length) / 2, 8, enemyIcon, enemy.ItemColor);

            // 2. BOTTOM LEFT - COMBAT KNOWLEDGE
            string atk1 = KeybindManager.Instance.GetKey("Combat", "NormalAttack").ToString();
            string atk2 = KeybindManager.Instance.GetKey("Combat", "StealthAttack").ToString();
            string atk3 = KeybindManager.Instance.GetKey("Combat", "MagicAttack").ToString();
            DrawBox(leftColX, 12, colWidth, 15, " COMBAT GUIDE ");
            int infoY = 14;
            WriteToBufferColored(leftColX + 2, infoY++, $"[{atk1}] Normal Attack", System.Drawing.Color.DarkGray);
            WriteToBuffer(leftColX + 2, infoY++, "Labrys/Xiphos: Base");
            WriteToBuffer(leftColX + 2, infoY++, "Dory: 1");
            infoY++;
            WriteToBufferColored(leftColX + 2, infoY++, $"[{atk2}] Stealth Attack", System.Drawing.Color.DarkGray);
            WriteToBuffer(leftColX + 2, infoY++, "Xiphos: 2 * base");
            WriteToBuffer(leftColX + 2, infoY++, "Labrys: 0.5 * base");
            WriteToBuffer(leftColX + 2, infoY++, "Dory: 1");
            infoY++;
            WriteToBufferColored(leftColX + 2, infoY++, $"[{atk3}] Magic Attack", System.Drawing.Color.DarkGray);
            WriteToBuffer(leftColX + 2, infoY++, "Dory: Base");
            WriteToBuffer(leftColX + 2, infoY++, "Labrys/Xiphos: 1");

            // ==========================================
            // RIGHT COLUMN
            // ==========================================

            // 3. TOP RIGHT - PLAYER
            DrawBox(rightColX, 1, colWidth, 11, " PLAYER ");
            WriteToBufferColored(rightColX + 2, 3, $"❤️: {player.Health}", System.Drawing.Color.Green);
            WriteToBuffer(rightColX + 2, 4, $"Fortune: {player.Luck}");
            WriteToBuffer(rightColX + 2, 5, $"Might: {player.Strength}");
            WriteToBuffer(rightColX + 2, 6, $"Fury: {player.Aggression}");
            WriteToBuffer(rightColX + 2, 7, $"Wisdom: {player.Wisdom}");

            // 4. BELOW PLAYER - HANDS
            DrawBox(rightColX, 13, colWidth, 5, " HANDS ");
            string leftHandName = player.LeftHand != null ? player.LeftHand.Name : "Empty";
            string rightHandName = player.RightHand != null ? player.RightHand.Name : "Empty";

            // Shorten names if they are too long to fit in box
            if (leftHandName.Length > 25) leftHandName = leftHandName.Substring(0, 22) + "...";
            if (rightHandName.Length > 25) rightHandName = rightHandName.Substring(0, 22) + "...";

            WriteToBuffer(rightColX + 2, 15, $"L: {leftHandName}");
            WriteToBuffer(rightColX + 2, 16, $"R: {rightHandName}");

            // 5. BOTTOM RIGHT - CONTROLS
            DrawBox(rightColX, 19, colWidth, 8, " CONTROLS ");
            int ctrlY = 21;
            if (isChoosingWeapon)
            {
                string left = KeybindManager.Instance.GetKey("Combat", "ChooseLeft").ToString();
                string right = KeybindManager.Instance.GetKey("Combat", "ChooseRight").ToString();
                string esc = KeybindManager.Instance.GetKey("Combat", "Flee").ToString();

                WriteToBufferColored(rightColX + 2, ctrlY++, "Choose weapon to strike with:", System.Drawing.Color.Gold);
                WriteToBuffer(rightColX + 2, ctrlY++, $"[{left}] Left Hand");
                WriteToBuffer(rightColX + 2, ctrlY++, $"[{right}] Right Hand");
                ctrlY++;
                WriteToBuffer(rightColX + 2, ctrlY++, $"[{esc}] Cancel");
            }
            else
            {
                string esc = KeybindManager.Instance.GetKey("Combat", "Flee").ToString();

                WriteToBuffer(rightColX + 2, ctrlY++, $"[{atk1}] Normal Attack");
                WriteToBuffer(rightColX + 2, ctrlY++, $"[{atk2}] Stealth Attack");
                WriteToBuffer(rightColX + 2, ctrlY++, $"[{atk3}] Magic Attack");
                ctrlY++;
                WriteToBuffer(rightColX + 2, ctrlY++, $"[{esc}] Flee");
            }

            // ==========================================
            // CENTER
            // ==========================================

            // 6. CENTER PANEL - BATTLE LOG
            DrawBox(centerColX, 1, centerColWidth, 26, " BATTLE LOG ");

            if (!string.IsNullOrEmpty(battleLog))
            {
                int logY = 3;
                // Split logs by " | "
                var logs = battleLog.Split(new[] { " | " }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var log in logs)
                {
                    string remaining = log.Trim();

                    // Wrap rows if text is longer than box width
                    while (remaining.Length > 0)
                    {
                        int maxLen = centerColWidth - 4;
                        string lineToPrint;

                        if (remaining.Length <= maxLen)
                        {
                            lineToPrint = remaining;
                            remaining = "";
                        }
                        else
                        {
                            // Find space to not cut words in half
                            int spaceIndex = remaining.LastIndexOf(' ', maxLen);
                            if (spaceIndex == -1) spaceIndex = maxLen;

                            lineToPrint = remaining.Substring(0, spaceIndex);
                            remaining = remaining.Substring(spaceIndex).Trim();
                        }

                        // Player attacks in gold, enemy attacks in red
                        var color = logY % 3 == 0 ? System.Drawing.Color.Gold : System.Drawing.Color.LightCoral;
                        WriteToBufferColored(centerColX + 2, logY++, lineToPrint, color);
                    }
                    logY++; // Empty line between entries
                }
            }

            // ==========================================
            // ERRORS
            // ==========================================
            DrawMessage(errorMessage, ScreenHeight - 1);
        }
        //public void DrawGameOverScreen()
        //{
        //    ClearBuffer(); // Czyści wszystko!

        //    // ASCII Art z podwójnymi ukośnikami (by C# ich nie pomylił ze znakami specjalnymi)
        //    string[] asciiDeath = new string[]
        //    {
        //        @"  _____          __  __  ______    ____ __      __ ______  _____  ",
        //        @" / ____|   /\   |  \/  ||  ____|  / __ \\ \    / /|  ____||  __ \ ",
        //        @"| |  __   /  \  | \  / || |__    | |  | |\ \  / / | |__   | |__) |",
        //        @"| | |_ | / /\ \ | |\/| ||  __|   | |  | | \ \/ /  |  __|  |  _  / ",
        //        @"| |__| |/ ____ \| |  | || |____  | |__| |  \  /   | |____ | | \ \ ",
        //        @" \_____/_/    \_\_|  |_||______|  \____/    \/    |______||_|  \_\",
        //        @"",
        //        @"                           YOU DIED.                            "
        //    };

        //    // Wyliczanie środka ekranu
        //    int startY = (ScreenHeight - asciiDeath.Length) / 2;
        //    int startX = (ScreenWidth - asciiDeath[0].Length) / 2;

        //    // Rysowanie na krwisto czerwono
        //    for (int i = 0; i < asciiDeath.Length; i++)
        //    {
        //        WriteToBufferColored(startX, startY + i, asciiDeath[i], System.Drawing.Color.DarkRed);
        //    }

        //    // Instrukcja na dole
        //    string info = "Wciśnij dowolny klawisz, aby zakończyć...";
        //    WriteToBuffer((ScreenWidth - info.Length) / 2, startY + asciiDeath.Length + 2, info);
        //}
        public void DrawGameOverScreen(bool animate)
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

            string info = "Press any key to exit...";
            int infoX = (ScreenWidth - info.Length) / 2;

            // ANIMATION PLAYED ONLY ONCE:
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
                    Thread.Sleep(120);
                }

                Thread.Sleep(800);

                Console.SetCursorPosition(infoX, startY + asciiDeath.Length + 8);
                foreach (char c in info)
                {
                    Console.Write(c.ToString().Pastel(System.Drawing.Color.White));
                    Thread.Sleep(30);
                }
            }

            // THIS ALWAYS EXECUTES (fills buffer for Engine, so screen doesn't flicker)
            for (int i = 0; i < asciiDeath.Length; i++)
            {
                WriteToBufferColored(startX, startY + i, asciiDeath[i], System.Drawing.Color.DarkRed);
            }
            WriteToBufferColored(youDiedX, startY + asciiDeath.Length + 2, youDied, System.Drawing.Color.Red);
            WriteToBufferColored(infoX, startY + asciiDeath.Length + 8, info, System.Drawing.Color.White);
        }
    }
}