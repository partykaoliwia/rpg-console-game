using System;

namespace RpgGame.Engine.States
{
    public class GameOverState : IGameState
    {
        private bool _isAnimationPlayed = false;
        public GameOverState()
        {
        }

        public void HandleInput(ConsoleKey key)
        {
            Environment.Exit(0);
        }

        public void Draw(ConsoleRenderer renderer)
        {
            renderer.DrawGameOverScreen(!_isAnimationPlayed);
            _isAnimationPlayed=true;
        }
    }
}