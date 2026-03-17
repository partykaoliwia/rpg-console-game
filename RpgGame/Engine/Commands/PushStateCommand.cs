using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RpgGame.Engine.States;

namespace RpgGame.Engine.Commands
{
    public class PushStateCommand : ICommand
    {
        private readonly GameEngine _engine;
        private readonly IGameState _stateToPush;

        public PushStateCommand(GameEngine engine, IGameState stateToPush)
        {
            _engine = engine;
            _stateToPush = stateToPush;
        }

        public void Execute()
        {
            _engine.PushState(_stateToPush);
        }
    }
}
