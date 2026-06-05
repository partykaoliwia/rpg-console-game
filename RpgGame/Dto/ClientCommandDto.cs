namespace RpgGame.Dto;

public sealed class ClientCommandDto
{
    public int PlayerId { get; set; }
    public ClientCommandKind Kind { get; set; }
    public AttackKind AttackKind { get; set; } = AttackKind.Normal;
    public HandSlotKind HandSlot { get; set; } = HandSlotKind.Left;
    public int DeltaX { get; set; }
    public int DeltaY { get; set; }
    public int TargetId { get; set; }
    public int ItemIndex { get; set; }
}
