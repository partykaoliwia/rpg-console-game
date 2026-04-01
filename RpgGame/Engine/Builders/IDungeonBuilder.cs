using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpgGame.Engine.Builders
{
    public interface IDungeonBuilder
    {
        void BuildEmptyDungeon();
        void BuildFilledDungeon();
        void AddCorridors();
        void AddRooms();
        void AddCentralHall(int width, int height);
        void AddItems(int count);
        void AddWeapons(int count);
        void AddMoney(int count);
        void AddEnemies(int count);
    }
}
