namespace RpgGame.Model.Game.Items;

public static class SlottedItemRecipes
{
    public static IItem CreateShowcaseSword(int damage)
    {
        var sword = new SocketedSword(damage);
        sword.TryPlaceInSlot(0, new LuckStone());

        var outerHandle = new BronzeHandle();
        outerHandle.TryPlaceInSlot(0, new StrengthStone());

        var innerHandle = new LaurelHandle();
        innerHandle.TryPlaceInSlot(1, new WisdomStone());

        outerHandle.TryPlaceInSlot(1, innerHandle);
        sword.TryPlaceInSlot(1, outerHandle);

        return sword;
    }

    public static IItem CreateHandle(int index)
    {
        if (index % 2 == 0)
        {
            return new BronzeHandle();
        }

        return new LaurelHandle();
    }
}
