using System;
using RpgGame.Engine.Commands;

namespace RpgGame.Engine.Input
{
    public class KeyCommandHandler : InputHandler
    {
        private readonly string _state;
        private readonly string _action;
        private readonly ICommand _command;

        public KeyCommandHandler(string state, string action, ICommand command)
        {
            _state = state;
            _action = action;
            _command = command;
        }

        public override bool Handle(ConsoleKey key, out string errorMessage)
        {
            ConsoleKey currentKey = KeybindManager.Instance.GetKey(_state, _action);
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