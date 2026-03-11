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
        public GameUI(int startX, int startY)
        {
            _startX = startX;
            _startY = startY;
        }
        public void Draw(Player.Player player, ITile currentTile)
        {
            int currentY = _startY;
            // ATRYBUTY
            WriteAt(_startX, currentY++, "=== STATYSTYKI ===");
            WriteAt(_startX, currentY++, $"Zdrowie:   {player.Health}");
            WriteAt(_startX, currentY++, $"Siła:      {player.Strength}");
            WriteAt(_startX, currentY++, $"Inteligencja:   {player.Intelligence}  | Agresja:   {player.Aggression}");
            WriteAt(_startX, currentY++, $"Szczęście: {player.Luck}");
            WriteAt(_startX, currentY++, "");
            // WALUTA
            WriteAt(_startX, currentY++, "=== PORTFEL ===");
            WriteAt(_startX, currentY++, $"Monety: {player.Coins}");
            WriteAt(_startX, currentY++, $"Złoto:  {player.Gold}");
            WriteAt(_startX, currentY++, "");
            // RĘCE
            WriteAt(_startX, currentY++, "=== WYPOSAŻENIE ===");
            WriteAt(_startX, currentY++, $"Lewa Ręka:  {(player.LeftHand != null ? player.LeftHand.Name : "Pusta")}");
            WriteAt(_startX, currentY++, $"Prawa Ręka: {(player.RightHand != null ? player.RightHand.Name : "Pusta")}");
            WriteAt(_startX, currentY++, "");
            // EKWIPUNEK
            WriteAt(_startX, currentY++, "=== PLECAK ===");
            if (player.Inventory.Count == 0)
            {
                WriteAt(_startX, currentY++, "Pusty");
            }
            else
            {
                for (int i = 0; i < player.Inventory.Count; i++)
                {
                    WriteAt(_startX, currentY++, $"{i + 1}. {player.Inventory[i].Name}");
                }
            }
            // Czyszczenie ewentualnych starych linii z dłuższego plecaka
            WriteAt(_startX, currentY++, "");
            WriteAt(_startX, currentY++, "");
            // Informacje o otoczeniu (Przedmioty na polu)
            WriteAt(_startX, currentY++, "=== OTOCZENIE ===");
            if (currentTile.Items.Count > 0)
            {
                WriteAt(_startX, currentY++, "Na ziemi leży:");
                foreach (var item in currentTile.Items)
                {
                    WriteAt(_startX, currentY++, $"- {item.Name} (Wciśnij 'E' by podnieść)");
                }
            }
            else
            {
                WriteAt(_startX, currentY++, "Nic tu nie ma.                        ");
                WriteAt(_startX, currentY++, "                                      "); // Nadpisanie resztek
            }
        }
        private void WriteAt(int x, int y, string text)
        {
            Console.SetCursorPosition(x, y);
            Console.Write(text.PadRight(WidthUI));
        }
    }
}
