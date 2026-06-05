namespace RpgGame.Dto;

public sealed class ServerStateDto
{
    public int StateSequence { get; set; }
    public GameStateDto State { get; set; } = new();
}
