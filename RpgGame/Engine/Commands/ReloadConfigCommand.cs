using RpgGame.Engine.Input;

namespace RpgGame.Engine.Commands
{
    public class ReloadConfigCommand : ICommand
    {
        public void Execute()
        {
            KeybindManager.Instance.LoadKeybinds("keybinds.txt");
        }
    }
}