using RpgGame.Dto;
using RpgGame.Model.Game;

namespace RpgGame.Controller.Shared.Commands;

public sealed class CollectCommand : IPlayerCommand
{
    public CollectCommand()
        : this(0)
    {
    }

    public CollectCommand(int itemIndex)
    {
        ItemIndex = itemIndex;
    }

    public int ItemIndex { get; }

    public void Execute(GameModel model, int playerId)
    {
        model.CollectItem(playerId, ItemIndex);
    }

    public ClientCommandDto ToDto(int playerId)
    {
        return new ClientCommandDto
        {
            PlayerId = playerId,
            Kind = ClientCommandKind.Collect,
            ItemIndex = ItemIndex
        };
    }
}
