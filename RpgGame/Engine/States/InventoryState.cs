using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using RpgGame.Characters;
using RpgGame.Engine.Commands;
using RpgGame.Engine.Input;
using RpgGame.Gameboard;
using RpgGame.Items;

namespace RpgGame.Engine.States
{
    public class InventoryState : IGameState, IMenuState
    {
        private readonly GameEngine _engine;
        private readonly Player _player;
        private readonly Board _board;
        private readonly InputHandler _inputChainHead;
        public int CursorIndex { get; set; } = 0;
        public int ItemCount => _player.Inventory.Count;
        private ITile CurrentTile => _board.GetTile(_player.X, _player.Y);

        public InventoryState(GameEngine engine, Player player, Board board)
        {
            _engine = engine;
            _player = player;
            _board = board;
            var s = "Inventory";

            _inputChainHead = new InputChainBuilder().AddKey(s, "MoveUp", new MoveCursorCommand(this, -1), engine.Keybinds)
                                                      .AddKey(s, "MoveDown", new MoveCursorCommand(this, 1), engine.Keybinds)
                                                      .AddKey(s, "EquipLeft", new EquipCommand(player, this, HandSlot.Left), engine.Keybinds)
                                                      .AddKey(s,"EquipRight", new EquipCommand(player, this, HandSlot.Right), engine.Keybinds)
                                                      .AddKey(s,"UnequipLeft", new UnequipCommand(player, HandSlot.Left), engine.Keybinds)
                                                      .AddKey(s,"UnequipRight", new UnequipCommand(player, HandSlot.Right), engine.Keybinds)
                                                      .AddKey(s,"Drop", new DropItemCommand(player, board, this), engine.Keybinds)
                                                      .AddKey(s,"Quit",new PopStateCommand(engine), engine.Keybinds)
                                                      .AddKey(s,"Reload", new ReloadConfigCommand(engine.Keybinds), engine.Keybinds)
                                                      .AddKey(s, "LogHistory", new PushStateCommand(engine, new LogHistoryState(engine)), engine.Keybinds)
                                                      .Build();
        }
        public void HandleInput(ConsoleKey key)
        {
            _inputChainHead.Handle(key);

            if (ItemCount == 0)
            {
                _engine.PopState();
            }

        }
        public void Draw(ConsoleRenderer renderer)
        {
            renderer.DrawBoard(_board, _player);
            renderer.DrawInventoryMenu(_player, CursorIndex,CurrentTile);
        }
    }
}