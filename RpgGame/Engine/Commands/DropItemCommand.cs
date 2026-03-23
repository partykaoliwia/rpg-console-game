// Plik: DropItemCommand.cs
using RpgGame.Characters;
using RpgGame.Gameboard;
using RpgGame.Engine.States;

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
                _player.DropItem(_menu.CursorIndex, _board);

                // poprawa kursora, jeśli usunęliśmy ostatni element
                if (_menu.CursorIndex >= _player.Inventory.Count && _player.Inventory.Count > 0)
                {
                    _menu.CursorIndex = _player.Inventory.Count - 1;
                }
            }
        }
    }
}