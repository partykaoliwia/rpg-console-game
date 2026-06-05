namespace RpgGame.Model.Local.InputModes;

public interface IInputModeRenderer
{
    void RenderInstructionMode();
    void RenderPlayingMode();
    void RenderInventoryMode();
    void RenderLootMode();
    void RenderCombatMode();
    void RenderWeaponMode();
    void RenderLogHistoryMode();
    void RenderGameOverMode();
}
