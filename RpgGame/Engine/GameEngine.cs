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
        private bool _isInventoryMode = false;
        private int _inventoryCursor = 0;

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
                { ConsoleKey.Escape, () => _isRunning = false },
                { ConsoleKey.I, () => OpenInventory() }
            };
        }
        private void OpenInventory()
        {
            if (_player.Inventory.Count > 0)
            {
                _isInventoryMode = !_isInventoryMode;
                _inventoryCursor = 0;
            }
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
            var keyInfo = Console.ReadKey(intercept: true);

            // Jeśli plecak jest otwarty, przejmujemy kontrolę nad klawiszami
            if (_isInventoryMode)
            {
                HandleInventoryInput(keyInfo.Key);
            }
            else
            {
                // Normalne chodzenie
                if (_keyBindings.TryGetValue(keyInfo.Key, out Action action))
                {
                    action.Invoke();
                }
            }
        }
        private void ValidateCursor()
        {
            if (_player.Inventory.Count == 0)
            {
                _isInventoryMode = false; // Zamykamy plecak, jeśli jest pusty
            }
            else if (_inventoryCursor >= _player.Inventory.Count)
            {
                _inventoryCursor = _player.Inventory.Count - 1;
            }
        }
        public void HandleInventoryInput(ConsoleKey key)
        {
            switch (key)
            {
                case ConsoleKey.W: // W górę
                    _inventoryCursor--;
                    if (_inventoryCursor < 0) _inventoryCursor = _player.Inventory.Count - 1;
                    break;

                case ConsoleKey.S: // W dół
                    _inventoryCursor++;
                    if (_inventoryCursor >= _player.Inventory.Count) _inventoryCursor = 0;
                    break;

                case ConsoleKey.L: // Wyposaż w lewą rękę
                    // Magia polimorfizmu - broń sama zadecyduje, czy zajmie jedną czy obie ręce!
                    _player.ClearHands(); // Ściągamy stare bronie do plecaka
                    _player.Inventory[_inventoryCursor].EquipToLeftHand(_player);
                    ValidateCursor();
                    break;

                case ConsoleKey.R: // Wyposaż w prawą rękę
                    _player.ClearHands();
                    _player.Inventory[_inventoryCursor].EquipToRightHand(_player);
                    ValidateCursor();
                    break;

                case ConsoleKey.Q: // Wyrzuć na ziemię
                    _player.DropItem(_inventoryCursor, _board);
                    ValidateCursor();
                    break;

                case ConsoleKey.I:
                case ConsoleKey.Escape: // Wyjście z menu
                    _isInventoryMode = false;
                    break;
            }
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
            _ui.Draw(_player, currentTile,_isInventoryMode,_inventoryCursor);
        }
    }
}
