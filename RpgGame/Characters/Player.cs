using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RpgGame.Common;
using RpgGame.Gameboard;
using RpgGame.Items;
using RpgGame.Items.Money;

namespace RpgGame.Characters
{
    public class Player : IDrawable
    {
        public Color ItemColor { get; set; } = Color.White;
        public string Symbol => "¶";

        // wspolrzedne
        public int X { get; private set; }
        public int Y { get; private set; }

        // atrybuty
        public int Strength { get; set; } = 20; 
        public int Health { get; set; } = 20; 
        public int Luck { get; set; } = 20;
        public int Aggression { get; set; } = 20;
        public int Wisdom { get; set; } = 20; 
        public int Dexterity { get; set; } = 20;

        // waluta
        public Dictionary<string, int> Wallet { get; private set; } = new Dictionary<string, int>()
        {
            {Gold.CurrencyName,0 },
            {Coin.CurrencyName,0 }
        };

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
        public void AddCurrency(string currencyName, int amount)
        {
            if (Wallet.ContainsKey(currencyName))
            {
                Wallet[currencyName] += amount;
            }
            else
            {
                Wallet.Add(currencyName, amount);
            }
        }
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
                LeftHand.Unequip(this);
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
                RightHand.Unequip(this);
                Inventory.Add(RightHand);
            }

            RightHand = item;
            Inventory.Remove(item);
        }
        public void UnequipLeft()
        {
            if (LeftHand != null)
            {
                if (LeftHand == RightHand) 
                {
                    ClearHands();
                }
                else
                {
                    if (!TryAddToInventory(LeftHand))
                    {
                        return;
                    }
                    LeftHand.Unequip(this);
                    LeftHand = null;
                }
            }
        }
        public void UnequipRight()
        {
            if (RightHand != null)
            {
                if (RightHand == LeftHand) 
                {
                    ClearHands();
                }
                else
                {
                    if (!TryAddToInventory(RightHand))
                    {
                        return;
                    }
                    RightHand.Unequip(this);
                    RightHand = null;
                }
            }
        }
        public void ClearHands()
        {
            bool isTwoHanded = (LeftHand != null && LeftHand == RightHand);

            if (isTwoHanded)
            {
                if (TryAddToInventory(LeftHand))
                {
                    LeftHand.Unequip(this);
                    LeftHand = null;
                    RightHand = null;
                }
            }
            else
            {
                if (LeftHand != null)
                {
                    if (TryAddToInventory(LeftHand))
                    {
                        LeftHand.Unequip(this);
                        LeftHand = null;
                    }
                }

                if (RightHand != null)
                {
                    if (TryAddToInventory(RightHand))
                    {
                        RightHand.Unequip(this);
                        RightHand = null;
                    }
                }
            }
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
        public void TakeDamage(int damage)
        {
            Health -= damage;
            if(Health < 0) Health = 0;
        }
    }
}
