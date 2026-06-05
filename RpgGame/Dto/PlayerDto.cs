namespace RpgGame.Dto;

public sealed class PlayerDto
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public int X { get; set; }
    public int Y { get; set; }
    public int Health { get; set; }
    public int BaseStrength { get; set; }
    public int BaseLuck { get; set; }
    public int BaseAggression { get; set; }
    public int BaseWisdom { get; set; }
    public int BaseDexterity { get; set; }
    public int Armor { get; set; }
    public Dictionary<string, int> Wallet { get; set; } = new();
    public List<ItemDto> Inventory { get; set; } = new();
    public ItemDto? LeftHand { get; set; }
    public ItemDto? RightHand { get; set; }
    public bool UsesBothHands { get; set; }
}
