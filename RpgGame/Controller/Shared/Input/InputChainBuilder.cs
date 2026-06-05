using RpgGame.Controller.Shared.Commands;
using RpgGame.Model.Local.KeyBindings;

namespace RpgGame.Controller.Shared.Input;

public sealed class InputChainBuilder
{
    private InputHandler? _head;
    private InputHandler? _current;
    private readonly KeyBindingProfile _keyBindings;

    public InputChainBuilder(KeyBindingProfile keyBindings)
    {
        _keyBindings = keyBindings;
    }

    public InputChainBuilder AddHandler(InputHandler handler)
    {
        if (_head is null)
        {
            _head = handler;
            _current = handler;
        }
        else
        {
            _current = _current!.SetNext(handler);
        }

        return this;
    }

    public InputChainBuilder AddKey(
        Func<KeyBindingProfile, ConsoleKey> keySelector,
        Func<IPlayerCommand?> commandFactory)
    {
        return AddHandler(new KeyCommandHandler(_keyBindings, keySelector, commandFactory));
    }

    public InputHandler Build()
    {
        return _head ?? new NullInputHandler();
    }
}
