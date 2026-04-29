using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RpgGame.Engine.Factories;
using RpgGame.Engine.Input;

namespace RpgGame.Engine.Builders
{
    public class InstructionBuilder : IDungeonBuilder
    {
        private List<string> _instructions = new List<string>();

        public List<string> BuildResult() => _instructions;
        private readonly KeybindManager _keybinds;
        private readonly IFactory _factory;
        public InstructionBuilder(KeybindManager keybinds, IFactory factory)
        {
            _keybinds = keybinds;
            _factory = factory;
        }

        public void BuildEmptyDungeon()
        {
            _instructions.Add(_factory.IntroMessage);
        }

        public void BuildFilledDungeon()
        {
            _instructions.Add(_factory.IntroMessage);
        }

        public void AddCorridors()
        {
            _instructions.Add("Before you stretch countless winding and narrow corridors, where it is easy to get lost.");
        }

        public void AddRooms()
        {
            _instructions.Add("You have stumbled upon several hidden chambers carved into the rock, which may conceal treasures or danger.");
        }

        public void AddCentralHall(int width, int height)
        {
            _instructions.Add("In the very heart of this cursed place lurks a mighty Chamber. That is where the greatest danger awaits.");
        }

        public void AddItems(int count)
        {
            string collect = _keybinds.GetKeyName("Playing", "Collect");
            _instructions.Add($"The gods have left here {count} ancient artifacts, which you can pick up by pressing [{collect}].");
        }

        public void AddWeapons(int count)
        {
            string collect = _keybinds.GetKeyName("Playing", "Collect");
            string inv = _keybinds.GetKeyName("Playing", "OpenInventory");
            _instructions.Add($"Previous adventurers have left {count} pieces of powerful weapons here - use [{collect}] to pick up and [{inv}] to manage your equipment.");
        }

        public void AddMoney(int count)
        {
            _instructions.Add($"There are {count} coins scattered here, collect them to strengthen your position in the world of mortals.");
        }

        public void AddEnemies(int count)
        {
            if (count == 0)
            {
                _instructions.Add("A powerful Boss lurks in the shadows, threatening your life.");
                return;
            }
            _instructions.Add($"Beware! {count} enemies and a powerful Boss lurk in the shadows, threatening your life.");
        }
    }
}