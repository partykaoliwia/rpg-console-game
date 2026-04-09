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
        PlayerStats Bonuses { get; }
        bool PickUp(Player player);
        void EquipToLeftHand(Player player, IItem instanceToEquip = null);
        void EquipToRightHand(Player player, IItem instanceToEquip = null);
        void Unequip(Player player);
        CombatStats Accept(IAttackVisitor visitor, Player player);
    }
}
