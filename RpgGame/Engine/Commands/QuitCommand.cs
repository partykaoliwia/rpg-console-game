using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            _engine.QuitGame();
        }
    }
}
