using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RpgGame.Gameboard;

namespace RpgGame.Engine
{
    public class GameEngine
    {
        private readonly Board _board;
        private readonly Player.Player _player;
        private readonly Dictionary<ConsoleKey, Action> _keyBindings;
        private readonly GameUI _ui;
        private bool _isRunning;

        public GameEngine(Board board, Player.Player player)
        {
            _board = board;
            _player = player;
            _ui = new GameUI(45, 0);
            _isRunning = true;
            _keyBindings = new Dictionary<ConsoleKey, Action>
            {
                { ConsoleKey.W, () => _player.Move(0, -1, _board) },
                { ConsoleKey.A, () => _player.Move(-1, 0, _board) },
                { ConsoleKey.S, () => _player.Move(0, 1, _board) },
                { ConsoleKey.D, () => _player.Move(1, 0, _board) },
                { ConsoleKey.E, () => _player.Collect(_board) },
                { ConsoleKey.Escape, () => _isRunning = false }
            };
        }

        public void RunGame()
        {
            Console.CursorVisible = false;
            _board.DrawBoard();
            while(_isRunning)
            {
                RenderFrame();
                HandleInput();
            }
        }
        public void HandleInput()
        {
            //if(Console.KeyAvailable)
            //{
                var keyInfo = Console.ReadKey(true);
                if(_keyBindings.TryGetValue(keyInfo.Key, out Action action))
                {
                    action.Invoke();
                }
            //}
        }
        public void RenderFrame()
        {
            Console.SetCursorPosition(0, 0);
            for (int y = 0; y < _board.Height; y++)
            {
                for (int x = 0; x < _board.Width; x++)
                {
                    Console.SetCursorPosition(x, y); // Ustawiamy precyzyjnie znak po znaku
                    if (_player.X == x && _player.Y == y)
                    {
                        Console.Write(_player.Symbol);
                    }
                    else
                    {
                        var tile = _board.GetTile(x, y);
                        char symbolToDraw = tile.Items.Count > 0 ? tile.Items[0].Symbol : tile.Symbol;
                        Console.Write(symbolToDraw);
                    }
                }
            }

            // Wywołanie rysowania UI
            var currentTile = _board.GetTile(_player.X, _player.Y);
            _ui.Draw(_player, currentTile);
        }
    }
}
