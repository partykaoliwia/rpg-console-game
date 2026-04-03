using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RpgGame.Characters;
using RpgGame.Combat;

namespace RpgGame.Items.Modifiers
{
    public abstract class ItemDecorator : IItem
    {
        protected IItem _decorated;
        protected ItemDecorator(IItem decorated)
        {
            _decorated = decorated;
        }

        public virtual string Name => _decorated.Name;

        public virtual string Symbol => _decorated.Symbol;

        public virtual CombatStats Accept(IAttackVisitor visitor, Player player)
        {
            return _decorated.Accept(visitor, player);
        }
        public Color ItemColor => _decorated.ItemColor;
        public virtual bool PickUp(Player player) => _decorated.PickUp(player);
        public virtual void EquipToLeftHand(Player player) => _decorated.EquipToLeftHand(player);
        public virtual void EquipToRightHand(Player player) => _decorated.EquipToRightHand(player);
        public virtual void Unequip(Player player) => _decorated.Unequip(player);
    }
}
