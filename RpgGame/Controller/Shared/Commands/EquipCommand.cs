using RpgGame.Dto;
using RpgGame.Model.Game;

namespace RpgGame.Controller.Shared.Commands;

public sealed class EquipCommand : IPlayerCommand
{
    private static readonly IReadOnlyDictionary<HandSlotKind, Action<GameModel, int, int>> EquipActions =
        new Dictionary<HandSlotKind, Action<GameModel, int, int>>
        {
            { HandSlotKind.Left, (model, playerId, itemIndex) => model.EquipLeftHand(playerId, itemIndex) },
            { HandSlotKind.Right, (model, playerId, itemIndex) => model.EquipRightHand(playerId, itemIndex) }
        };

    public EquipCommand(int itemIndex, HandSlotKind handSlot)
    {
        ItemIndex = itemIndex;
        HandSlot = handSlot;
    }

    public int ItemIndex { get; }
    public HandSlotKind HandSlot { get; }

    public void Execute(GameModel model, int playerId)
    {
        if (!EquipActions.TryGetValue(HandSlot, out var equipAction))
        {
            throw new InvalidOperationException($"Unsupported hand slot: {HandSlot}.");
        }

        equipAction(model, playerId, ItemIndex);
    }

    public ClientCommandDto ToDto(int playerId)
    {
        return new ClientCommandDto
        {
            PlayerId = playerId,
            Kind = ClientCommandKind.Equip,
            HandSlot = HandSlot,
            ItemIndex = ItemIndex
        };
    }
}
