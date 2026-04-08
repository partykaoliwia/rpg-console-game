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
            // Wciśnięcie jakiegokolwiek klawisza po prostu wyłącza program
            Environment.Exit(0);
        }

        public void Draw(ConsoleRenderer renderer)
        {
            // Wywołujemy nową metodę rysującą
            renderer.DrawGameOverScreen(!_isAnimationPlayed);
            _isAnimationPlayed=true;
        }
    }
}