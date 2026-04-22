// FILE: LibraryFactory.cs
using System;
using RpgGame.Characters;
using RpgGame.Items;
using RpgGame.Items.Modifiers;
using RpgGame.Items.Money;
using RpgGame.Items.Unusables;
using RpgGame.Items.Weapons;

namespace RpgGame.Engine.Builders
{
    public class LibraryFactory : IFactory
    {
        private readonly Random _random = new Random();
        public string ThemeName => "Library of Alexandria";
        public IGenerationStrategy GenerationStrategy => new LibraryGenStrategy();
        public string[] Title =>  new string[]
        {
            " █████╗ ██╗     ███████╗██╗  ██╗ █████╗ ███╗   ██╗██████╗ ██████╗ ██╗  █████╗ ",
            "██╔══██╗██║     ██╔════╝╚██╗██╔╝██╔══██╗████╗  ██║██╔══██╗██╔══██╗██║ ██╔══██╗",
            "███████║██║     █████╗   ╚███╔╝ ███████║██╔██╗ ██║██║  ██║██████╔╝██║ ███████║",
            "██╔══██║██║     ██╔══╝   ██╔██╗ ██╔══██║██║╚██╗██║██║  ██║██╔══██╗██║ ██╔══██║",
            "██║  ██║███████╗███████╗██╔╝ ██╗██║  ██║██║ ╚████║██████╔╝██║  ██║██║ ██║  ██║",
            "╚═╝  ╚═╝╚══════╝╚══════╝╚═╝  ╚═╝╚═╝  ╚═╝╚═╝  ╚═══╝╚═════╝ ╚═╝  ╚═╝╚═╝ ╚═╝  ╚═╝",
            "            ██╗     ██╗ ██████╗ ██████╗  █████╗ ██████╗ ██╗   ██╗             ",
            "            ██║     ██║ ██╔══██╗██╔══██╗██╔══██╗██╔══██╗╚██╗ ██╔╝             ",
            "            ██║     ██║ ██████╔╝██████╔╝███████║██████╔╝ ╚████╔╝              ",
            "            ██║     ██║ ██╔══██╗██╔══██╗██╔══██║██╔══██╗  ╚██╔╝               ",
            "            ███████╗██║ ██████╔╝██║  ██║██║  ██║██║  ██║   ██║                ",
            "            ╚══════╝╚═╝ ╚═════╝ ╚═╝  ╚═╝╚═╝  ╚═╝╚═╝  ╚═╝   ╚═╝                "
        };
        public string IntroMessage => "The scent of ancient papyrus fills the air. You have entered the lost Library of Alexandria.";

        public IItem CreateRandomJunk()
        {
            return new AthenianModifier(new OldBook());
        }

        public Currency CreateRandomCurrency()
        {
            int[] moneyValues = { 5, 10, 15, 20 };
            int value = moneyValues[_random.Next(moneyValues.Length)];
            return new Coin(value);
        }

        public IItem CreateRandomWeapon(int index)
        {
            int damage = 10 + _random.Next(0, 6);

            IItem weapon = new StymphalianQuill(damage);

            int modRoll = _random.Next(0, 3);
            return modRoll switch
            {
                0 => new AthenianModifier(weapon),    // dodaje mądrość
                1 => new PrometheanModifier(weapon),  // mądrość kosztem zdrowia
                _ => weapon                           // czyste, śmiercionośne pióro
            };
        }

        public IAttackable CreateEnemy(int distanceFromStart)
        {
            if (distanceFromStart < 25)
                return new Enemy(25, 8, 1, "Ptolemaic Scholar") { ItemColor = System.Drawing.Color.MediumPurple, Symbol = "s" };

            if (distanceFromStart < 40)
                return new Enemy(35, 14, 2, "Cultist of Hecate") { ItemColor = System.Drawing.Color.DarkOrchid, Symbol = "C" };

            return new Enemy(45, 18, 4, "Corrupted Oracle") { ItemColor = System.Drawing.Color.Purple, Symbol = "O" };
        }

        public IAttackable CreateBoss()
        {
            var boss = new Enemy(90, 22, 6, "Grand Archivist");
            boss.ItemColor = System.Drawing.Color.DarkMagenta;
            boss.Symbol = "Ѱ";
            return boss;
        }
    }
}