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
            int newX = _player.X + _dx;
            int newY = _player.Y + _dy;
            ITile targetTile = _board.GetTile(newX, newY);
            if(targetTile.Entity != null)
            {
                var command = new PushStateCommand(_engine, new CombatState(_engine, _player, targetTile.Entity, targetTile));
                command.Execute();
                return;
            }
            if (targetTile.IsWalkable)
            { 
                _player.SetPosition(newX, newY); 
            }
            else
            {
                Logger.Instance.Log("Player tried to move to a non-walkable tile.");
                return;
            }
        }
    }
}
