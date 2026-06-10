namespace RpgGame.Model.Local.InputModes;

public sealed class SlotInputMode : IInputMode
{
    public bool ShowsInstructions => false;
    public bool ShowsInventory => true;
    public bool ShowsSlots => true;
    public bool ShowsLoot => false;
    public bool ShowsLogHistory => false;
    public bool ShowsGameOver => false;

    public void Handle(ConsoleKey key, IInputModeHandler handler)
    {
        handler.HandleSlotKey(key);
    }

    public void Render(IInputModeRenderer renderer)
    {
        renderer.RenderSlotMode();
    }
}
