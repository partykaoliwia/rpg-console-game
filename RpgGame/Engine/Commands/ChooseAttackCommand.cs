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
            // Zabezpieczenie: jeśli wróg już nie żyje, ignorujemy kolejne wciskanie ataków
            if (!_state.CurrentEnemy.IsAlive || _state.Player.Health <= 0) return;

            var p = _state.Player;
            _state.PendingAttack = _attack; // Zapamiętujemy, jaki rodzaj ataku wybrano

            bool hasLeft = p.LeftHand != null;
            bool hasRight = p.RightHand != null;
            bool areWeaponsDifferent = p.LeftHand != p.RightHand;

            // Sprawdzamy czy mamy dwie RÓŻNE bronie (wykluczamy broń dwuręczną)
            if (hasLeft && hasRight && areWeaponsDifferent)
            {
                // Przełączamy stan interfejsu na nasłuchiwanie L i R
                _state.IsChoosingWeapon = true;
                // change battlelog to english 
                string left = KeybindManager.Instance.GetKey("Combat", "ChooseLeft").ToString();
                string right = KeybindManager.Instance.GetKey("Combat", "ChooseRight").ToString();
                string flee = KeybindManager.Instance.GetKey("Combat", "Flee").ToString();

                _state.BattleLog = $"You have a weapon in both hands! Which one to strike? [{left}] - Left, [{right}] - Right (or [{flee}] to cancel)";
            }
            else
            {
                // Jeśli ma tylko w jednej ręce, lub w obu tę samą (dwuręczna), bijemy od razu.
                // Domyślnie bije prawą, chyba że tylko w lewej coś ma.
                bool useLeft = !hasRight && hasLeft;

                // Odpalamy komendę uderzenia "w locie"
                new PerformAttackCommand(_state, useLeft).Execute();
            }
        }
    }
}