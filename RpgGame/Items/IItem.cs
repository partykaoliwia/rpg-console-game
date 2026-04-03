using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RpgGame.Characters;
using RpgGame.Combat;
using RpgGame.Common;

namespace RpgGame.Items
{
    public interface IItem : IDrawable
    {
        string Name { get; }
        bool PickUp(Player player);
        void EquipToLeftHand(Player player);
        void EquipToRightHand(Player player);
        void Unequip(Player player);
        CombatStats Accept(IAttackVisitor visitor, Player player);
    }
}
