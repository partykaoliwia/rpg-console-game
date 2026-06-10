namespace RpgGame.Model.Local.InputModes;

public sealed class LootInputMode : IInputMode
{
    public bool ShowsInstructions => false;
    public bool ShowsInventory => false;
    public bool ShowsSlots => false;
    public bool ShowsLoot => true;
    public bool ShowsLogHistory => false;
    public bool ShowsGameOver => false;

    public void Handle(ConsoleKey key, IInputModeHandler handler)
    {
        handler.HandleLootKey(key);
    }

    public void Render(IInputModeRenderer renderer)
    {
        renderer.RenderLootMode();
    }
}
