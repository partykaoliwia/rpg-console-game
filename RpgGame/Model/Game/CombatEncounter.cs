namespace RpgGame.Model.Game;

public sealed class CombatEncounter
{
    public CombatEncounter(int sequence, int playerId, int enemyId)
    {
        Sequence = sequence;
        PlayerId = playerId;
        EnemyId = enemyId;
    }

    public int Sequence { get; }
    public int PlayerId { get; }
    public int EnemyId { get; }
}
