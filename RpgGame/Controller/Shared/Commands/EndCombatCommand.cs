using RpgGame.Dto;
using RpgGame.Model.Game;

namespace RpgGame.Controller.Shared.Commands;

public sealed class EndCombatCommand : IPlayerCommand
{
    public void Execute(GameModel model, int playerId)
    {
        model.EndEncounterForPlayer(playerId);
    }

    public ClientCommandDto ToDto(int playerId)
    {
        return new ClientCommandDto
        {
            PlayerId = playerId,
            Kind = ClientCommandKind.EndCombat
        };
    }
}
