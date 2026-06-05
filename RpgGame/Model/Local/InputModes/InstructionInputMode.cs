namespace RpgGame.Model.Local.InputModes;

public sealed class InstructionInputMode : IInputMode
{
    public bool ShowsInstructions => true;
    public bool ShowsInventory => false;
    public bool ShowsLoot => false;
    public bool ShowsLogHistory => false;
    public bool ShowsGameOver => false;

    public void Handle(ConsoleKey key, IInputModeHandler handler)
    {
        handler.HandleInstructionKey(key);
    }

    public void Render(IInputModeRenderer renderer)
    {
        renderer.RenderInstructionMode();
    }
}
