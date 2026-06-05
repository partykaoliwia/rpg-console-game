using RpgGame.Model.Game.Combat;

namespace RpgGame.Model.Game.Items;

public abstract class HeavyWeapon : Weapon
{
    protected HeavyWeapon(int damage)
        : base(damage)
    {
    }

    public override int SoundRange => 15;

    public override int GetFullDamage(Player player)
    {
        return Damage + player.Strength + player.Aggression;
    }

    public override CombatStats AcceptAttack(IAttackVisitor visitor, Player player)
    {
        return visitor.Visit(this, player);
    }
}
