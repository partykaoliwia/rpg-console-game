using RpgGame.Engine.States;

namespace RpgGame.Engine.Commands
{
    public class CancelWeaponChoiceCommand : ICommand
    {
        private readonly CombatState _state;

        public CancelWeaponChoiceCommand(CombatState state)
        {
            _state = state;
        }

        public void Execute()
        {
            // Po prostu zdejmujemy flagę, żeby łańcuch znowu nasłuchiwał 1,2,3
            _state.IsChoosingWeapon = false;
            _state.BattleLog = "Anulowano wybór broni. Wybierz atak ponownie (1, 2, 3).";
        }
    }
}