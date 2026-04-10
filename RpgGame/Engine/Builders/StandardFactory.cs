using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RpgGame.Characters;
using RpgGame.Items;
using RpgGame.Items.Modifiers;
using RpgGame.Items.Money;
using RpgGame.Items.Unusables;

namespace RpgGame.Engine.Builders
{
    public class StandardFactory : IFactory
    {
        private readonly Random _random = new Random();
        public IItem CreateRandomJunk()
        {
            int roll = _random.Next(0, 3);
            return roll switch
            {
                0 => new IcarusFeather(),
                1 => new CyclopsEye(),
                _ => new AriadnesThread()
            };
        }

        public Currency CreateRandomCurrency()
        {
            int[] moneyValues = { 10, 10, 10, 10, 25, 25, 25, 35, 35, 50 };
            int value = moneyValues[_random.Next(moneyValues.Length)];
            return _random.Next(0, 2) == 0 ? new Gold(value) : new Coin(value);
        }

        public IItem CreateRandomWeapon(int index)
        {
            int damage = 10 + _random.Next(0, 4);
            IItem weapon;

            if (index == 0 || index == 2)
            {
                weapon = _random.Next(0, 2) == 0 ? new Xiphos(damage) : new Dory(damage);
            }
            else if (index == 1)
            {
                weapon = new Labrys(damage);
            }
            else
            {
                int roll = _random.Next(0, 3);
                if (roll == 0) weapon = new Xiphos(damage);
                else if (roll == 1) weapon = new Dory(damage);
                else weapon = new Labrys(damage);
            }

            int modRoll = _random.Next(0, 4);
            return modRoll switch
            {
                0 => new SpartanModifier(weapon),
                1 => new MidasTouchModifier(weapon),
                2 => new IcarianModifier(new DionysianModifier(weapon)),
                _ => new AthenianModifier(weapon)
            };
        }

        public IAttackable CreateEnemy(int distanceFromStart)
        {
            if (distanceFromStart < 25)
                return new Enemy(30, 20, 2);
            if (distanceFromStart < 40)
                return new Enemy(40, 13, 4);

            return new Enemy(50, 18, 8);
        }

        public IAttackable CreateBoss()
        {
            var boss = new Enemy(80, 15, 5, "Minotaur");
            boss.ItemColor = System.Drawing.Color.SaddleBrown;
            return boss;
        }
    }
}