using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RpgGame.Characters;
using RpgGame.Gameboard;
using RpgGame.Items;

namespace RpgGame.Engine.States
{
    public class LootingState : IGameState
    {
        private readonly GameEngine _engine;
        private readonly Player _player;
        private readonly Board _board;
        private readonly Dictionary<ConsoleKey, Action> _keyBindings;
        private int _cursorIndex = 0;
        private string _errorMessage = string.Empty;

        private ITile CurrentTile => _board.GetTile(_player.X, _player.Y);
        private List<IItem> CurrentTileItems => CurrentTile.Items;
        private int CurrentTileItemsCount => CurrentTileItems.Count;

        public LootingState(GameEngine engine, Player player, Board board)
        {
            _engine = engine;
            _player = player;
            _board = board;

            _keyBindings = new Dictionary<ConsoleKey, Action>
            {
                { ConsoleKey.W, () => MoveCursor(-1) },
                { ConsoleKey.S, () => MoveCursor(1) },
                { ConsoleKey.E, () => PickUpSelectedItem() },
                { ConsoleKey.Q, () => _engine.PopState() },
                { ConsoleKey.Escape, () => _engine.PopState() }
            };
        }
        private void MoveCursor(int direction)
        {
            if (CurrentTileItemsCount == 0) return;
            _cursorIndex += direction;
            if (_cursorIndex < 0) _cursorIndex = CurrentTileItemsCount - 1;
            if (_cursorIndex >= CurrentTileItemsCount) _cursorIndex = 0;
        }
        private void PickUpSelectedItem()
        {
            _player.CollectSpecific(_cursorIndex, _board);
            ValidateState();
        }
        private void ValidateState()
        {
            if(CurrentTileItemsCount==0)
            {
                _engine.PopState();
            }
            else if(_cursorIndex>= CurrentTileItemsCount)
            {
                _cursorIndex = CurrentTileItemsCount - 1;
            }
        }

        public void HandleInput(ConsoleKey key)
        {
            _errorMessage = string.Empty;
            if (_keyBindings.TryGetValue(key, out Action action)) action.Invoke();
            else _errorMessage = $"Klawisz '{key}' nie ma funkcji w menu podnoszenia.";
        }

        public void Draw(ConsoleRenderer renderer)
        {
            renderer.DrawBoard(_board, _player);
            renderer.DrawLootingMenu(_player, CurrentTile, _cursorIndex, _errorMessage);
        }
    }
}
