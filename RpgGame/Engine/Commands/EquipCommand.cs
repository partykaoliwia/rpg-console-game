using System;
using RpgGame.Characters;
using RpgGame.Engine.States;

namespace RpgGame.Engine.Commands
{
    public enum HandSlot { Left, Right }

    public class EquipCommand : ICommand
    {
        private readonly Player _player;
        private readonly IMenuState _menu;
        private readonly HandSlot _slot;

        public EquipCommand(Player player, IMenuState menu, HandSlot slot)
        {
            _player = player;
            _menu = menu;
            _slot = slot;
        }

        public void Execute()
        {
            if (_player.Inventory.Count == 0) return;

            // pobieramy przedmiot, na który wskazuje kursor
            var item = _player.Inventory[_menu.CursorIndex];

            // zakładamy go w odpowiednią rękę
            if (_slot == HandSlot.Left)
            {
                item.EquipToLeftHand(_player);
            }
            else
            {
                item.EquipToRightHand(_player);
            }

            // poprawa kursora
            if (_menu.CursorIndex >= _player.Inventory.Count && _player.Inventory.Count > 0)
            {
                _menu.CursorIndex = _player.Inventory.Count - 1;
            }
        }
    }
}