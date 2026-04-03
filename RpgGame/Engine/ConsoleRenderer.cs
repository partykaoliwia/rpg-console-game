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
                        else if(tile.Entity != null)
                        {
                            thingToDraw = tile.Entity;
                        }
                        else
                        {
                            thingToDraw = tile;
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
            DrawBox(RightX, currentY, RightWidth, 11, " PLAYER ");

            int coins = player.Wallet.ContainsKey(Coin.CurrencyName) ? player.Wallet[Coin.CurrencyName] : 0;
            int gold = player.Wallet.ContainsKey(Gold.CurrencyName) ? player.Wallet[Gold.CurrencyName] : 0;
            WriteToBuffer(RightX + 2, currentY + 2, $"Obole Charona: {coins}");
            WriteToBuffer(RightX + 2, currentY + 3, $"Złote Drachmy:  {gold}");


            WriteToBuffer(RightX + 2, currentY + 5, $"Wigor: {player.Health}");
            WriteToBuffer(RightX + 2, currentY + 6, $"Fortuna: {player.Luck}");
            WriteToBuffer(RightX + 2, currentY + 7, $"Krzepa Heraklesa: {player.Strength}");
            WriteToBuffer(RightX + 2, currentY + 8, $"Furia Aresa: {player.Aggression}");
            WriteToBuffer(RightX + 2, currentY + 9, $"Mądrość Ateny: {player.Wisdom}");

            // BOX 2: EKWIPUNEK
            currentY += 11;
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
            DrawBox(LeftX, 1, LeftWidth, 10, " MENU EKWIPUNKU ");
            string up = KeybindManager.Instance.GetKey("Inventory", "MoveUp").ToString();
            string down = KeybindManager.Instance.GetKey("Inventory", "MoveDown").ToString();
            string quit = KeybindManager.Instance.GetKey("Inventory", "Quit").ToString();
            string drop = KeybindManager.Instance.GetKey("Inventory", "Drop").ToString();
            string eqL = KeybindManager.Instance.GetKey("Inventory", "EquipLeft").ToString();
            string eqR = KeybindManager.Instance.GetKey("Inventory", "EquipRight").ToString();
            string uneqL = KeybindManager.Instance.GetKey("Inventory", "UnequipLeft").ToString();
            string uneqR = KeybindManager.Instance.GetKey("Inventory", "UnequipRight").ToString();

            int y = 3;
            WriteToBuffer(LeftX + 2, y++, $"[{up}/{down}] W górę/dół");
            WriteToBuffer(LeftX + 2, y++, $"[{eqL}/{eqR}] Włóż do L/P ręki");
            WriteToBuffer(LeftX + 2, y++, $"[{uneqL}/{uneqR}] Wyciągnij z L/P ręki");
            WriteToBuffer(LeftX + 2, y++, $"[{drop}] Wyrzuć na ziemię");
            WriteToBuffer(LeftX + 2, y++, $"[{quit}] Powrót");

            DrawPlayerStats(player, cursorIndex); 
            DrawGroundItems(currentTile, 0);
            DrawMessage(errorMessage, ScreenHeight - 1);
        }

        // RYSOWANIE MENU LOOTING
        public void DrawLootingMenu(Player player, ITile currentTile, int cursorIndex, string errorMessage)
        {
            DrawBox(LeftX, 1, LeftWidth, 10, " LOOT MENU ");
            string up = KeybindManager.Instance.GetKey("Looting", "MoveUp").ToString();
            string down = KeybindManager.Instance.GetKey("Looting", "MoveDown").ToString();
            string quit = KeybindManager.Instance.GetKey("Looting", "Quit").ToString();
            string collect = KeybindManager.Instance.GetKey("Looting", "Collect").ToString();

            int y = 3;
            WriteToBuffer(LeftX + 2, y++, $"[{up}/{down}] W górę/dół");
            WriteToBuffer(LeftX + 2, y++, $"[{collect}] Podnieś");
            WriteToBuffer(LeftX + 2, y++, $"[{quit}] Powrót");

            DrawPlayerStats(player);
            DrawGroundItems(currentTile, 0, cursorIndex);
            DrawMessage(errorMessage, ScreenHeight - 1);
        }

        // GŁÓWNE RYSOWANIE STEROWANIA, STATÓW ITD
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
            WriteToBuffer(LeftX + 2, y++, $"[{up}/{left}/{down}/{right}] Ruch");
            WriteToBuffer(LeftX + 2, y++, $"[{inv}] Otwórz ekwipunek");

            var tile = board.GetTile(player.X, player.Y);
            if (tile.Items.Count >= 1)
                WriteToBuffer(LeftX + 2, y++, $"[{collect}] Podnieś");

            DrawBoard(board, player);
            DrawPlayerStats(player); 
            DrawGroundItems(tile, 0); 
            DrawMessage(errorMessage, ScreenHeight - 1);
        }

        // RYSOWANIE KOMUNIKATÓW
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
         " ██╗      █████╗ ██████╗ ██╗██████╗ ██╗   ██╗███╗   ██╗████████╗",
         " ██║     ██╔══██╗██╔══██╗██║██╔══██╗╚██╗ ██╔╝████╗  ██║╚══██╔══╝",
         " ██║     ███████║██████╔╝██║██████╔╝ ╚████╔╝ ██╔██╗ ██║   ██║   ",
         " ██║     ██╔══██║██╔══██╗██║██╔══██╗  ╚██╔╝  ██║╚██╗██║   ██║   ",
         " ███████╗██║  ██║██████╔╝██║██║  ██║   ██║   ██║ ╚████║   ██║   ",
         " ╚══════╝╚═╝  ╚═╝╚═════╝ ╚═╝╚═╝  ╚═╝   ╚═╝   ╚═╝  ╚═══╝   ╚═╝   ",
         " ██████╗ ███████╗██████╗  █████╗ ██╗      █████╗",
         "  ██╔══██╗██╔════╝██╔══██╗██╔══██╗██║     ██╔══██╗",
         "  ██║  ██║█████╗  ██║  ██║███████║██║     ███████║",
         "  ██║  ██║██╔══╝  ██║  ██║██╔══██║██║     ██╔══██║",
         "  ██████╔╝███████╗██████╔╝██║  ██║███████╗██║  ██║",
         "  ╚═════╝ ╚══════╝╚═════╝ ╚═╝  ╚═╝╚══════╝╚═╝  ╚═╝"
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
            string continueText = $"Wciśnij [{continueKey}], aby wejść do labiryntu...";

            // Centrowanie tekstu "Wciśnij..."
            // WriteToBuffer((ScreenWidth - continueText.Length) / 2, currentY, continueText);
            WriteToBufferColored((ScreenWidth - continueText.Length) / 2, currentY, continueText, System.Drawing.Color.DarkGoldenrod);
            // Wyświetlanie błędów (DrawMessage ma wbudowane centrowanie)
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
            // 1. LEWY PANEL - INSTRUKCJE AKCJI
            DrawBox(LeftX, 1, LeftWidth, 10, " AKCJA ");
            int y = 3;

            if (isChoosingWeapon)
            {
                string left = KeybindManager.Instance.GetKey("Combat", "ChooseLeft").ToString();
                string right = KeybindManager.Instance.GetKey("Combat", "ChooseRight").ToString();
                string esc = KeybindManager.Instance.GetKey("Combat", "Flee").ToString();

                WriteToBuffer(LeftX + 2, y++, "Wybierz broń:");
                WriteToBuffer(LeftX + 2, y++, $"[{left}] Lewa Ręka");
                WriteToBuffer(LeftX + 2, y++, $"[{right}] Prawa Ręka");
                y++;
                WriteToBuffer(LeftX + 2, y++, $"[{esc}] Anuluj");
            }
            else
            {
                string atk1 = KeybindManager.Instance.GetKey("Combat", "NormalAttack").ToString();
                string atk2 = KeybindManager.Instance.GetKey("Combat", "StealthAttack").ToString();
                string atk3 = KeybindManager.Instance.GetKey("Combat", "MagicAttack").ToString();
                string esc = KeybindManager.Instance.GetKey("Combat", "Flee").ToString();

                WriteToBuffer(LeftX + 2, y++, $"[{atk1}] Zwykły Atak");
                WriteToBuffer(LeftX + 2, y++, $"[{atk2}] Skryty Atak");
                WriteToBuffer(LeftX + 2, y++, $"[{atk3}] Atak Magiczny");
                y++;
                WriteToBuffer(LeftX + 2, y++, $"[{esc}] Ucieczka");
            }

            // 2. PRAWY PANEL - STATYSTYKI GRACZA
            // Używamy Twojej gotowej metody - pokaże na bieżąco HP, staty i co mamy w rękach!
            DrawPlayerStats(player);

            // 3. ŚRODKOWY PANEL - PRZECIWNIK
            int centerAreaStart = LeftX + LeftWidth + 2;
            int centerAreaWidth = RightX - centerAreaStart - 2;
            int enemyBoxWidth = 30;
            int enemyBoxX = centerAreaStart + (centerAreaWidth - enemyBoxWidth) / 2;

            DrawBox(enemyBoxX, 4, enemyBoxWidth, 8, $" {enemy.Name} ");

            // Kolorujemy HP na czerwono żeby się wyróżniało
            WriteToBufferColored(enemyBoxX + 2, 6, $"Wigor (HP): {Math.Max(0, enemy.HP)}", System.Drawing.Color.Red);
            WriteToBuffer(enemyBoxX + 2, 7, $"Siła:       {enemy.Strength}");
            WriteToBuffer(enemyBoxX + 2, 8, $"Pancerz:    {enemy.Armor}");

            // Rysujemy "Ikonkę" wroga na środku ramki
            string enemyIcon = $"[ {enemy.Symbol} ]";
            WriteToBufferColored(enemyBoxX + (enemyBoxWidth - enemyIcon.Length) / 2, 10, enemyIcon, enemy.ItemColor);

            // 4. DOLNY PANEL - BATTLE LOG
            int logBoxY = 15;
            int logBoxHeight = 5;
            DrawBox(centerAreaStart, logBoxY, centerAreaWidth, logBoxHeight, " LOG WALKI ");

            if (!string.IsNullOrEmpty(battleLog))
            {
                // Sprytnie łamiemy logi na dwie linie, jeśli zawierają pionową kreskę kontrataku
                if (battleLog.Contains(" | "))
                {
                    var parts = battleLog.Split(new[] { " | " }, StringSplitOptions.None);
                    // Odcinamy nadmiar znaków by nie rozsadzić bufora ekranu
                    string line1 = parts[0].Length > centerAreaWidth - 4 ? parts[0].Substring(0, centerAreaWidth - 4) : parts[0];
                    string line2 = parts[1].Length > centerAreaWidth - 4 ? parts[1].Substring(0, centerAreaWidth - 4) : parts[1];

                    WriteToBufferColored(centerAreaStart + 2, logBoxY + 1, line1, System.Drawing.Color.Gold);
                    WriteToBufferColored(centerAreaStart + 2, logBoxY + 2, line2, System.Drawing.Color.LightCoral);
                }
                else
                {
                    string singleLine = battleLog.Length > centerAreaWidth - 4 ? battleLog.Substring(0, centerAreaWidth - 4) : battleLog;
                    WriteToBufferColored(centerAreaStart + 2, logBoxY + 2, singleLine, System.Drawing.Color.Gold);
                }
            }

            // 5. RYSOWANIE EWENTUALNYCH BŁĘDÓW (na samym dole)
            DrawMessage(errorMessage, ScreenHeight - 1);
        }
    }
}