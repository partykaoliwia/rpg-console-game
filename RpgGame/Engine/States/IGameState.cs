using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpgGame.Engine.States
{
    public interface IGameState
    {
        void HandleInput(ConsoleKey key);
        void Draw(ConsoleRenderer renderer);
    }
}
