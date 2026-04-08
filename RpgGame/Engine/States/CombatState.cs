using System;
using RpgGame.Characters;
using RpgGame.Combat;
using RpgGame.Engine.Commands;
using RpgGame.Engine.Input;
using RpgGame.Gameboard;

namespace RpgGame.Engine.States
{
    public class CombatState : IGameState
    {
        public GameEngine Engine { get; }

        // Zmienione na publiczną właściwość, żeby komendy mogły z niej korzystać!
        public Player Player { get; }
        public Enemy CurrentEnemy { get; }
        public ITile EnemyTile { get; }

        // Dwa osobne łańcuchy odpowiedzialności (zamiast jednego)
        private readonly InputHandler _attackChain;
        private readonly InputHandler _weaponChain;

        public bool IsChoosingWeapon { get; set; } = false;
        public IAttackVisitor PendingAttack { get; set; }


        public string BattleLog { get; set; } = "An enemy approaches! Choose your attack.";

        public CombatState(GameEngine engine, Player player, Enemy enemy, ITile enemyTile)
        {
            Engine = engine;
            Player = player;
            CurrentEnemy = enemy;
            EnemyTile = enemyTile;


            string s = "Combat";

            // --- FAZA 1: ŁAŃCUCH WYBORU ATAKU ---
            var normalAttackKey = new KeyCommandHandler(s, "NormalAttack", new ChooseAttackCommand(this, new NormalAttack()));
            var stealthAttackKey = new KeyCommandHandler(s, "StealthAttack", new ChooseAttackCommand(this, new StealthAttack()));
            var magicAttackKey = new KeyCommandHandler(s, "MagicAttack", new ChooseAttackCommand(this, new MagicAttack()));
            var fleeKey1 = new KeyCommandHandler(s, "Flee", new PopStateCommand(engine));

            normalAttackKey.SetNext(stealthAttackKey).SetNext(magicAttackKey).SetNext(fleeKey1);
            _attackChain = normalAttackKey;

            // --- FAZA 2: ŁAŃCUCH WYBORU BRONI (L/R) ---
            // Używamy boola: true = lewa ręka, false = prawa ręka
            var leftKey = new KeyCommandHandler(s, "ChooseLeft", new PerformAttackCommand(this, true));
            var rightKey = new KeyCommandHandler(s, "ChooseRight", new PerformAttackCommand(this, false));
            var cancelKey = new KeyCommandHandler(s, "Flee", new CancelWeaponChoiceCommand(this));

            leftKey.SetNext(rightKey).SetNext(cancelKey);
            _weaponChain = leftKey;
        }

        public void HandleInput(ConsoleKey key)
        {
            string error;

            if (IsChoosingWeapon)
            {
                // Faza 2: Wybór ręki L/R
                _weaponChain.Handle(key, out error);
            }
            else
            {
                // Faza 1: Wybór ataku 1/2/3
                _attackChain.Handle(key, out error);
            }

            // Opcjonalnie: logowanie błędów, np. gdy gracz wciśnie nieprzypisany klawisz
            if (!string.IsNullOrEmpty(error))
            {
                BattleLog = error;
            }
        }

        public void Draw(ConsoleRenderer renderer)
        {
            // Przekazujemy log i wroga do renderera. 
            // Warto przekazać też IsChoosingWeapon, żeby renderer wiedział, czy rysować instrukcję "Wybierz 1/2/3" czy "Wybierz L/R".
            renderer.DrawCombatScreen(Player, CurrentEnemy, BattleLog, IsChoosingWeapon);
        }
    }
}