using System;
using RpgGame.Engine.Commands;

namespace RpgGame.Engine.Input
{
    public class KeyCommandHandler : InputHandler
    {
        private readonly ConsoleKey _triggerKey;
        private readonly ICommand _command;

        public KeyCommandHandler(ConsoleKey triggerKey, ICommand command)
        {
            _triggerKey = triggerKey;
            _command = command;
        }

        public override bool Handle(ConsoleKey key, out string errorMessage)
        {
            // Pozbyliśmy się switcha! Sprawdzamy tylko ten jeden konkretny warunek.
            if (key == _triggerKey)
            {
                _command.Execute();
                errorMessage = string.Empty;
                return true; // Obsłużone, przerywamy łańcuch
            }

            // Podajemy dalej
            return base.Handle(key, out errorMessage);
        }
    }
}