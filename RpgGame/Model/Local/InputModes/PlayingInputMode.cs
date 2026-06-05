namespace RpgGame.Model.Local.InputModes;

public sealed class PlayingInputMode : IInputMode
{
    public bool ShowsInstructions => false;
    public bool ShowsInventory => false;
    public bool ShowsLoot => false;
    public bool ShowsLogHistory => false;
    public bool ShowsGameOver => false;

    public void Handle(ConsoleKey key, IInputModeHandler handler)
    {
        handler.HandlePlayingKey(key);
    }

    public void Render(IInputModeRenderer renderer)
    {
        renderer.RenderPlayingMode();
    }
}
