using System;
using System.Text;
using Pastel;
using RpgGame.Characters;
using RpgGame.Common;
using RpgGame.Gameboard;

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
            for (int y = 0; y < board.Height; y++)
            {
                for (int x = 0; x < board.Width; x++)
                {
                    if (player.X == x && player.Y == y)
                    {
                        DrawAt(x, y, player);
                    }
                    else
                    {
                        var tile = board.GetTile(x, y);
                        IDrawable thingToDraw = tile.Items.Count > 0 ? tile.Items[0] : tile;
                        DrawAt(x, y, thingToDraw);
                    }
                }
            }
        }

        // RYSOWANIE STATYSTYK GRACZA
        public int DrawPlayerStats(Player player, int inventoryCursorIndex =-1)
        {
            int currentY = StatY;
            WriteToBuffer(StatX, currentY++, "—————————————————");
            WriteToBuffer(StatX, currentY++, "ATRYBUTY");
            WriteToBuffer(StatX, currentY++, $"Wigor witalny: {player.Health}");
            WriteToBuffer(StatX, currentY++, $"Krzepa Heraklesa: {player.Strength}");
            WriteToBuffer(StatX, currentY++, $"Mądrość Ateny: {player.Wisdom}");
            WriteToBuffer(StatX, currentY++, $"Fortuna: {player.Luck}");
            WriteToBuffer(StatX, currentY++, $"Furia Aresa: {player.Aggression}");
            WriteToBuffer(StatX, currentY++, "—————————————————");
            foreach(var currency in player.Wallet)
            {
                WriteToBuffer(StatX, currentY++, $"{currency.Key} : {currency.Value}");
            }
            WriteToBuffer(StatX, currentY++, "—————————————————");
            WriteToBuffer(StatX, currentY++, $"Left Hand:   {(player.LeftHand != null ? player.LeftHand.Name : "Empty")}");
            WriteToBuffer(StatX, currentY++, $"Right Hand:  {(player.RightHand != null ? player.RightHand.Name : "Empty")}");
            WriteToBuffer(StatX, currentY++, "—————————————————");

            string status = player.Inventory.Count >= player.MaxInventorySize ? "(FULL)" : $"({player.Inventory.Count}/{player.MaxInventorySize})";
            WriteToBuffer(StatX, currentY++, $"INVENTORY {status}");

            if (player.Inventory.Count == 0)
            {
                WriteToBuffer(StatX, currentY++, "  (Pusty)");
            }
            else
            {
                for (int i = 0; i < player.Inventory.Count; i++)
                {
                    string prefix = (i == inventoryCursorIndex) ? "> " : "  ";
                    WriteToBuffer(StatX, currentY++, $"{prefix}{i + 1}. {player.Inventory[i].Name}");
                }
            }

            return currentY;
        }

        // RYSOWANIE ITEMÓW NA ZIEMI
        public void DrawGroundItems(ITile tile, int currentY, int lootCursorIndex = -1)
        {
            if (tile.Items.Count > 0)
            {
                currentY++;
                WriteToBuffer(StatX, currentY++, "ON THE GROUND:");
                for (int i = 0; i < tile.Items.Count; i++)
                {
                    string prefix = (i == lootCursorIndex) ? "> " : "  ";
                    WriteToBuffer(StatX, currentY++, $"{prefix}{i + 1}. {tile.Items[i].Name}");
                }
            }
        }
        // RYSOWANIE MENU INVENTORY
        public void DrawInventoryMenu(Player player, int cursorIndex, string errorMessage)
        {
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

        // RYSOWANIE MENU LOOTING
        public void DrawLootingMenu(Player player, ITile currentTile, int cursorIndex, string errorMessage)
        {
            int nextY = DrawPlayerStats(player);
            DrawGroundItems(currentTile, nextY, cursorIndex);

            int bY = BottomY;
            WriteToBuffer(BottomX, bY++, "LOOTING MENU");
            WriteToBuffer(BottomX, bY++, "[W/S] Up/Down");
            WriteToBuffer(BottomX, bY++, "[E] Pick Up Selected");
            WriteToBuffer(BottomX, bY++, "[Q] Cancel");

            DrawMessage(errorMessage, bY + 1);
        }

        // GŁÓWNE RYSOWANIE STEROWANIA, STATÓW ITD
        public void DrawPlayingView(Board board, Player player, string errorMessage = "")
        {
            int nextY = DrawPlayerStats(player); // staty, ekwipunek
            var tile = board.GetTile(player.X, player.Y);
            DrawGroundItems(tile, nextY);        // przedmioty na ziemi

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

        // RYSOWANIE KOMUNIKATÓW
        public void DrawMessage(string message, int line)
        {
            if (!string.IsNullOrEmpty(message))
            {
                WriteToBuffer(BottomX, line, $"! {message} !");
            }
        }

        // metody pomocnicze
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
                if (x < ScreenWidth && startY < ScreenHeight)
                {
                    _buffer[x, startY] = text[i].ToString();
                }
            }
        }
        // RYSOWANIE EKRANU INSTRUKCJI
        public void DrawInstructionScreen(List<string> instructions, string errorMessage = "")
        {
            int currentY = 0; 
            int startX = 30;

            WriteToBuffer(startX-10, currentY++, "===============================================================================");
            WriteToBuffer(startX, currentY++, "                      LABIRYNT DEDALA                     ");
            WriteToBuffer(startX-10, currentY++, "===============================================================================");
            currentY++;

            foreach (var line in instructions)
            {
                WriteToBuffer(startX, currentY++, line);
            }

            currentY += 1;
            WriteToBuffer(startX, currentY, ">>> Wciśnij [ENTER], aby wejść do labiryntu... <<<");
            DrawMessage(errorMessage, currentY + 2);
        }
        private void WriteToBufferColored(int startX, int startY, string text, System.Drawing.Color color)
        {
            for (int i = 0; i < text.Length; i++)
            {
                int x = startX + i;
                if (x < ScreenWidth && startY < ScreenHeight)
                {
                    // Każdy znak pokolorowany z osobna
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