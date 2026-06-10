namespace RpgGame.Model.Local.InputModes;

public sealed class GameOverInputMode : IInputMode
{
    public bool ShowsInstructions => false;
    public bool ShowsInventory => false;
    public bool ShowsSlots => false;
    public bool ShowsLoot => false;
    public bool ShowsLogHistory => false;
    public bool ShowsGameOver => true;

    public void Handle(ConsoleKey key, IInputModeHandler handler)
    {
        handler.HandleGameOverKey(key);
    }

    public void Render(IInputModeRenderer renderer)
    {
        renderer.RenderGameOverMode();
    }
}
