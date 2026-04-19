using RpgGame.Engine.Logging;
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
            _state.IsChoosingWeapon = false;
            Logger.Instance.Log("Weapon choice cancelled.");
        }
    }
}