using RpgGame.Engine.Input;

namespace RpgGame.Engine.Commands
{
    public class ReloadConfigCommand : ICommand
{
    private readonly KeybindManager _keybinds;
    public ReloadConfigCommand(KeybindManager keybinds) { _keybinds = keybinds; }
    public void Execute() { _keybinds.LoadKeybinds("keybinds.txt"); }
}
}