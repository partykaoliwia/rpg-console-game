using System;

namespace RpgGame.Engine.Builders
{
    public class DungeonDirector
    {
        private IDungeonBuilder _builder;

        public void SetBuilder(IDungeonBuilder builder)
        {
            _builder = builder;
        }

        // default sposob na budowanie mojej planszy
        public void ConstructMinotaursLabyrinth()
        {
            _builder.BuildFilledDungeon();
            _builder.AddCentralHall(9, 6);
            _builder.AddRooms();
            _builder.AddCorridors();
            _builder.AddItems(10);
            _builder.AddWeapons(6);
        }

        // pozostale sposoby
        public void ConstructTestItemkow()
        {
            _builder.BuildEmptyDungeon();
            _builder.AddItems(20);
            _builder.AddWeapons(10);
        }

    }
}