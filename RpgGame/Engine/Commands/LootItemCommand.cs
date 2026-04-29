using System;
using RpgGame.Characters;
using RpgGame.Engine.Logging;
using RpgGame.Engine.Observers;
using RpgGame.Engine.States;
using RpgGame.Gameboard;

namespace RpgGame.Engine.Commands
{
    public class LootItemCommand : ICommand
    {
        private readonly Player _player;
        private readonly Board _board;
        private readonly IMenuState _menu;
        private readonly SoundPublisher _sound;


        public LootItemCommand(Player player, Board board, IMenuState menu, GameEngine engine, SoundPublisher sound)
        {
            _player = player;
            _board = board;
            _menu = menu;
            _sound = sound;
        }

        public void Execute()
        {
            var tile = _board.GetTile(_player.X, _player.Y);
            if (tile.Items.Count == 0) return;

            var item = tile.Items[_menu.CursorIndex];
            if(item.PickUp(_player))
            {
                tile.Items.RemoveAt(_menu.CursorIndex);
                Logger.Instance.Log($"Player picked up {item.Name}. Removed from tile ({_player.X}, {_player.Y}).");
                if (item.SoundRange > 0)
                {
                    _sound.NotifySubscribers(_player.X, _player.Y, item.SoundRange,_board);
                }
            }

            // poprawa kursora
            if (_menu.CursorIndex >= tile.Items.Count && tile.Items.Count > 0)
            {
                _menu.CursorIndex = tile.Items.Count - 1;
            }
        }
    }
}