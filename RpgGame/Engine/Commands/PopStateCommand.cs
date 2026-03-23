using System;
using RpgGame.Engine.States;

namespace RpgGame.Engine.Commands
{
    public class PopStateCommand : ICommand
    {
        private readonly GameEngine _engine;

        public PopStateCommand(GameEngine engine)
        {
            _engine = engine ?? throw new ArgumentNullException(nameof(engine));
        }

        public void Execute()
        {
            _engine.PopState();
        }
    }
}