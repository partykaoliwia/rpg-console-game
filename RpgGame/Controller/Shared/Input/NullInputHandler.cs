using RpgGame.Controller.Shared.Commands;

namespace RpgGame.Controller.Shared.Input;

public sealed class NullInputHandler : InputHandler
{
    protected override bool TryCreateCommand(ConsoleKey key, out IPlayerCommand? command)
    {
        command = null;
        return false;
    }
}
