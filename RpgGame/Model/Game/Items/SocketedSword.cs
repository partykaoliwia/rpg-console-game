namespace RpgGame.Model.Game.Items;

public sealed class SocketedSword : HeavyWeapon
{
    private readonly ItemSlotCollection _slots;

    public SocketedSword(int damage, int slotCount = 2)
        : base(damage)
    {
        _slots = new ItemSlotCollection(slotCount, ItemSlotKind.Weapon);
    }

    public override string Name => "Socketed Sword";

    public override PlayerStats Bonuses => new PlayerStats
    {
        Strength = 2
    } + _slots.Bonuses;

    public override IReadOnlyList<ItemSlot> Slots => _slots.Slots;

    public override bool TryPlaceInSlot(int slotIndex, IItem item)
    {
        return _slots.TryPlaceInSlot(slotIndex, item);
    }

    public override void Accept(IItemVisitor visitor)
    {
        visitor.Visit(this);
    }
}
