using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RpgGame.Engine.Commands;
using RpgGame.Engine.Input;

namespace RpgGame.Engine.States
{
    public class InstructionState : IGameState
    {
        private readonly GameEngine _engine;
        private readonly List<string> _instructions;
        private readonly InputHandler _inputChainHead;
        private string _errorMessage = string.Empty;
        public InstructionState(GameEngine engine, List<string> instructions)
        {
            _engine = engine;
            _instructions = instructions;
            var enterKey = new KeyCommandHandler(ConsoleKey.Enter, new PopStateCommand(engine));
            _inputChainHead = enterKey;
        }
        public void Draw(ConsoleRenderer renderer)
        {
            renderer.DrawInstructionScreen(_instructions,_errorMessage);
        }

        public void HandleInput(ConsoleKey key)
        {
            _inputChainHead.Handle(key, out _errorMessage);
        }
    }
}
