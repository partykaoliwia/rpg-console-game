using RpgGame.Dto;
using RpgGame.Model.Game;

namespace RpgGame.Controller.Shared.Commands;

public sealed class DropItemCommand : IPlayerCommand
{
    public DropItemCommand(int itemIndex)
    {
        ItemIndex = itemIndex;
    }

    public int ItemIndex { get; }

    public void Execute(GameModel model, int playerId)
    {
        model.DropInventoryItem(playerId, ItemIndex);
    }

    public ClientCommandDto ToDto(int playerId)
    {
        return new ClientCommandDto
        {
            PlayerId = playerId,
            Kind = ClientCommandKind.DropItem,
            ItemIndex = ItemIndex
        };
    }
}
