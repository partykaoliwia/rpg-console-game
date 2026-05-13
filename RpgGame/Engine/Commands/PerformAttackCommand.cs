using System;
using RpgGame.Engine.States;
using RpgGame.Items;
using RpgGame.Combat;
using RpgGame.Engine.Input;
using RpgGame.Engine.Logging;

namespace RpgGame.Engine.Commands
{
    public class PerformAttackCommand : ICommand
    {
        private readonly CombatState _state;
        private readonly bool _useLeftHand; // true = lewa, false = prawa

        public PerformAttackCommand(CombatState state, bool useLeftHand)
        {
            _state = state;
            _useLeftHand = useLeftHand;
        }

        public void Execute()
        {
            var player = _state.Player;
            var target = _state.Target;
            var attackVisitor = _state.PendingAttack;

            IItem weapon = _useLeftHand ? player.LeftHand : player.RightHand;

            CombatStats stats;
            string attackName;

            if (weapon != null)
            {
                stats = weapon.Accept(attackVisitor, player);
                attackName = weapon.Name;
            }
            else
            {
                stats = attackVisitor.Visit((IItem)null, player);
                attackName = "Bare hands";
            }

            // atakowanie wroga
            int dealtDamage = target.TakeDamage(stats.Damage);
            Logger.Instance.Log($"Player attacked with {attackName}, dealing {dealtDamage} damage to {target.Name}.");

            // wróg zginął
            if (!target.IsAlive)
            {
                string flee = _state.Engine.Keybinds.GetKeyName("Combat", "Flee");
                Logger.Instance.Log("Enemy defeated! Returning to exploration.");
                _state.IsChoosingWeapon = false;
                _state.TargetTile.Entity = null;
                _state.Engine.PopState();
                return;
            }

            int receivedDamage = Math.Max(0, target.Strength - stats.Defense);
            int actualReceivedDamage = player.TakeDamage(receivedDamage);

            Logger.Instance.Log($"{target.Name} counterattacks for {actualReceivedDamage} damage! Player has {player.Health} HP left.");

            if (!player.IsAlive)
            {
                Logger.Instance.Log("Player has been defeated! Game over.");
                _state.Engine.PushState(new GameOverState(_state.Engine.LogFilePath));
                return; 
            }

            _state.IsChoosingWeapon = false;
        }
    }
}