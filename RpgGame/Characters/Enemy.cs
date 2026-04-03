using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RpgGame.Common;

namespace RpgGame.Characters
{
    public class Enemy : IDrawable
    {
        public string Symbol { get; set; } = "■";
        public Color ItemColor { get; set; } = Color.DarkRed;
        public string Name { get; set; }
        public int HP { get; set; }
        public int Strength { get; set; }
        public int Armor { get; set; }
        public bool IsAlive => HP > 0;
        public Enemy(int hp, int strength, int armor, string name = "Enemy")
        {
            HP = hp;
            Strength = strength;
            Armor = armor;
            Name = name;
        }
        public int TakeDamage(int damage)
        {
            int actualDamage = Math.Max(0, damage - Armor);
            HP -= actualDamage;
            return actualDamage;
        }

    }
}
