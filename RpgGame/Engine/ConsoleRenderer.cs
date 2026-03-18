using System;
using System.Text;
using RpgGame.Characters;
using RpgGame.Common; // Konieczne do interfejsu IDrawable!
using RpgGame.Gameboard;

namespace RpgGame.Engine
{
    public class ConsoleRenderer
    {
        // Rozmiary całego okna konsoli (Plansza + UI)
        private const int ScreenWidth = 120;
        private static int ScreenHeight = 29;

        private const int StatX = 45;
        private const int StatY = 0;
        private const int BottomX = 0;
        private const int BottomY = 21;

        // Nasz wirtualny ekran w pamięci RAM!
        private readonly char[,] _buffer = new char[ScreenWidth, ScreenHeight];

        public ConsoleRenderer()
        {
            Console.CursorVisible = false;
            ClearBuffer();
        }

        // Czyści bufor w pamięci (zastępuje Console.Clear)
        public void ClearBuffer()
        {
            for (int y = 0; y < ScreenHeight; y++)
            {
                for (int x = 0; x < ScreenWidth; x++)
                {
                    _buffer[x, y] = ' ';
                }
            }
        }

        // 1. RYSOWANIE PLANSZY DO BUFORA (Używa IDrawable!)
        public void DrawBoard(Board board, Player player)
        {
            for (int y = 0; y < board.Height; y++)
            {
                for (int x = 0; x < board.Width; x++)
                {
                    if (player.X == x && player.Y == y)
                    {
                        DrawAt(x, y, player); // Przekazujemy gracza jako IDrawable
                    }
                    else
                    {
                        var tile = board.GetTile(x, y);
                        // Polimorfizm - rysujemy przedmiot LUB płytkę
                        IDrawable thingToDraw = tile.Items.Count > 0 ? tile.Items[0] : tile;
                        DrawAt(x, y, thingToDraw);
                    }
                }
            }
        }

        // 2. RYSOWANIE STATYSTYK GRACZA DO BUFORA
        public int DrawPlayerStats(Player player, int inventoryCursorIndex =-1)
        {
            int currentY = StatY;
            WriteToBuffer(StatX, currentY++, "—————————————————");
            WriteToBuffer(StatX, currentY++, "ATTRIBUTES");
            WriteToBuffer(StatX, currentY++, $"Health: {player.Health}");
            WriteToBuffer(StatX, currentY++, $"Strength: {player.Strength}");
            WriteToBuffer(StatX, currentY++, $"Wisdom: {player.Wisdom}");
            WriteToBuffer(StatX, currentY++, $"Luck: {player.Luck}");
            WriteToBuffer(StatX, currentY++, $"Aggression: {player.Aggression}");
            WriteToBuffer(StatX, currentY++, "—————————————————");
            WriteToBuffer(StatX, currentY++, $"Coins: {player.Coins} Gold: {player.Gold}");
            WriteToBuffer(StatX, currentY++, "—————————————————");
            WriteToBuffer(StatX, currentY++, $"Left Hand:   {(player.LeftHand != null ? player.LeftHand.Name : "Empty")}");
            WriteToBuffer(StatX, currentY++, $"Right Hand:  {(player.RightHand != null ? player.RightHand.Name : "Empty")}");
            WriteToBuffer(StatX, currentY++, "—————————————————");

            // Rysujemy ekwipunek ZAWSZE obok planszy
            string status = player.Inventory.Count >= player.MaxInventorySize ? "(FULL)" : $"({player.Inventory.Count}/{player.MaxInventorySize})";
            WriteToBuffer(StatX, currentY++, $"INVENTORY {status}");

            if (player.Inventory.Count == 0)
            {
                WriteToBuffer(StatX, currentY++, "  (Empty)");
            }
            else
            {
                for (int i = 0; i < player.Inventory.Count; i++)
                {
                    // Jeśli jesteśmy w InventoryState, inventoryCursorIndex narysuje strzałkę
                    string prefix = (i == inventoryCursorIndex) ? "> " : "  ";
                    WriteToBuffer(StatX, currentY++, $"{prefix}{i + 1}. {player.Inventory[i].Name}");
                }
            }

            return currentY; // Zwracamy pozycję Y!
        }

        // NOWA METODA: Zawsze wyświetla informację, czy stoimy na przedmiocie
        public void DrawGroundItems(ITile tile, int currentY, int lootCursorIndex = -1)
        {
            if (tile.Items.Count > 0)
            {
                currentY++; // Pusta linia odstępu
                WriteToBuffer(StatX, currentY++, "ON THE GROUND:");
                for (int i = 0; i < tile.Items.Count; i++)
                {
                    // Jeśli jesteśmy w LootingState, lootCursorIndex narysuje strzałkę
                    string prefix = (i == lootCursorIndex) ? "> " : "  ";
                    WriteToBuffer(StatX, currentY++, $"{prefix}{i + 1}. {tile.Items[i].Name}");
                }
            }
        }
        // 3. RYSOWANIE MENU EKWIPUNKU
        // 4. RYSOWANIE MENU EKWIPUNKU (Tylko nakładka sterowania)
        public void DrawInventoryMenu(Player player, int cursorIndex, string errorMessage)
        {
            // Przekazujemy kursor! Ekwipunek narysuje się ze wskaźnikiem
            DrawPlayerStats(player, cursorIndex);

            int bY = BottomY;
            WriteToBuffer(BottomX, bY++, "INVENTORY MENU");
            WriteToBuffer(BottomX, bY++, "[W/S] Up/Down");
            WriteToBuffer(BottomX, bY++, "[L/R] Equip L/R Hand");
            WriteToBuffer(BottomX, bY++, "[1/2] Empty L/R");
            WriteToBuffer(BottomX, bY++, "[Q] Drop");
            WriteToBuffer(BottomX, bY++, "[I] Close");

            DrawMessage(errorMessage, bY + 1);
        }

        // 4. RYSOWANIE MENU PODNOSZENIA (Looting)
        public void DrawLootingMenu(Player player, ITile currentTile, int cursorIndex, string errorMessage)
        {
            int nextY = DrawPlayerStats(player); // Ekwipunek bez kursora
            DrawGroundItems(currentTile, nextY, cursorIndex); // Ziemia z kursoerm!

            int bY = BottomY;
            WriteToBuffer(BottomX, bY++, "LOOTING MENU");
            WriteToBuffer(BottomX, bY++, "[W/S] Up/Down");
            WriteToBuffer(BottomX, bY++, "[E] Pick Up Selected");
            WriteToBuffer(BottomX, bY++, "[Q] Cancel");

            DrawMessage(errorMessage, bY + 1);
        }

        // 5. DYNAMICZNY TUTORIAL (Zwykła gra)
        // 3. RYSOWANIE WIDOKU GRY (Zastępuje dawne DrawTutorial)
        public void DrawPlayingView(Board board, Player player, string errorMessage = "")
        {
            int nextY = DrawPlayerStats(player); // Rysuje statystyki i ekwipunek (bez kursora)
            var tile = board.GetTile(player.X, player.Y);
            DrawGroundItems(tile, nextY);        // Rysuje przedmioty na ziemi (bez kursora)

            int bY = BottomY;
            WriteToBuffer(BottomX, bY++, "CONTROLS:");
            WriteToBuffer(BottomX, bY++, "[W/A/S/D] Move");
            WriteToBuffer(BottomX, bY++, "[I] Inventory");

            if (tile.Items.Count == 1)
                WriteToBuffer(BottomX, bY++, $"[E] Pick up {tile.Items[0].Name}");
            else if (tile.Items.Count > 1)
                WriteToBuffer(BottomX, bY++, "[E] Open Loot Menu");

            DrawMessage(errorMessage, bY + 1);
        }

        // 6. RYSOWANIE KOMUNIKATÓW
        public void DrawMessage(string message, int line)
        {
            if (!string.IsNullOrEmpty(message))
            {
                WriteToBuffer(BottomX, line, $"! {message} !");
            }
        }

        // === MAGIA DZIEJE SIĘ TUTAJ ===

        // NOWA METODA POMOCNICZA DLA IDRAWABLE
        private void DrawAt(int x, int y, IDrawable drawable)
        {
            if (x >= 0 && x < ScreenWidth && y >= 0 && y < ScreenHeight)
            {
                _buffer[x, y] = drawable.Symbol;
            }
        }

        // Prywatna metoda, która wpisuje stringa w odpowiednie miejsce tablicy
        private void WriteToBuffer(int startX, int startY, string text)
        {
            for (int i = 0; i < text.Length; i++)
            {
                int x = startX + i;
                if (x < ScreenWidth && startY < ScreenHeight)
                {
                    _buffer[x, startY] = text[i];
                }
            }
        }

        // Tę metodę wywołuje GameEngine na samym końcu pętli!
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

            Console.SetCursorPosition(0, 0); // Wracamy na górę konsoli
            Console.Write(sb.ToString());    // Wypluwamy całą klatkę naraz!

            ClearBuffer(); // Czyścimy wirtualny ekran przygotowując na kolejną klatkę
        }
    }
}