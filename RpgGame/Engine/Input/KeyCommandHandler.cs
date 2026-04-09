using System;
using RpgGame.Engine.Commands;

namespace RpgGame.Engine.Input
{
    public class KeyCommandHandler : InputHandler
    {
        private readonly string _state;
        private readonly string _action;
        private readonly ICommand _command;
        private readonly KeybindManager _keybinds;

        public KeyCommandHandler(string state, string action, ICommand command, KeybindManager keybinds)
        {
            _state = state;
            _action = action;
            _command = command;
            _keybinds = keybinds;
        }

        public override bool Handle(ConsoleKey key, out string errorMessage)
        {
            ConsoleKey currentKey = _keybinds.GetKey(_state, _action);
            if (key == currentKey && currentKey != ConsoleKey.None)
            {
                _command.Execute();
                errorMessage = string.Empty;
                return true;
            }
            return base.Handle(key, out errorMessage);
        }
    }
}