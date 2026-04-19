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
        public InstructionState(GameEngine engine, List<string> instructions)
        {
            _engine = engine;
            _instructions = instructions;
            string s = "Instruction";
            _inputChainHead = new InputChainBuilder().AddKey(s, "Continue", new PopStateCommand(engine), engine.Keybinds)
                                                      .AddKey(s, "Reload", new ReloadConfigCommand(engine.Keybinds), engine.Keybinds)
                                                      .Build();
        }
        public void Draw(ConsoleRenderer renderer)
        {
            renderer.DrawInstructionScreen(_instructions);
        }

        public void HandleInput(ConsoleKey key)
        {
            _inputChainHead.Handle(key);
        }
    }
}
