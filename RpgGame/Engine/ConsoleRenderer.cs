using System;
using System.Collections.Generic;
using System.Text;
using Pastel;
using RpgGame.Characters;
using RpgGame.Common;
using RpgGame.Engine.Input;
using RpgGame.Gameboard;
using RpgGame.Items;

namespace RpgGame.Engine
{
    public class ConsoleRenderer
    {
        private const int ScreenWidth = 120;
        private const int ScreenHeight = 29;

        // Stałe używane do zachowania oryginalnego API, 
        // ale wewnątrz metod i tak pozycjonujemy według nowego Layoutu.
        private const int StatX = 45; 
        private const int StatY = 0;
        private const int BottomX = 0;
        private const int BottomY = 21;

        // --- NOWY LAYOUT ---
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
        // RYSOWANIE PLANSZY
        public void DrawBoard(Board board, Player player)
        {
            // 1. Dostępna przestrzeń między lewą a prawą kolumną
            int availableWidth = RightX - (LeftX + LeftWidth);

            // 2. Wyśrodkowanie samej planszy (bez marginesów na ramkę)
            int boardStartX = (LeftX + LeftWidth) + (availableWidth - board.Width) / 2;
            int boardStartY = Math.Max(3, (ScreenHeight - board.Height) / 2);

            // 3. Tytuł gry wyśrodkowany precyzyjnie względem planszy
            //string title = "L A B I R Y N T   D E D A L A";
            //int titleX = boardStartX + (board.Width / 2) - (title.Length / 2);
            //WriteToBufferColored(titleX, boardStartY - 2, title, System.Drawing.Color.Gold);

            // 4. Rysowanie kafelków i gracza
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
            DrawBox(RightX, currentY, RightWidth, 11, " GRACZ ");

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
                DrawBox(RightX, currentY, RightWidth, 10, " EKWIPUNEK (FULL) ");

            }
            else
            {
                DrawBox(RightX, currentY, RightWidth, 10, " EKWIPUNEK ");

            }
            for (int i = 0; i < player.MaxInventorySize && i < 10; i++)
            {
                string prefix = (i == inventoryCursorIndex) ? " ➤ " : "  ";
                string itemStr = i < player.Inventory.Count ? player.Inventory[i].Name : "";
                WriteToBuffer(RightX + 1, currentY + 2 + i, $"{prefix}{itemStr}");
            }

            // BOX 3: RĘCE
            currentY += 10;
            DrawBox(RightX, currentY, RightWidth, 5, " RĘCE ");
            WriteToBuffer(RightX + 2, currentY + 2, $"L: {(player.LeftHand != null ? player.LeftHand.Name : "")}");
            WriteToBuffer(RightX + 2, currentY + 3, $"P: {(player.RightHand != null ? player.RightHand.Name : "")}");

            return currentY + 5; // Zgodnie z sygnaturą zwraca int (następną pozycję Y)
        }

        // RYSOWANIE ITEMÓW NA ZIEMI
        public void DrawGroundItems(ITile tile, int currentY, int lootCursorIndex = -1)
        {
            // Parametr currentY ignorujemy wizualnie na rzecz sztywnego layoutu, 
            // ale nie zmieniamy sygnatury by zachować kompatybilność.
            int startY = 12;
            DrawBox(LeftX, startY, LeftWidth, 14, " NA ZIEMI ");

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

            // Narysuj resztę widoku
            DrawPlayerStats(player, cursorIndex); 
            DrawGroundItems(currentTile, 0);
            DrawMessage(errorMessage, ScreenHeight - 1);
        }

        // RYSOWANIE MENU LOOTING
        public void DrawLootingMenu(Player player, ITile currentTile, int cursorIndex, string errorMessage)
        {
            DrawBox(LeftX, 1, LeftWidth, 10, " MENU PODNOSZENIA ");
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
            DrawBox(LeftX, 1, LeftWidth, 10, " INSTRUKCJE ");
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

        // RYSOWANIE EKRANU INSTRUKCJI
        //public void DrawInstructionScreen(List<string> instructions, string errorMessage = "")
        //{
        //    int currentY = 2; 
        //    int startX = 25;

        //    WriteToBuffer(startX, currentY++, "=====================================================================");
        //    WriteToBufferColored(startX, currentY++, "                      LABIRYNT DEDALA                      ", System.Drawing.Color.Gold);
        //    WriteToBuffer(startX, currentY++, "=====================================================================");
        //    currentY++;

        //    foreach (var line in instructions)
        //    {
        //        WriteToBuffer(startX, currentY++, line);
        //    }

        //    currentY += 2;
        //    string continueKey = KeybindManager.Instance.GetKey("Instruction", "Continue").ToString();
        //    WriteToBuffer(startX, currentY, $">>> Wciśnij [{continueKey}], aby wejść do labiryntu... <<<");
        //    DrawMessage(errorMessage, currentY + 2);
        //}
        //public void DrawInstructionScreen(List<string> instructions, string errorMessage = "")
        //{
        //    // 1. Obliczamy całkowitą wysokość tego widoku, aby wyśrodkować go również w PIONIE
        //    // 3 linie nagłówka + 1 pusta + liczba linii instrukcji + 2 puste + 1 linia "continue"
        //    int totalHeight = 3 + 1 + instructions.Count + 2 + 1;

        //    // Zaczynamy rysować od połowy ekranu minus połowa wysokości naszego bloku tekstu
        //    int currentY = Math.Max(0, (ScreenHeight - totalHeight) / 2);

        //    string headerTop = "=====================================================================";
        //    string headerMid = "                      LABIRYNT DEDALA                      ";
        //    string headerBot = "=====================================================================";

        //    // 2. Wyśrodkowanie nagłówków w POZIOMIE
        //    WriteToBuffer((ScreenWidth - headerTop.Length) / 2, currentY++, headerTop);
        //    WriteToBufferColored((ScreenWidth - headerMid.Length) / 2, currentY++, headerMid, System.Drawing.Color.Gold);
        //    WriteToBuffer((ScreenWidth - headerBot.Length) / 2, currentY++, headerBot);
        //    currentY++;

        //    // 3. Dynamiczne centrowanie każdej linii z listy (Twój pomysł z DrawDescription!)
        //    foreach (var line in instructions)
        //    {
        //        int startX = (ScreenWidth - line.Length) / 2;
        //        WriteToBuffer(Math.Max(0, startX), currentY++, line);
        //    }

        //    currentY += 2;
        //    string continueKey = KeybindManager.Instance.GetKey("Instruction", "Continue").ToString();
        //    string continueText = $">>> Wciśnij [{continueKey}], aby wejść do labiryntu... <<<";

        //    // Centrowanie tekstu "Wciśnij..."
        //    WriteToBuffer((ScreenWidth - continueText.Length) / 2, currentY, continueText);

        //    // Wyświetlanie błędów (Twoja metoda DrawMessage ma już wbudowane centrowanie)
        //    DrawMessage(errorMessage, currentY + 2);
        //}
        public void DrawInstructionScreen(List<string> instructions, string errorMessage = "")
        {
            // --- DEFINICJA TYTUŁU ASCII (6 linijek wysokości, szerokość ~60 znaków) ---
            // Używamy solidnych znaków ██╗, aby uzyskać gruby, "klockowy" efekt jak w Twoim przykładzie.
            string[] asciiTitle = new string[]
            {
         " ██╗      █████╗ ██████╗ ██╗██████╗ ██╗   ██╗███╗   ██╗████████╗",
         " ██║     ██╔══██╗██╔══██╗██║██╔══██╗╚██╗ ██╔╝████╗  ██║╚══██╔══╝",
         " ██║     ███████║██████╔╝██║██████╔╝ ╚████╔╝ ██╔██╗ ██║   ██║   ",
         " ██║     ██╔══██║██╔══██╗██║██╔══██╗  ╚██╔╝  ██║╚██╗██║   ██║   ",
         " ███████╗██║  ██║██████╔╝██║██║  ██║   ██║   ██║ ╚████║   ██║   ",
         " ╚══════╝╚═╝  ╚═╝╚═════╝ ╚═╝╚═╝  ╚═╝   ╚═╝   ╚═╝  ╚═══╝   ╚═╝   ", // Pusta linia między wyrazami
         " ██████╗ ███████╗██████╗  █████╗ ██╗      █████╗",
         "  ██╔══██╗██╔════╝██╔══██╗██╔══██╗██║     ██╔══██╗",
         "  ██║  ██║█████╗  ██║  ██║███████║██║     ███████║",
         "  ██║  ██║██╔══╝  ██║  ██║██╔══██║██║     ██╔══██║",
         "  ██████╔╝███████╗██████╔╝██║  ██║███████╗██║  ██║",
         "  ╚═════╝ ╚══════╝╚═════╝ ╚═╝  ╚═╝╚══════╝╚═╝  ╚═╝"
            };

            // 1. Obliczamy całkowitą wysokość tego widoku, aby wyśrodkować go w PIONIE (Y)
            // Wysokość ASCII (12) + Pusta (1) + Instrukcje + Puste (2) + "Continue" (1)
            int totalHeight = asciiTitle.Length + 1 + instructions.Count + 2 + 1;

            // Zaczynamy rysować od połowy ekranu minus połowa wysokości naszego bloku tekstu
            int currentY = Math.Max(1, (ScreenHeight - totalHeight) / 2);

            // 2. Rysujemy tytuł ASCII (wyśrodkowany w POZIOMIE w Gold)
            // Wybieramy Gold, by nawiązać do "Mitologii Greckiej".
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

        // --- PRYWATNE METODY POMOCNICZE ---

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
    }
}