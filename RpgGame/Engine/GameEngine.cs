using System;
using System.Collections.Generic;
using System.Threading;
using RpgGame.Characters;
using RpgGame.Engine.States;
using RpgGame.Gameboard;

namespace RpgGame.Engine
{
    public class GameEngine
    {
        private readonly Stack<IGameState> _stateStack = new();
        private readonly ConsoleRenderer _renderer;
        private bool _isRunning = true;
        public GameEngine(Board board, Player player)
        {
            _renderer = new ConsoleRenderer();
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
                currentState.Draw(_renderer);
                _renderer.RenderToConsole();
                if(Console.KeyAvailable)
                { 
                    var keyInfo = Console.ReadKey(true);
                    currentState.HandleInput(keyInfo.Key);
                }
                Thread.Sleep(5); // TO DO
            }
        }
    }
}