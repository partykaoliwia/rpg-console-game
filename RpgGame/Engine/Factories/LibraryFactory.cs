// FILE: LibraryFactory.cs
using System;
using RpgGame.Characters;
using RpgGame.Engine.Builders;
using RpgGame.Engine.Observers;
using RpgGame.Items;
using RpgGame.Items.Modifiers;
using RpgGame.Items.Money;
using RpgGame.Items.Unusables;
using RpgGame.Items.Weapons;

namespace RpgGame.Engine.Factories
{
    public class LibraryFactory : IFactory
    {
        private readonly Random _random = new Random();
        private readonly SpeciesPublisher _harpies = new SpeciesPublisher("Harpies");
        private readonly SpeciesPublisher _giants = new SpeciesPublisher("Giants");
        private readonly SoundPublisher _sound;
        public LibraryFactory(SoundPublisher sound)
        {
            _sound = sound;
        }
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
            {
                return new Harpy(30, 20, 2, "h", System.Drawing.Color.MediumPurple, _harpies, _sound);
            }
            if (distanceFromStart < 40)
            {
                return new Giant(40, 13, 4, "G", System.Drawing.Color.DarkOrchid, _giants, _sound);
            }

            return new Giant(50, 18, 8, "G", System.Drawing.Color.Purple, _giants, _sound);
        }

        public IAttackable CreateBoss()
        {
            return new Harpy(80, 15, 5, "M", System.Drawing.Color.DarkGreen, _harpies, _sound) { Name = "Medusa" };
        }
    }
}