namespace RpgGame.Model.Local.InputModes;

public sealed class LogHistoryInputMode : IInputMode
{
    public bool ShowsInstructions => false;
    public bool ShowsInventory => false;
    public bool ShowsSlots => false;
    public bool ShowsLoot => false;
    public bool ShowsLogHistory => true;
    public bool ShowsGameOver => false;

    public void Handle(ConsoleKey key, IInputModeHandler handler)
    {
        handler.HandleLogHistoryKey(key);
    }

    public void Render(IInputModeRenderer renderer)
    {
        renderer.RenderLogHistoryMode();
    }
}
