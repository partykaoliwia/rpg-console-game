using RpgGame.Dto;
using RpgGame.Model.Game;

namespace RpgGame.Controller.Shared.Commands;

public sealed class MoveCommand : IPlayerCommand
{
    public MoveCommand(int deltaX, int deltaY)
    {
        DeltaX = deltaX;
        DeltaY = deltaY;
    }

    public int DeltaX { get; }
    public int DeltaY { get; }

    public void Execute(GameModel model, int playerId)
    {
        model.MovePlayer(playerId, DeltaX, DeltaY);
    }

    public ClientCommandDto ToDto(int playerId)
    {
        return new ClientCommandDto
        {
            PlayerId = playerId,
            Kind = ClientCommandKind.Move,
            DeltaX = DeltaX,
            DeltaY = DeltaY
        };
    }
}
