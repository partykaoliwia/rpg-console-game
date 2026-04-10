using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RpgGame.Common;

namespace RpgGame.Characters
{
    public class Enemy : IAttackable
    {
        public string Symbol { get; set; } = "■";
        public Color ItemColor { get; set; } = Color.DarkRed;
        public string Name { get; set; }
        public int Health { get; set; }
        public int Strength { get; set; }
        public int Armor { get; set; }
        public bool IsAlive => Health > 0;
        public Enemy(int health, int strength, int armor, string name = "Enemy")
        {
            Health = health;
            Strength = strength;
            Armor = armor;
            Name = name;
        }
        public int TakeDamage(int damage)
        {
            int actualDamage = Math.Max(0, damage - Armor);
            Health -= actualDamage;
            if(Health< 0) Health = 0;
            return actualDamage;
        }

    }
}
