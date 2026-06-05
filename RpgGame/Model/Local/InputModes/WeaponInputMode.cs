namespace RpgGame.Model.Local.InputModes;

public sealed class WeaponInputMode : IInputMode
{
    public bool ShowsInstructions => false;
    public bool ShowsInventory => false;
    public bool ShowsLoot => false;
    public bool ShowsLogHistory => false;
    public bool ShowsGameOver => false;

    public void Handle(ConsoleKey key, IInputModeHandler handler)
    {
        handler.HandleWeaponKey(key);
    }

    public void Render(IInputModeRenderer renderer)
    {
        renderer.RenderWeaponMode();
    }
}
