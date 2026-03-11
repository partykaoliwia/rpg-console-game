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
        public void Draw(Player.Player player, ITile currentTile, bool isInventoryMode, int cursorIndex)
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
            WriteAt(_startX, currentY++, "INVENTORY");
            if (player.Inventory.Count == 0)
            {
                WriteAt(_startX, currentY++, " ");
            }
            else
            {
                for (int i = 0; i < player.Inventory.Count; i++)
                {
                    string prefix = (isInventoryMode && i == cursorIndex) ? "> " : "  ";
                    WriteAt(_startX, currentY++, $"{prefix}{i + 1}. {player.Inventory[i].Name}");
                }
            }
            WriteAt(_startX, currentY++, "—————————————————");
            WriteAt(_startX, currentY++, "ITEMS TO PICK UP");
            if (currentTile.Items.Count > 0)
            {
                int i = 0;
                foreach (var item in currentTile.Items)
                {
                    WriteAt(_startX, currentY++, $"{i++}. {item.Name}");
                }
            }
            else
            {
                WriteAt(_startX, currentY++, "");
            }
            WriteAt(_startX, currentY++, "—————————————————");
            if (isInventoryMode)
            {
                WriteAt(_startX, currentY++, "INVENTORY MENU");
                WriteAt(_startX, currentY++, "[W/S] Up/Down  | [L] Left hand");
                WriteAt(_startX, currentY++, "[Q] Throw away | [R] Right hand");
                WriteAt(_startX, currentY++, "[I] Close inventory");
            }
            else
            {
                WriteAt(_startX, currentY++, "[I] Open inventory menu");
                WriteAt(_startX, currentY++, "[E] Pick item up");
                WriteAt(_startX, currentY++, "");
                WriteAt(_startX, currentY++, "");
                WriteAt(_startX, currentY++, "");
            }

            for (int y = currentY; y < HeightUI; y++)
            {
                WriteAt(_startX, y, "");
            }
        }
        private void WriteAt(int x, int y, string text)
        {
            Console.SetCursorPosition(x, y);
            Console.Write(text.PadRight(WidthUI));
        }

    }
}
