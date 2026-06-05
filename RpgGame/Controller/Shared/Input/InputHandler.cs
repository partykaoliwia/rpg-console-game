using RpgGame.Controller.Shared.Commands;

namespace RpgGame.Controller.Shared.Input;

public abstract class InputHandler
{
    private InputHandler? _nextHandler;

    public InputHandler SetNext(InputHandler handler)
    {
        _nextHandler = handler;
        return handler;
    }

    public IPlayerCommand? Handle(ConsoleKey key)
    {
        if (TryCreateCommand(key, out var command))
        {
            return command;
        }

        return _nextHandler?.Handle(key);
    }

    protected abstract bool TryCreateCommand(ConsoleKey key, out IPlayerCommand? command);
}
