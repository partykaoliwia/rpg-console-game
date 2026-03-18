using System;

namespace RpgGame.Engine.Input
{
    public abstract class InputHandler
    {
        private InputHandler _nextHandler;

        // Metoda do budowania łańcucha
        public InputHandler SetNext(InputHandler handler)
        {
            _nextHandler = handler;
            return handler; // Zwracamy handler, by móc łączyć wywołania (fluent interface)
        }

        // HandleRequest z wzorca
        public virtual bool Handle(ConsoleKey key, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (_nextHandler != null)
            {
                return _nextHandler.Handle(key, out errorMessage);
            }

            // Koniec łańcucha - nikt nie obsłużył klawisza
            errorMessage = $"Klawisz '{key}' nie ma przypisanej akcji.";
            return false;
        }
    }
}