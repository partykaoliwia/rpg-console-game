using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RpgGame.Characters;
using RpgGame.Engine.Builders;
using RpgGame.Engine.Observers;
using RpgGame.Items;
using RpgGame.Items.Modifiers;
using RpgGame.Items.Money;
using RpgGame.Items.Unusables;

namespace RpgGame.Engine.Factories
{
    public class LabyrinthFactory : IFactory
    {
        public string ThemeName => "Daedalus' labyrinth";
        public string[] Title => new string[]
            {
                "  ██████╗  █████╗ ███████╗██████╗  █████╗ ██╗     ██╗   ██╗███████╗",
                "  ██╔══██╗██╔══██╗██╔════╝██╔══██╗██╔══██╗██║     ██║   ██║██╔════╝",
                "  ██║  ██║███████║█████╗  ██║  ██║███████║██║     ██║   ██║███████╗",
                "  ██║  ██║██╔══██║██╔══╝  ██║  ██║██╔══██║██║     ██║   ██║╚════██║",
                "  ██████╔╝██║  ██║███████╗██████╔╝██║  ██║███████╗╚██████╔╝███████║",
                "  ╚═════╝ ╚═╝  ╚═╝╚══════╝╚═════╝ ╚═╝  ╚═╝╚══════╝ ╚═════╝ ╚══════╝",
                "██╗      █████╗ ██████╗ ██╗   ██╗██████╗ ██╗ ███╗   ██╗████████╗██╗  ██╗",
                "██║     ██╔══██╗██╔══██╗╚██╗ ██╔╝██╔══██╗██║ ████╗  ██║╚══██╔══╝██║  ██║",
                "██║     ███████║██████╔╝ ╚████╔╝ ██████╔╝██║ ██╔██╗ ██║   ██║   ███████║",
                "██║     ██╔══██║██╔══██╗  ╚██╔╝  ██╔══██╗██║ ██║╚██╗██║   ██║   ██╔══██║",
                "███████╗██║  ██║██████╔╝   ██║   ██║  ██║██║ ██║ ╚████║   ██║   ██║  ██║",
                "╚══════╝╚═╝  ╚═╝╚═════╝    ╚═╝   ╚═╝  ╚═╝╚═╝ ╚═╝  ╚═══╝   ╚═╝   ╚═╝  ╚═╝"
            };
        public string IntroMessage => "You stand before the entrance to a dark labyrinth carved by Daedalus, full of hidden paths and mysteries.";
        public IGenerationStrategy GenerationStrategy => new LabyrinthGenStrategy();
        private readonly Random _random = new Random();
        private readonly SpeciesPublisher _harpies = new SpeciesPublisher("Harpies");
        private readonly SpeciesPublisher _giants = new SpeciesPublisher("Giants");
        private readonly SoundPublisher _sound;
        public LabyrinthFactory(SoundPublisher sound)
        {
            _sound = sound;
        }

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
            int damage = 6 + _random.Next(0, 8);
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
                return new Harpy(30, 20, 2, "h", System.Drawing.Color.MediumPurple, _harpies,_sound);

            if (distanceFromStart < 40)
                return new Giant(40, 13, 4, "G", System.Drawing.Color.DarkOrchid, _giants, _sound);

            return new Giant(50, 18, 8, "G", System.Drawing.Color.Purple, _giants, _sound);
        }

        public IAttackable CreateBoss()
        {
            return new Harpy(80, 15, 5, "M", System.Drawing.Color.SaddleBrown, _harpies, _sound) { Name = "Minotaur" };
        }
    }
}