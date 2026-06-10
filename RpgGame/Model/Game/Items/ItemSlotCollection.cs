namespace RpgGame.Model.Game.Items;

public sealed class ItemSlotCollection
{
    private readonly List<ItemSlot> _slots = new();

    public ItemSlotCollection(int slotCount, ItemSlotKind kind)
    {
        for (var index = 0; index < slotCount; index++)
        {
            _slots.Add(new ItemSlot(kind));
        }
    }

    public IReadOnlyList<ItemSlot> Slots => _slots;

    public PlayerStats Bonuses
    {
        get
        {
            var bonuses = new PlayerStats();
            foreach (var slot in _slots)
            {
                if (slot.Item != null)
                {
                    bonuses += slot.Item.Bonuses;
                }
            }

            return bonuses;
        }
    }

    public bool TryPlaceInSlot(int slotIndex, IItem item)
    {
        if (slotIndex < 0 || slotIndex >= _slots.Count)
        {
            return false;
        }

        return _slots[slotIndex].TryPlace(item);
    }
}
