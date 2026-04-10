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

        public Player Player { get; }
        public IAttackable Target { get; }
        public ITile TargetTile { get; }

        private readonly InputHandler _attackChain;
        private readonly InputHandler _weaponChain;

        public bool IsChoosingWeapon { get; set; } = false;
        public IAttackVisitor PendingAttack { get; set; }


        public string BattleLog { get; set; } = "An enemy approaches! Choose your attack.";

        public CombatState(GameEngine engine, Player player, IAttackable target, ITile targetTile)
        {
            Engine = engine;
            Player = player;
            Target = target;
            TargetTile = targetTile;


            string s = "Combat";

            // ŁAŃCUCH WYBORU ATAKU (1-3)
            _attackChain = new InputChainBuilder().AddKey(s,"NormalAttack", new ChooseAttackCommand(this, new NormalAttack()), engine.Keybinds)
                                              .AddKey(s, "StealthAttack", new ChooseAttackCommand(this, new StealthAttack()), engine.Keybinds)
                                              .AddKey(s, "MagicAttack", new ChooseAttackCommand(this, new MagicAttack()), engine.Keybinds)
                                              .AddKey(s, "Flee", new PopStateCommand(engine), engine.Keybinds)
                                              .Build();

            // ŁAŃCUCH WYBORU BRONI (L/R)
            _weaponChain = new InputChainBuilder().AddKey(s, "ChooseLeft", new PerformAttackCommand(this, true), engine.Keybinds)
                                                 .AddKey(s, "ChooseRight", new PerformAttackCommand(this, false), engine.Keybinds)
                                                 .AddKey(s, "Flee", new CancelWeaponChoiceCommand(this), engine.Keybinds)
                                                 .Build();
        }

        public void HandleInput(ConsoleKey key)
        {
            string error;

            if (IsChoosingWeapon)
            {
                _weaponChain.Handle(key, out error);
            }
            else
            {
                _attackChain.Handle(key, out error);
            }

            if (!string.IsNullOrEmpty(error))
            {
                BattleLog = error;
            }
        }

        public void Draw(ConsoleRenderer renderer)
        {
            renderer.DrawCombatScreen(Player, Target, BattleLog, IsChoosingWeapon);
        }
    }
}