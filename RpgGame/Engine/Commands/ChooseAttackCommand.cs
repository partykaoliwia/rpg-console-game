using System;
using RpgGame.Engine.States;
using RpgGame.Combat;

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
                _state.BattleLog = "Masz broń w obu dłoniach! Którą uderzyć? [L] - Lewa, [R] - Prawa (lub ESC by anulować)";
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