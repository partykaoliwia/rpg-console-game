using System;

namespace RpgGame.Engine.States
{
    public class GameOverState : IGameState
    {
        private bool _isAnimationPlayed = false;
        private readonly string _logFilePath;
        public GameOverState(string logFilePath)
        {
            _logFilePath = logFilePath;
        }

        public void HandleInput(ConsoleKey key)
        {
            Environment.Exit(0);
        }

        public void Draw(ConsoleRenderer renderer)
        {
            renderer.DrawGameOverScreen(!_isAnimationPlayed, _logFilePath);
            _isAnimationPlayed=true;
        }
    }
}