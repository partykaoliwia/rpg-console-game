using System;
using RpgGame.Engine.Logging;

namespace RpgGame.Engine.Input
{
    public abstract class InputHandler
    {
        private InputHandler _nextHandler;

        // metoda do budowania łańcucha (chain of responsibility)
        public InputHandler SetNext(InputHandler handler)
        {
            _nextHandler = handler;
            return handler;
        }

        public virtual bool Handle(ConsoleKey key)
        {

            if (_nextHandler != null)
            {
                return _nextHandler.Handle(key);
            }

            // koniec łańcucha, nikt nie obsłużył klawisza
            Logger.Instance.Log($"Invalid key, no action assigned to [{key}].");
            return false;
        }
    }
}