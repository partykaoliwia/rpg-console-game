using RpgGame.Dto;
using RpgGame.Model.Game;

namespace RpgGame.Controller.Shared.Commands;

public sealed class UnequipCommand : IPlayerCommand
{
    private static readonly IReadOnlyDictionary<HandSlotKind, Action<GameModel, int>> UnequipActions =
        new Dictionary<HandSlotKind, Action<GameModel, int>>
        {
            { HandSlotKind.Left, (model, playerId) => model.UnequipLeftHand(playerId) },
            { HandSlotKind.Right, (model, playerId) => model.UnequipRightHand(playerId) }
        };

    public UnequipCommand(HandSlotKind handSlot)
    {
        HandSlot = handSlot;
    }

    public HandSlotKind HandSlot { get; }

    public void Execute(GameModel model, int playerId)
    {
        if (!UnequipActions.TryGetValue(HandSlot, out var unequipAction))
        {
            throw new InvalidOperationException($"Unsupported hand slot: {HandSlot}.");
        }

        unequipAction(model, playerId);
    }

    public ClientCommandDto ToDto(int playerId)
    {
        return new ClientCommandDto
        {
            PlayerId = playerId,
            Kind = ClientCommandKind.Unequip,
            HandSlot = HandSlot
        };
    }
}
