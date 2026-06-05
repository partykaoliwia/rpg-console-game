namespace RpgGame.Dto;

public sealed class ItemDto
{
    public ItemKind Kind { get; set; }
    public string Name { get; set; } = "";
    public int Value { get; set; }
    public int Damage { get; set; }
    public List<ItemModifierKind> Modifiers { get; set; } = new();
}
