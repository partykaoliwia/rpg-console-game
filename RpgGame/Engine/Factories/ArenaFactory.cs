// FILE: ArenaFactory.cs
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
    public class ArenaFactory : IFactory
    {
        private readonly Random _random = new Random();
        private readonly SpeciesPublisher _harpies = new SpeciesPublisher("Harpies");
        private readonly SpeciesPublisher _giants = new SpeciesPublisher("Giants");
        private readonly SoundPublisher _sound;
        public ArenaFactory(SoundPublisher sound)
        {
            _sound = sound;
        }
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
            {
                // Słabsi wrogowie jako zwinne Harpie
                return new Harpy(30, 12, 2, "h", System.Drawing.Color.IndianRed, _harpies, _sound);
            }
            if (distanceFromStart < 40)
            {
                // Średni wrogowie jako Giganci
                return new Giant(45, 18, 5, "G", System.Drawing.Color.Firebrick, _giants, _sound);
            }

            // Najsilniejsi zwykli wrogowie
            return new Giant(60, 25, 8, "A", System.Drawing.Color.DarkRed, _giants, _sound);
        }

        public IAttackable CreateBoss()
        {
            return new Giant(80, 15, 5, "Δ", System.Drawing.Color.Crimson, _giants, _sound) { Name = "Deimos" };
        }
    }
}