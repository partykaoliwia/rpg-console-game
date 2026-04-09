using RpgGame.Engine.Commands;

namespace RpgGame.Engine.Input
{
    public class InputChainBuilder
    {
        private InputHandler _head;
        private InputHandler _current;

        // Pozwala dodać gotowy handler
        public InputChainBuilder AddHandler(InputHandler handler)
        {
            if (_head == null)
            {
                _head = handler;
                _current = handler;
            }
            else
            {
                _current.SetNext(handler);
                _current = handler;
            }
            return this;
        }

        // Metoda pomocnicza - automatycznie tworzy KeyCommandHandler i dodaje do łańcucha
        public InputChainBuilder AddKey(string state, string action, ICommand command, KeybindManager keybinds)
        {
            return AddHandler(new KeyCommandHandler(state, action, command, keybinds));
        }

        // Zwraca gotowy, połączony łańcuch
        public InputHandler Build()
        {
            return _head;
        }
    }
}