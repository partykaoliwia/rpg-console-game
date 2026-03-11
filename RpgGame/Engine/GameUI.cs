using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RpgGame.Gameboard;

namespace RpgGame.Engine
{
    public class GameUI
    {
        private readonly int _startX;
        private readonly int _startY;
        private int WidthUI = 40;
        private int HeightUI = 30;
        public GameUI(int startX, int startY)
        {
            _startX = startX;
            _startY = startY;
        }
        public void Draw(Player.Player player, ITile currentTile, bool isInventoryMode, int inventCursorIndex, bool isLootMode, int lootCursorIndex)
        {
            int currentY = _startY;
            WriteAt(_startX, currentY++, "—————————————————");
            WriteAt(_startX, currentY++, "ATTRIBUTES");
            WriteAt(_startX, currentY++, $"Health: {player.Health}");
            WriteAt(_startX, currentY++, $"Strength: {player.Strength}");
            WriteAt(_startX, currentY++, $"Wisdom: {player.Wisdom}");
            WriteAt(_startX, currentY++, $"Luck: {player.Luck}");
            WriteAt(_startX, currentY++, $"Aggression: {player.Aggression}");
            WriteAt(_startX, currentY++, "—————————————————");
            WriteAt(_startX, currentY++, $"Coins: {player.Coins} Gold: {player.Gold}"); 
            WriteAt(_startX, currentY++, "—————————————————");
            WriteAt(_startX, currentY++, $"Left Hand:   {(player.LeftHand != null ? player.LeftHand.Name : " ")}");
            WriteAt(_startX, currentY++, $"Right Hand:  {(player.RightHand != null ? player.RightHand.Name : " ")}");
            WriteAt(_startX, currentY++, "—————————————————");
            string status = player.Inventory.Count >= player.MaxInventorySize ? "(FULL)" : $"({player.Inventory.Count}/{player.MaxInventorySize})";
            WriteAt(_startX, currentY++, $"INVENTORY {status}");
            if (player.Inventory.Count == 0)
            {
                WriteAt(_startX, currentY++, " ");
            }
            else
            {
                for (int i = 0; i < player.Inventory.Count; i++)
                {
                    string prefix = (isInventoryMode && i == inventCursorIndex) ? "> " : "  ";
                    WriteAt(_startX, currentY++, $"{prefix}{i + 1}. {player.Inventory[i].Name}");
                }
            }
            if (currentTile.Items.Count > 0)
            {
                WriteAt(_startX, currentY++, "—————————————————");
                WriteAt(_startX, currentY++, "ITEMS TO PICK UP");
                for (int i = 0; i < currentTile.Items.Count; i++)
                {
                    // NOWOŚĆ: Kursor dla łupów
                    string prefix = (isLootMode && i == lootCursorIndex) ? "> " : "  ";
                    WriteAt(_startX, currentY++, $"{prefix}{i + 1}. {currentTile.Items[i].Name}");
                }
            }
            else
            {
                WriteAt(_startX, currentY++, "");
            }
            //WriteAt(_startX, currentY++, "—————————————————");
            //if (isInventoryMode)
            //{
            //    WriteAt(_startX, currentY++, "INVENTORY MENU");
            //    WriteAt(_startX, currentY++, "[W/S] Up/Down  | [L] Left hand");
            //    WriteAt(_startX, currentY++, "[Q] Throw away | [R] Right hand");
            //    WriteAt(_startX, currentY++, "[I] Close inventory");
            //}
            //else if (isLootMode) // Instrukcje do zbierania!
            //{
            //    WriteAt(_startX, currentY++, "LOOTING MENU");
            //    WriteAt(_startX, currentY++, "[W/S] Up/Down");
            //    WriteAt(_startX, currentY++, "[E] Pick up selected");
            //    WriteAt(_startX, currentY++, "[Q] Cancel");
            //}
            //else
            //{
            //    WriteAt(_startX, currentY++, "[I] Open inventory menu");
            //    if (currentTile.Items.Count > 0) WriteAt(_startX, currentY++, "[E] Pick item up");
            //    else WriteAt(_startX, currentY++, "");
            //    WriteAt(_startX, currentY++, "");
            //    WriteAt(_startX, currentY++, "");
            //}

            for (int y = currentY; y < HeightUI; y++)
            {
                WriteAt(_startX, y, "");
            }
            int bottomX = 0;  // Rysujemy od lewej krawędzi (pod planszą)
            int bottomY = 21; // Plansza zajmuje 0-19. Zostawiamy linię 20 pustą, zaczynamy od 21

            if (isInventoryMode)
            {
                WriteAt(bottomX, bottomY++, "INVENTORY MENU");
                WriteAt(bottomX, bottomY++, "[W/S] Up/Down");
                WriteAt(bottomX, bottomY++, "[L] Equip Left Hand");
                WriteAt(bottomX, bottomY++, "[R] Equip Right Hand");
                WriteAt(bottomX, bottomY++, "[1] Empty Left");
                WriteAt(bottomX, bottomY++, "[2] Empty Right");
                WriteAt(bottomX, bottomY++, "[Q] Throw Away");
                WriteAt(bottomX, bottomY++, "[I] Close Inventory");
            }
            else if (isLootMode)
            {
                WriteAt(bottomX, bottomY++, "LOOTING MENU");
                WriteAt(bottomX, bottomY++, "[W/S] Up/Down");
                WriteAt(bottomX, bottomY++, "[E] Pick Up Selected");
                WriteAt(bottomX, bottomY++, "[ESC] Cancel");
            }
            else
            {
                WriteAt(bottomX, bottomY++, "CONTROLS");
                WriteAt(bottomX, bottomY++, "[W/A/S/D] Move");
                WriteAt(bottomX, bottomY++, "[I] Open Inventory");

                // Dynamiczna podpowiedź dla przycisku E
                if (currentTile.Items.Count == 1)
                {
                    WriteAt(bottomX, bottomY++, $"[E] Pick up {currentTile.Items[0].Name}");
                }
                else if (currentTile.Items.Count > 1)
                {
                    WriteAt(bottomX, bottomY++, "[E] Open Loot Menu");
                }
                else
                {
                    WriteAt(bottomX, bottomY++, ""); // Pusta linia, by uniknąć ghostingu
                }
                WriteAt(bottomX, bottomY++, "");
            }

            // Czyszczenie reszty DOLNEGO panelu (żeby stare instrukcje zniknęły po zmianie trybu)
            for (int y = bottomY; y < HeightUI; y++)
            {
                WriteAt(bottomX, y, "");
            }
        }
        private void WriteAt(int x, int y, string text)
        {
            Console.SetCursorPosition(x, y);
            Console.Write(text.PadRight(WidthUI));
        }

    }
}
