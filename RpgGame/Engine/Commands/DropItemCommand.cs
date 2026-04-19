// Plik: DropItemCommand.cs
using RpgGame.Characters;
using RpgGame.Gameboard;
using RpgGame.Engine.States;
using RpgGame.Engine.Logging;

namespace RpgGame.Engine.Commands
{
    public class DropItemCommand : ICommand
    {
        private readonly Player _player;
        private readonly Board _board;
        private readonly IMenuState _menu;

        public DropItemCommand(Player player, Board board, IMenuState menu)
        {
            _player = player;
            _board = board;
            _menu = menu;
        }

        public void Execute()
        {
            if (_player.Inventory.Count > 0)
            {
                var droppedItem = _player.DropItem(_menu.CursorIndex);
                if (droppedItem != null)
                {
                    var currentTile = _board.GetTile(_player.X, _player.Y);
                    currentTile.Items.Add(droppedItem);
                    Logger.Instance.Log($"Player dropped {droppedItem.Name}. Added to tile ({_player.X}, {_player.Y}).");

                    // poprawa kursora
                    if (_menu.CursorIndex >= _player.Inventory.Count && _player.Inventory.Count > 0)
                    {
                        _menu.CursorIndex = _player.Inventory.Count - 1;
                    }
                }
            }
        }
    }
}