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


            // Przesuwanie kursora
            var wKey = new KeyCommandHandler(ConsoleKey.W, new MoveCursorCommand(this, -1));
            var sKey = new KeyCommandHandler(ConsoleKey.S, new MoveCursorCommand(this, 1));

            // Zakładanie
            var lKey = new KeyCommandHandler(ConsoleKey.L, new EquipCommand(player, this, HandSlot.Left));
            var rKey = new KeyCommandHandler(ConsoleKey.R, new EquipCommand(player, this, HandSlot.Right));

            // Zdejmowanie
            var d1Key = new KeyCommandHandler(ConsoleKey.D1, new UnequipCommand(player, HandSlot.Left));
            var d2Key = new KeyCommandHandler(ConsoleKey.D2, new UnequipCommand(player, HandSlot.Right));

            // Wyrzucanie
            var qKey = new KeyCommandHandler(ConsoleKey.Q, new DropItemCommand(player, board, this));

            // Zamykanie
            var escKey = new KeyCommandHandler(ConsoleKey.Escape, new PopStateCommand(engine));
            var iKey = new KeyCommandHandler(ConsoleKey.I, new PopStateCommand(engine));

            // Budowanie łańcucha
            wKey.SetNext(sKey).SetNext(lKey).SetNext(rKey)
                .SetNext(d1Key).SetNext(d2Key).SetNext(qKey).SetNext(escKey).SetNext(iKey);

            _inputChainHead = wKey;
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