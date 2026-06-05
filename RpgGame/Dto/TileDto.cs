namespace RpgGame.Dto;

public sealed class TileDto
{
    public int X { get; set; }
    public int Y { get; set; }
    public TileKind Kind { get; set; }
    public List<ItemDto> Items { get; set; } = new();
}
