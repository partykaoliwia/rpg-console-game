using System;
using RpgGame.Engine.States;
using RpgGame.Items;
using RpgGame.Combat;
using RpgGame.Engine.Input;

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
            var enemy = _state.CurrentEnemy;
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
            int dealtDamage = enemy.TakeDamage(stats.Damage);
            _state.BattleLog = $"You strike with: {attackName}! You dealt {dealtDamage} damage.";

            // wróg zginął
            if (!enemy.IsAlive)
            {
                string flee = KeybindManager.Instance.GetKeyName("Combat", "Flee");
                _state.BattleLog += $" The enemy has been defeated! Press [{flee}] to return to the board.";
                _state.IsChoosingWeapon = false;
                _state.EnemyTile.Entity = null;
                return;
            }

            int receivedDamage = Math.Max(0, enemy.Strength - stats.Defense);
            player.TakeDamage(receivedDamage);

            _state.BattleLog += $" | {enemy.Name} counterattacks for {receivedDamage}! You have {player.Health} HP.";

            if (player.Health <= 0)
            {
                _state.Engine.PushState(new GameOverState());
                return; 
            }

            _state.IsChoosingWeapon = false;
        }
    }
}