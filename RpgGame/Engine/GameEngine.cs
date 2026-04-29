using System;
using System.Collections.Generic;
using System.Threading;
using RpgGame.Characters;
using RpgGame.Engine.Input;
using RpgGame.Engine.Observers;
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
        public SoundPublisher SoundSystem { get; }
        public GameEngine(Board board, Player player, KeybindManager keybinds, string logFilePath)
        {
            _renderer = new ConsoleRenderer(keybinds);
            Keybinds = keybinds;
            LogFilePath = logFilePath;
            SoundSystem = new SoundPublisher();
            _stateStack.Push(new PlayingState(this, board, player));
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
                if(Console.KeyAvailable)
                { 
                    var keyInfo = Console.ReadKey(true);
                    while (Console.KeyAvailable) Console.ReadKey(true);
                    currentState.HandleInput(keyInfo.Key);
                }
                currentState.Update();
                currentState.Draw(_renderer);
                _renderer.RenderToConsole();
                Thread.Sleep(30);
            }
        }
    }
}