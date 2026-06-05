using RpgGame.Model.Game.Combat;

namespace RpgGame.Model.Game.Items;

public abstract class MagicWeapon : Weapon
{
    protected MagicWeapon(int damage)
        : base(damage)
    {
    }

    public override int SoundRange => 10;

    public override int GetFullDamage(Player player)
    {
        return Damage + player.Wisdom;
    }

    public override CombatStats AcceptAttack(IAttackVisitor visitor, Player player)
    {
        return visitor.Visit(this, player);
    }
}
