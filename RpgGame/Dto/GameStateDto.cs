namespace RpgGame.Dto;

public sealed class GameStateDto
{
    public string DungeonThemeName { get; set; } = "";
    public BoardDto Board { get; set; } = new();
    public List<PlayerDto> Players { get; set; } = new();
    public List<EnemyDto> Enemies { get; set; } = new();
    public List<CombatEncounterDto> Encounters { get; set; } = new();
    public List<string> Events { get; set; } = new();
}
