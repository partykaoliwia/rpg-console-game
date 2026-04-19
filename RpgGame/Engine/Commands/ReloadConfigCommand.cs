using RpgGame.Engine.Input;
using RpgGame.Engine.Logging;

namespace RpgGame.Engine.Commands
{
    public class ReloadConfigCommand : ICommand
{
    private readonly KeybindManager _keybinds;
    public ReloadConfigCommand(KeybindManager keybinds) { _keybinds = keybinds; }
    public void Execute() 
        { 
            _keybinds.LoadKeybinds("keybinds.txt"); 
            Logger.Instance.Log("Keybinds reloaded from file.");
        }
}
}