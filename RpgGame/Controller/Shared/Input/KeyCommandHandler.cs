using RpgGame.Controller.Shared.Commands;
using RpgGame.Model.Local.KeyBindings;

namespace RpgGame.Controller.Shared.Input;

public sealed class KeyCommandHandler : InputHandler
{
    private readonly KeyBindingProfile _keyBindings;
    private readonly Func<KeyBindingProfile, ConsoleKey> _keySelector;
    private readonly Func<IPlayerCommand?> _commandFactory;

    public KeyCommandHandler(
        KeyBindingProfile keyBindings,
        Func<KeyBindingProfile, ConsoleKey> keySelector,
        Func<IPlayerCommand?> commandFactory)
    {
        _keyBindings = keyBindings;
        _keySelector = keySelector;
        _commandFactory = commandFactory;
    }

    protected override bool TryCreateCommand(ConsoleKey key, out IPlayerCommand? command)
    {
        command = null;

        if (key != _keySelector(_keyBindings))
        {
            return false;
        }

        command = _commandFactory();
        return true;
    }
}
