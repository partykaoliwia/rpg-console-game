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

            // 1. Pobieramy broń ze wskazanej ręki
            IItem weapon = _useLeftHand ? player.LeftHand : player.RightHand;

            CombatStats stats;
            string attackName;

            // 2. WZORZEC ODWIEDZAJĄCY I DEKORATOR w akcji
            if (weapon != null)
            {
                // Jeśli mamy broń, korzystamy z jej metody Accept
                stats = weapon.Accept(attackVisitor, player);
                attackName = weapon.Name;
            }
            else
            {
                // Gołe pięści! Bezpośrednio wołamy metodę dla "zwykłych przedmiotów"
                // Nasze metody Visit(IItem) i tak korzystają tylko ze statystyk gracza, więc null tu nic nie popsuje.
                stats = attackVisitor.Visit((IItem)null, player);
                attackName = "Bare hands";
            }

            // 3. Atakujemy wroga
            int dealtDamage = enemy.TakeDamage(stats.Damage);
            _state.BattleLog = $"You strike with: {attackName}! You dealt {dealtDamage} damage.";

            // 4. Sprawdzamy czy wróg zginął
            if (!enemy.IsAlive)
            {
                string flee = KeybindManager.Instance.GetKey("Combat", "Flee").ToString();
                _state.BattleLog += $" The enemy has been defeated! Press [{flee}] to return to the board.";
                _state.IsChoosingWeapon = false;
                _state.EnemyTile.Entity = null; // Wróg znika z planszy
                return; // Koniec tury, wróg nie oddaje ciosu
            }

            // 5. Wróg oddaje cios (tylko jeśli przeżył)
            int receivedDamage = Math.Max(0, enemy.Strength - stats.Defense);
            player.TakeDamage(receivedDamage);

            _state.BattleLog += $" | {enemy.Name} counterattacks for {receivedDamage}! You have {player.Health} HP.";

            // 6. Sprawdzamy czy gracz zginął
            if (player.Health <= 0)
            {
                // Zamiast tylko wpisywać do logu, nakładamy stan Game Over!
                // Wrzucamy go bezpośrednio na silnik gry:
                _state.Engine.PushState(new GameOverState());
                return; // Przerywamy komendę!
            }

            // Na koniec zawsze resetujemy flagę, by kolejna tura znów pytała o atak 1,2,3
            _state.IsChoosingWeapon = false;
        }
    }
}