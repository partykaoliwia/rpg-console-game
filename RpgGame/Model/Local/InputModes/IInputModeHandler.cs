namespace RpgGame.Model.Local.InputModes;

public interface IInputModeHandler
{
    void HandleInstructionKey(ConsoleKey key);
    void HandlePlayingKey(ConsoleKey key);
    void HandleInventoryKey(ConsoleKey key);
    void HandleSlotKey(ConsoleKey key);
    void HandleLootKey(ConsoleKey key);
    void HandleCombatKey(ConsoleKey key);
    void HandleWeaponKey(ConsoleKey key);
    void HandleLogHistoryKey(ConsoleKey key);
    void HandleGameOverKey(ConsoleKey key);
}
