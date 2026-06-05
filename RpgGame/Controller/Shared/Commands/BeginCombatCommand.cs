using RpgGame.Dto;
using RpgGame.Model.Game;

namespace RpgGame.Controller.Shared.Commands;

public sealed class BeginCombatCommand : IPlayerCommand
{
    public void Execute(GameModel model, int playerId)
    {
        model.BeginEncounterWithAdjacentEnemy(playerId);
    }

    public ClientCommandDto ToDto(int playerId)
    {
        return new ClientCommandDto
        {
            PlayerId = playerId,
            Kind = ClientCommandKind.BeginCombat
        };
    }
}
