using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RpgGame.Characters;
using RpgGame.Gameboard;
using RpgGame.Items;

namespace RpgGame.Engine.States
{
    public class InventoryState : IGameState
    {
        private readonly GameEngine _engine;
        private readonly Player _player;
        private readonly Board _board;
        private readonly Dictionary<ConsoleKey, Action> _keyBindings;
        private int _cursorIndex = 0;
        private string _errorMessage = string.Empty;

        public InventoryState(GameEngine engine, Player player, Board board)
        {
            _engine = engine;
            _player = player;
            _board = board;

            _keyBindings = new Dictionary<ConsoleKey, Action>
            {
                { ConsoleKey.W, () => MoveCursor(-1) }, //up
                { ConsoleKey.S, () => MoveCursor(1) }, //down
                { ConsoleKey.L, () => { GetSelectedItem()?.EquipToLeftHand(_player); 
                                        ValidateState(); } },
                { ConsoleKey.R, () => { GetSelectedItem()?.EquipToRightHand(_player); 
                                        ValidateState(); } },
                { ConsoleKey.D1, () => _player.UnequipLeft() },
                { ConsoleKey.D2, () => _player.UnequipRight() },
                { ConsoleKey.Q, () => { DropSelectedItem(); 
                                        ValidateState(); } },
                { ConsoleKey.I, () => _engine.PopState() },
                { ConsoleKey.Escape, () => _engine.PopState() }
            };
        }

        private void MoveCursor(int direction)
        {
            //direction -1 up, 1 down
            if (_player.Inventory.Count == 0) return;
            _cursorIndex += direction;
            if (_cursorIndex < 0) _cursorIndex = _player.Inventory.Count - 1; // za bardzo w gore, to idziemy od dolu
            else if ( _cursorIndex >= _player.Inventory.Count) _cursorIndex = 0; // za bardzo w dol, to idziemy od gory
        }
        private IItem GetSelectedItem()
        {
            if (_player.Inventory.Count == 0) return null;
            return _player.Inventory[_cursorIndex];
        }
        private void DropSelectedItem()
        {
            if(_player.Inventory.Count > 0)
            {
                _player.DropItem(_cursorIndex, _board);
            }
        }
        private void ValidateState()
        {
            if(_player.Inventory.Count == 0)
            {
                _engine.PopState(); //automatyczne wyjscie z InventoryMode, gdy pozbedziemy sie z inventory przedmiotow
            }
            else if(_cursorIndex >= _player.Inventory.Count)
            {
                _cursorIndex = _player.Inventory.Count - 1; //ustawiamy kursor na ostatni przedmiot
            }
        }
        public void HandleInput(ConsoleKey key)
        {
            _errorMessage = string.Empty;

            if (_keyBindings.TryGetValue(key, out Action action))
            {
                action.Invoke();
            }
            else
            {
                _errorMessage = $"Klawisz '{key}' nie ma funkcji w ekwipunku.";
            }
        }

        public void Draw(ConsoleRenderer renderer)
        {
            renderer.DrawBoard(_board, _player);
            renderer.DrawInventoryMenu(_player, _cursorIndex, _errorMessage);
        }
    }
}
