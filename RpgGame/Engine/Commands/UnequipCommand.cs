using System;
using RpgGame.Characters;
using RpgGame.Engine.Logging;

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
            if (_slot == HandSlot.Left)
            {
                Logger.Instance.Log($"{_player.Name} unequipped the item in the left hand.");
                _player.UnequipLeft();
            }
            else
            {
                Logger.Instance.Log($"{_player.Name} unequipped the item in the right hand.");
                _player.UnequipRight();
            }
        }
    }
}