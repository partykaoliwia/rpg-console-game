using System;
using RpgGame.Characters;
using RpgGame.Gameboard;
using RpgGame.Engine.States;

namespace RpgGame.Engine.Commands
{
    public class LootItemCommand : ICommand
    {
        private readonly Player _player;
        private readonly Board _board;
        private readonly IMenuState _menu;

        public LootItemCommand(Player player, Board board, IMenuState menu)
        {
            _player = player;
            _board = board;
            _menu = menu;
        }

        public void Execute()
        {
            var tile = _board.GetTile(_player.X, _player.Y);
            if (tile.Items.Count == 0) return;

            _player.CollectSpecific(_menu.CursorIndex, _board);

            // poprawa kursora
            if (_menu.CursorIndex >= tile.Items.Count && tile.Items.Count > 0)
            {
                _menu.CursorIndex = tile.Items.Count - 1;
            }
        }
    }
}