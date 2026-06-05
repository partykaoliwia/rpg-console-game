using RpgGame.Model.Game.Combat;

namespace RpgGame.Model.Game.Items;

public abstract class LightWeapon : Weapon
{
    protected LightWeapon(int damage)
        : base(damage)
    {
    }

    public override int SoundRange => 5;

    public override int GetFullDamage(Player player)
    {
        return Damage + player.Luck + player.Dexterity;
    }

    public override CombatStats AcceptAttack(IAttackVisitor visitor, Player player)
    {
        return visitor.Visit(this, player);
    }
}
