using System;
using RpgGame.Characters;

namespace RpgGame.Engine.Commands
{
    public class UnequipCommand : ICommand
    {
        private readonly Player _player;
        private readonly HandSlot _slot;

        public UnequipCommand(Player player, HandSlot slot)
        {
            _player = player;
            _slot = slot;
        }

        public void Execute()
        {
            // Prosta decyzja w oparciu o przekazany slot
            if (_slot == HandSlot.Left)
            {
                _player.UnequipLeft();
            }
            else
            {
                _player.UnequipRight();
            }
        }
    }
}