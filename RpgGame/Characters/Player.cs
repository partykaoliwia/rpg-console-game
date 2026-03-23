using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RpgGame.Common;
using RpgGame.Gameboard;
using RpgGame.Items;

namespace RpgGame.Characters
{
    public class Player : IDrawable
    {
        public char Symbol => '¶';

        // wspolrzedne
        public int X { get; private set; }
        public int Y { get; private set; }

        // atrybuty
        public int Strength { get; private set; } = 10; // Krzepa Heraklesa
        public int Health { get; private set; } = 20; // Wigor Witalny
        public int Luck { get; private set; } = 30; // Fortuna
        public int Aggression { get; private set; } = 40; // Furia Aresa
        public int Wisdom { get; private set; } = 50; // Mądrość Ateny

        // waluta
        public int Coins { get; private set; } = 0;
        public int Gold { get; private set; } = 0;

        // ekwipunek, rece
        public List<IItem> Inventory { get;  } = new List<IItem>();
        public IItem? LeftHand { get; private set; }
        public IItem? RightHand { get; private set; }
        public int MaxInventorySize { get; } = 6;

        // konstruktor
        public Player(int startX = 0, int startY = 0)
        {
            X = startX;
            Y = startY;
        }

        // metody
        public void Move(int dx, int dy, Board board) 
        {
            int newX = X + dx;
            int newY = Y + dy;

            ITile target = board.GetTile(newX, newY);
            if(target.IsWalkable)
            {
                X = newX;
                Y = newY;
            }    
        }
        public void AddCoins(int amount) => Coins += amount;
        public void AddGold(int amount) => Gold += amount;
        public void EquipToBothHands(IItem item)
        {
            ClearHands();
            LeftHand = item;
            RightHand = item;
            Inventory.Remove(item);
        }
        public void EquipToLeftHand(IItem item)
        {
            if (LeftHand != null && LeftHand == RightHand)
            {
                ClearHands();
            }
            else if (LeftHand != null)
            {
                Inventory.Add(LeftHand);
            }

            LeftHand = item;
            Inventory.Remove(item);
        }
        public void EquipToRightHand(IItem item)
        {
            if (RightHand != null && RightHand == LeftHand)
            {
                ClearHands();
            }
            else if (RightHand != null)
            {
                Inventory.Add(RightHand);
            }

            RightHand = item;
            Inventory.Remove(item);
        }
        public void UnequipLeft()
        {
            if (LeftHand != null)
            {
                if (LeftHand == RightHand) ClearHands();
                else
                {
                    Inventory.Add(LeftHand);
                    LeftHand = null;
                }
            }
        }
        public void UnequipRight()
        {
            if (RightHand != null)
            {
                if (RightHand == LeftHand) ClearHands();
                else
                {
                    Inventory.Add(RightHand);
                    RightHand = null;
                }
            }
        }
        public void ClearHands()
        {
            if (LeftHand != null) Inventory.Add(LeftHand);
            if (RightHand != null && RightHand != LeftHand) Inventory.Add(RightHand);

            LeftHand = null;
            RightHand = null;
        }
        public void CollectSpecific(int index, Board board)
        {
            ITile currentTile = board.GetTile(X, Y);
            if (index < 0 || index >= currentTile.Items.Count) return;
            var item = currentTile.Items[index];
            bool success = item.PickUp(this);
            if (success)
            {
                currentTile.Items.RemoveAt(index);
            }
        }
        public void DropItem(int index, Board board)
        {
            if (index < 0 || index >= Inventory.Count) return;
            var item = Inventory[index];
            Inventory.RemoveAt(index);
            board.GetTile(X, Y).Items.Add(item);
        }
        public bool TryAddToInventory(IItem item)
        {
            if (Inventory.Count >= MaxInventorySize) return false;
            Inventory.Add(item);
            return true;
        }
    }
}
