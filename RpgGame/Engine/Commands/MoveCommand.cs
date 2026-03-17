using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RpgGame.Characters;
using RpgGame.Gameboard;

namespace RpgGame.Engine.Commands
{
    public class MoveCommand : ICommand
    {
        private readonly Player _player;
        private readonly Board _board;
        private readonly int _dx;
        private readonly int _dy;

        public MoveCommand(Player player, Board board, int dx, int dy)
        {
            _player = player;
            _board = board;
            _dx = dx;
            _dy = dy;
        }

        public void Execute()
        {
            _player.Move(_dx, _dy, _board);
        }
    }
}
