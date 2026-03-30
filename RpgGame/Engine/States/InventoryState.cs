using System;
using System.Collections.Generic;
using System.Linq;
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
        private string _errorMessage = string.Empty;
        public int CursorIndex { get; set; } = 0;
        public int ItemCount => _player.Inventory.Count;

        public InventoryState(GameEngine engine, Player player, Board board)
        {
            _engine = engine;
            _player = player;
            _board = board;
            var s = "Inventory";

            // Góra dół
            var upKey = new KeyCommandHandler(s,"MoveUp", new MoveCursorCommand(this, -1));
            var downKey = new KeyCommandHandler(s, "MoveDown", new MoveCursorCommand(this, 1));

            // Zakładanie
            var equipLeftKey = new KeyCommandHandler(s, "EquipLeft", new EquipCommand(player, this, HandSlot.Left));
            var equipRightKey = new KeyCommandHandler(s,"EquipRight", new EquipCommand(player, this, HandSlot.Right));

            // Zdejmowanie
            var unequipRightKey = new KeyCommandHandler(s,"UnequipRight", new UnequipCommand(player, HandSlot.Left));
            var unequipLeftKey = new KeyCommandHandler(s,"UnequipLeft", new UnequipCommand(player, HandSlot.Right));

            // Wyrzucanie
            var dropKey = new KeyCommandHandler(s,"Drop", new DropItemCommand(player, board, this));

            // Zamykanie
            var quitKey = new KeyCommandHandler(s,"Quit",new PopStateCommand(engine));

            // Budowanie łańcucha
            upKey.SetNext(downKey).SetNext(equipLeftKey).SetNext(equipRightKey).SetNext(unequipLeftKey).SetNext(unequipRightKey).SetNext(dropKey).SetNext(quitKey);

            _inputChainHead = upKey;
        }
        public void HandleInput(ConsoleKey key)
        {
            _inputChainHead.Handle(key, out _errorMessage);

            if (ItemCount == 0)
            {
                _engine.PopState();
            }
        }
        public void Draw(ConsoleRenderer renderer)
        {
            renderer.DrawBoard(_board, _player);
            renderer.DrawInventoryMenu(_player, CursorIndex, _errorMessage);
        }
    }
}