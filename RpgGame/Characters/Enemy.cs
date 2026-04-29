using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RpgGame.Common;
using RpgGame.Engine.Observers;

namespace RpgGame.Characters
{
    public abstract class Enemy : IAttackable, ISpeciesSubscriber, ISoundSubscriber
    {
        public string Symbol { get; set; } = "■";
        public Color ItemColor { get; set; } = Color.DarkRed;
        public string Name { get; set; }
        public int Health { get; protected set; }
        public int Strength { get; protected set; }
        public int Armor { get; protected set; }
        public int X { get; private set; }
        public int Y { get; private set; }
        public bool IsAlive => Health > 0;
        private readonly SpeciesPublisher _species;
        public Enemy(int health, int strength, int armor, string symbol, Color itemColor, SpeciesPublisher species, string name = "Enemy")
        {
            Health = health;
            Strength = strength;
            Armor = armor;
            Symbol = symbol;
            ItemColor = itemColor;
            Name = name;

            _species = species;
            _species?.Subscribe(this);
        }
        public void SetPosition(int x, int y)
        {
            X = x;
            Y = y;
        }
        public int TakeDamage(int damage)
        {
            if(!IsAlive) return 0;

            int actualDamage = Math.Max(0, damage - Armor);
            Health -= actualDamage;
            if (Health <= 0)
            {
                Health = 0;
                this.Die();
            }
            return actualDamage;
        }
        public void Die()
        {
            _species.NotifySubscribers();
            _species.Unsubscribe(this);
        }

        public int MoveEveryMs { get; protected set; }
        public DateTime LastMoved { get; set; } = DateTime.Now;
        public abstract void UpdateOnSound(int sourceX, int sourceY, int range);

        public abstract void UpdateOnDeath();
    }
}
