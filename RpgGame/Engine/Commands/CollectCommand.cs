using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RpgGame.Characters;
using RpgGame.Engine.Logging;
using RpgGame.Engine.States;
using RpgGame.Gameboard;

namespace RpgGame.Engine.Commands
{
    public class CollectCommand : ICommand
    {
        private readonly GameEngine _engine;
        private readonly Player _player;
        private readonly Board _board;
        public CollectCommand(GameEngine engine, Player player, Board board)
        {
            _engine = engine;
            _player = player;
            _board = board;
        }
        public void Execute()
        {
            var tile = _board.GetTile(_player.X, _player.Y);

            if (tile.Items.Count == 1)
            {
                var item = tile.Items[0];
                if(item.PickUp(_player))
                {
                    tile.Items.RemoveAt(0);
                    Logger.Instance.Log($"Player picked up {item.Name}. Removed from tile ({_player.X}, {_player.Y}).");

                    if(item.SoundRange>4)
                    {
                        _engine.SoundSystem.NotifySubscribers(_player.X,_player.Y,item.SoundRange,_board);
                    }
                }
            }
            else if (tile.Items.Count > 1)
            {
                _engine.PushState(new LootingState(_engine, _player, _board));
            }
        }
    }
}
