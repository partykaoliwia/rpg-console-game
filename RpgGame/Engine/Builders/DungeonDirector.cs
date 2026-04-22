using System;

namespace RpgGame.Engine.Builders
{
    public class DungeonDirector
    {
        private IDungeonBuilder _builder;
        private IGenerationStrategy _strategy;

        public void SetBuilder(IDungeonBuilder builder)
        {
            _builder = builder;
        }

        public void SetStrategy(IGenerationStrategy strategy)
        {
            _strategy = strategy;
        }
        public void ConstructDungeon()
        {
            _strategy.GenerateMap(_builder);
        }

    }
}