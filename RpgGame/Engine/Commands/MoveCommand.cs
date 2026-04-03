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
    public class MoveCommand : ICommand
    {
        private readonly Player _player;
        private readonly Board _board;
        private readonly int _dx;
        private readonly int _dy;
        private readonly GameEngine _engine;

        public MoveCommand(GameEngine engine, Player player, Board board, int dx, int dy)
        {
            _engine = engine;
            _player = player;
            _board = board;
            _dx = dx;
            _dy = dy;
        }

        public void Execute()
        {

            if(_board.GetTile(_player.X + _dx, _player.Y + _dy).Entity!=null)
            { 
                var command = new PushStateCommand(_engine,new CombatState(_engine,_player, _board.GetTile(_player.X + _dx, _player.Y + _dy).Entity)); 
                command.Execute();
                return;
            }
            _player.Move(_dx, _dy, _board);
        }
    }
}
