// FILE: ArenaFactory.cs
using System;
using RpgGame.Characters;
using RpgGame.Items;
using RpgGame.Items.Modifiers;
using RpgGame.Items.Money;
using RpgGame.Items.Unusables;
using RpgGame.Items.Weapons;

namespace RpgGame.Engine.Builders
{
    public class ArenaFactory : IFactory
    {
        private readonly Random _random = new Random();
        public string ThemeName => "Ares' arena";
        public IGenerationStrategy GenerationStrategy => new ArenaGenStrategy();

        public string[] Title => new string[]
        {
            "           █████╗ ██████╗ ███████╗███████╗           ",
            "          ██╔══██╗██╔══██╗██╔════╝██╔════╝           ",
            "          ███████║██████╔╝█████╗  ███████╗           ",
            "          ██╔══██║██╔══██╗██╔══╝  ╚════██║           ",
            "          ██║  ██║██║  ██║███████╗███████║           ",
            "          ╚═╝  ╚═╝╚═╝  ╚═╝╚══════╝╚══════╝           ",
            "     █████╗ ██████╗ ███████╗███╗   ██╗ █████╗        ",
            "    ██╔══██╗██╔══██╗██╔════╝████╗  ██║██╔══██╗       ",
            "    ███████║██████╔╝█████╗  ██╔██╗ ██║███████║       ",
            "    ██╔══██║██╔══██╗██╔══╝  ██║╚██╗██║██╔══██║       ",
            "    ██║  ██║██║  ██║███████╗██║ ╚████║██║  ██║       ",
            "    ╚═╝  ╚═╝╚═╝  ╚═╝╚══════╝╚═╝  ╚═══╝╚═╝  ╚═╝       "
        };

        public string IntroMessage => "The scent of blood and sweat fills the air. You have entered the brutal Arena of Ares.";

        public IItem CreateRandomJunk()
        {
            return new SpartanModifier(new ShatteredShield());
        }

        public Currency CreateRandomCurrency()
        {
            int[] moneyValues = { 10, 20, 30, 50 };
            int value = moneyValues[_random.Next(moneyValues.Length)];
            return new Gold(value);
        }

        public IItem CreateRandomWeapon(int index)
        {
            int damage = 15 + _random.Next(0, 8);

            IItem weapon = new Kopis(damage);

            int modRoll = _random.Next(0, 3);
            return modRoll switch
            {
                0 => new SpartanModifier(weapon),
                1 => new HephaesteanModifier(weapon), 
                _ => weapon  
            };
        }

        public IAttackable CreateEnemy(int distanceFromStart)
        {
            if (distanceFromStart < 25)
                return new Enemy(30, 12, 2, "Enslaved Hoplite") { ItemColor = System.Drawing.Color.IndianRed, Symbol = "h" };

            if (distanceFromStart < 40)
                return new Enemy(45, 18, 5, "Blood-crazed Gladiator") { ItemColor = System.Drawing.Color.Firebrick, Symbol = "G" };

            return new Enemy(60, 25, 8, "Chosen of Ares") { ItemColor = System.Drawing.Color.DarkRed, Symbol = "A" };
        }

        public IAttackable CreateBoss()
        {
            var boss = new Enemy(120, 30, 10, "Deimos");
            boss.ItemColor = System.Drawing.Color.Crimson;
            boss.Symbol = "Δ"; 
            return boss;
        }
    }
}