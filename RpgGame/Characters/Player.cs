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
    public class Player : IAttackable
    {
        public Color ItemColor { get; set; } = Color.White;
        public string Symbol => "¶";
        public string Name { get; set; } = "Tezeusz";
        public int Armor => 0;
        public bool IsAlive => Health > 0;

        // wspolrzedne
        public int X { get; private set; }
        public int Y { get; private set; }

        // atrybuty
        public int BaseStrength { get; set; } = 20; 
        public int BaseHealth { get; set; } = 20; 
        public int BaseLuck { get; set; } = 20;
        public int BaseAggression { get; set; } = 20;
        public int BaseWisdom { get; set; } = 20; 
        public int BaseDexterity { get; set; } = 20;

        public int Strength => BaseStrength + GetBonusStats().Strength;
        public int Health => Math.Max(0,BaseHealth + GetBonusStats().Health);
        public int Luck => BaseLuck + GetBonusStats().Luck;
        public int Aggression => BaseAggression + GetBonusStats().Aggression;
        public int Wisdom => BaseWisdom + GetBonusStats().Wisdom;
        public int Dexterity => BaseDexterity + GetBonusStats().Dexterity;
        public int MoveEveryMs => 0;
        public DateTime LastMoved { get; set; } = DateTime.Now;


        private PlayerStats GetBonusStats()
        {
            PlayerStats bonusStats = new PlayerStats();
            if (LeftHand != null)
            {
                bonusStats += LeftHand.Bonuses;
            }
            if (RightHand != null && RightHand != LeftHand)
            {
                bonusStats += RightHand.Bonuses;
            }
            return bonusStats;
        }

        public int TakeDamage(int damage)
        {
            if(!IsAlive) return 0;
            int actualDamage = Math.Max(0, damage - Armor);
            BaseHealth -= actualDamage;
            if (BaseHealth <= 0)
            {
                BaseHealth = 0;
                Die();
            }
            return actualDamage;
        }
        public void Die()
        {

        }

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
        public void SetPosition(int x, int y)
        {             
            X = x;
            Y = y;
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
        public IItem DropItem(int index)
        {
            if (index < 0 || index >= Inventory.Count) return null;
            var item = Inventory[index];
            Inventory.RemoveAt(index);
            return item;
        }
        public bool TryAddToInventory(IItem item)
        {
            if (Inventory.Count >= MaxInventorySize) return false;
            Inventory.Add(item);
            return true;
        }
    }
}
