using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpgGame.Engine.Builders
{
    public class ArenaGenStrategy : IGenerationStrategy
    {
        public void GenerateMap(IDungeonBuilder builder)
        {
            builder.BuildFilledDungeon();
            builder.AddCorridors();
            builder.AddCentralHall(37, 17);
            builder.AddItems(5);
            builder.AddWeapons(15);
            builder.AddMoney(10);
            builder.AddEnemies(25);
        }
    }
}
