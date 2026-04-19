using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RpgGame.Engine.Logging;

namespace RpgGame.Engine.Commands
{
    public class QuitCommand : ICommand
    {
        private readonly GameEngine _engine;
        public QuitCommand(GameEngine engine)
        {
            _engine = engine;
        }
        public void Execute()
        {
            Logger.Instance.Log("Player chose to quit the game.");
            _engine.QuitGame();
        }
    }
}
