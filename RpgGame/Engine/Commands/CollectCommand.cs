using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RpgGame.Characters;
using RpgGame.Engine.States;
using RpgGame.Gameboard;

namespace RpgGame.Engine.Commands
{
    public class CollectCommand : ICommand
    {
        private readonly GameEngine _engine;
        private readonly Player _player;
        private readonly Board _board;
        private ITile CurrentTile => _board.GetTile(_player.X, _player.Y);
        public CollectCommand(GameEngine engine, Player player, Board board)
        {
            _engine = engine;
            _player = player;
            _board = board;
        }
        public void Execute()
        {
            if (CurrentTile.Items.Count == 1)
            {
                // jeśli jest tylko jeden przedmiot, podnosimy go bez otwierania menu
                _player.CollectSpecific(0, _board);
            }
            else if (CurrentTile.Items.Count > 1)
            {
                // jeśli jest więcej, odpalamy Looting menu
                _engine.PushState(new LootingState(_engine, _player, _board));
            }
        }
    }
}
