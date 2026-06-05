namespace RpgGame.Dto;

public sealed class WelcomeDto
{
    public int PlayerId { get; set; }
    public int StateSequence { get; set; }
    public GameStateDto State { get; set; } = new();
}
