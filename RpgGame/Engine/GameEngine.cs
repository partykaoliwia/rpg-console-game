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
        private bool _isLootMode = false;
        private int _lootCursor = 0;
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
                { ConsoleKey.E, () => OpenLootMenu() },
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
        private void OpenLootMenu()
        {
            var currentTile = _board.GetTile(_player.X, _player.Y);
            if (currentTile.Items.Count == 0 || _isInventoryMode) return;
            if (currentTile.Items.Count == 1)
            {
                _player.CollectSpecific(0, _board);
            }
            else
            {
                _isLootMode = true;
                _lootCursor = 0;
            }
        }
        public void RunGame()
        {
            Console.CursorVisible = false;
            _board.DrawBoard();
            while (_isRunning)
            {
                RenderFrame();
                HandleInput();
            }
        }
        public void HandleInput()
        {
            var keyInfo = Console.ReadKey(intercept: true);
            if (_isInventoryMode)
            {
                HandleInventoryInput(keyInfo.Key);
            }
            else if (_isLootMode)
            {
                HandleLootInput(keyInfo.Key);
            }
            else
            {
                if (_keyBindings.TryGetValue(keyInfo.Key, out Action action))
                {
                    action.Invoke();
                }
            }
        }
        private void HandleLootInput(ConsoleKey key)
        {
            var currentTile = _board.GetTile(_player.X, _player.Y);
            switch (key)
            {
                case ConsoleKey.W:
                    _lootCursor--;
                    if (_lootCursor < 0) _lootCursor = currentTile.Items.Count - 1;
                    break;

                case ConsoleKey.S:
                    _lootCursor++;
                    if (_lootCursor >= currentTile.Items.Count) _lootCursor = 0;
                    break;

                case ConsoleKey.E:
                    _player.CollectSpecific(_lootCursor, _board);
                    if (currentTile.Items.Count == 0)
                    {
                        _isLootMode = false;
                    }
                    else if (_lootCursor >= currentTile.Items.Count)
                    {
                        _lootCursor = currentTile.Items.Count - 1;
                    }
                    break;
                case ConsoleKey.Q:
                    _isLootMode = false;
                    break;
            }
        }
        private void ValidateCursor()
        {
            if (_player.Inventory.Count == 0)
            {
                _isInventoryMode = false;
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
                case ConsoleKey.W:
                    _inventoryCursor--;
                    if (_inventoryCursor < 0) _inventoryCursor = _player.Inventory.Count - 1;
                    break;
                case ConsoleKey.S:
                    _inventoryCursor++;
                    if (_inventoryCursor >= _player.Inventory.Count) _inventoryCursor = 0;
                    break;
                case ConsoleKey.L:
                    _player.Inventory[_inventoryCursor].EquipToLeftHand(_player);
                    ValidateCursor();
                    break;
                case ConsoleKey.R:
                    _player.Inventory[_inventoryCursor].EquipToRightHand(_player);
                    ValidateCursor();
                    break;
                case ConsoleKey.D1:
                    _player.UnequipLeft();
                    break;
                case ConsoleKey.D2:
                    _player.UnequipRight();
                    break;
                case ConsoleKey.Q:
                    _player.DropItem(_inventoryCursor, _board);
                    ValidateCursor();
                    break;
                case ConsoleKey.I:
                case ConsoleKey.Escape:
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
                    Console.SetCursorPosition(x, y);
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
            var currentTile = _board.GetTile(_player.X, _player.Y);
            _ui.Draw(_player, currentTile, _isInventoryMode, _inventoryCursor, _isLootMode, _lootCursor);
        }
    }
}
