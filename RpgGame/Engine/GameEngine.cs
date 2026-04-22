using System;
using System.Collections.Generic;
using System.Threading;
using RpgGame.Characters;
using RpgGame.Engine.Input;
using RpgGame.Engine.States;
using RpgGame.Gameboard;

namespace RpgGame.Engine
{
    public class GameEngine
    {
        private readonly Stack<IGameState> _stateStack = new();
        private readonly ConsoleRenderer _renderer;
        private bool _isRunning = true;
        public string LogFilePath { get; }
        public KeybindManager Keybinds { get; }
        public GameEngine(Board board, Player player, KeybindManager keybinds, string logFilePath)
        {
            _renderer = new ConsoleRenderer(keybinds);
            Keybinds = keybinds;
            _stateStack.Push(new PlayingState(this, board, player));
            LogFilePath = logFilePath;
        }
        public void PushState(IGameState state)
        {
            _stateStack.Push(state);
        }
        public void PopState()
        {
            if(_stateStack.Count > 1)
            {
                _stateStack.Pop();
            }
        }
        public void QuitGame() => _isRunning = false;
        public void RunGame()
        {
            Console.CursorVisible = false;
            while(_isRunning)
            {
                var currentState = _stateStack.Peek();
                currentState.Draw(_renderer);
                _renderer.RenderToConsole();
                if(Console.KeyAvailable)
                { 
                    var keyInfo = Console.ReadKey(true);
                    currentState.HandleInput(keyInfo.Key);
                }
            }
        }
    }
}