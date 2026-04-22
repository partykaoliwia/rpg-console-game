using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpgGame.Engine.Builders
{
    public class LabyrinthGenStrategy : IGenerationStrategy
    {
        public void GenerateMap(IDungeonBuilder builder)
        {
            builder.BuildFilledDungeon();
            builder.AddCentralHall(9, 6);
            builder.AddRooms();
            builder.AddCorridors();
            builder.AddItems(10);
            builder.AddWeapons(12);
            builder.AddMoney(10);
            builder.AddEnemies(20);
        }
    }
}
