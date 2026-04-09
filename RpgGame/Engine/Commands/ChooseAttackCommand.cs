using System;
using RpgGame.Combat;
using RpgGame.Engine.Input;
using RpgGame.Engine.States;

namespace RpgGame.Engine.Commands
{
    public class ChooseAttackCommand : ICommand
    {
        private readonly CombatState _state;
        private readonly IAttackVisitor _attack;

        public ChooseAttackCommand(CombatState state, IAttackVisitor attack)
        {
            _state = state;
            _attack = attack;
        }

        public void Execute()
        {
            if (!_state.CurrentEnemy.IsAlive || _state.Player.Health <= 0) return;

            var p = _state.Player;
            _state.PendingAttack = _attack;

            bool hasLeft = p.LeftHand != null;
            bool hasRight = p.RightHand != null;
            bool areWeaponsDifferent = p.LeftHand != p.RightHand;

            if (hasLeft && hasRight && areWeaponsDifferent)
            {
                _state.IsChoosingWeapon = true;

                string left = KeybindManager.Instance.GetKey("Combat", "ChooseLeft").ToString();
                string right = KeybindManager.Instance.GetKey("Combat", "ChooseRight").ToString();
                string flee = KeybindManager.Instance.GetKey("Combat", "Flee").ToString();
                _state.BattleLog = $"You have a weapon in both hands! Which one to strike? [{left}] - Left, [{right}] - Right (or [{flee}] to cancel)";
            }
            else
            {
                bool useLeft = !hasRight && hasLeft;
                new PerformAttackCommand(_state, useLeft).Execute();
            }
        }
    }
}