using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RpgGame.Engine.Commands;
using RpgGame.Engine.Input;
using RpgGame.Engine.Logging;

namespace RpgGame.Engine.States
{
    public class LogHistoryState : IGameState
    {
        private readonly GameEngine _engine;
        private readonly InputHandler _inputChainHead;
        public void Update() { }
        public LogHistoryState(GameEngine engine)
        {
            _engine = engine;
            string s = "LogHistory";
            _inputChainHead = new InputChainBuilder().AddKey(s, "CloseLogs", new PopStateCommand(engine), engine.Keybinds)
                                                      .AddKey(s, "Reload", new ReloadConfigCommand(engine.Keybinds), engine.Keybinds)
                                                      .Build();
        }
        public void Draw(ConsoleRenderer renderer)
        {
            var allLogs=Logger.Instance.GetAllLogs();
            renderer.DrawLogHistoryScreen(allLogs);
        }

        public void HandleInput(ConsoleKey key)
        {
            _inputChainHead.Handle(key);
        }
    }
}
