namespace RpgGame.Model.Local.InputModes;

public interface IInputMode
{
    bool ShowsInstructions { get; }
    bool ShowsInventory { get; }
    bool ShowsSlots { get; }
    bool ShowsLoot { get; }
    bool ShowsLogHistory { get; }
    bool ShowsGameOver { get; }

    void Handle(ConsoleKey key, IInputModeHandler handler);
    void Render(IInputModeRenderer renderer);
}
