using RpgGame.Dto;
using RpgGame.Model.Game;

namespace RpgGame.Controller.Shared.Commands;

public interface IPlayerCommand
{
    void Execute(GameModel model, int playerId);
    ClientCommandDto ToDto(int playerId);
}
