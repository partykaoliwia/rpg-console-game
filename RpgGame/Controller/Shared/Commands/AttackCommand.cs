using RpgGame.Dto;
using RpgGame.Model.Game;
using RpgGame.Model.Game.Combat;

namespace RpgGame.Controller.Shared.Commands;

public sealed class AttackCommand : IPlayerCommand
{
    private static readonly IReadOnlyDictionary<AttackKind, Func<IAttackVisitor>> AttackFactories =
        new Dictionary<AttackKind, Func<IAttackVisitor>>
        {
            { AttackKind.Normal, () => new NormalAttack() },
            { AttackKind.Stealth, () => new StealthAttack() },
            { AttackKind.Magic, () => new MagicAttack() }
        };

    private static readonly IReadOnlyDictionary<HandSlotKind, Action<GameModel, int, int, IAttackVisitor>> AttackActions =
        new Dictionary<HandSlotKind, Action<GameModel, int, int, IAttackVisitor>>
        {
            { HandSlotKind.Left, (model, playerId, targetId, attack) => model.AttackEnemyWithLeftHand(playerId, targetId, attack) },
            { HandSlotKind.Right, (model, playerId, targetId, attack) => model.AttackEnemyWithRightHand(playerId, targetId, attack) }
        };

    public AttackCommand(int targetId)
        : this(targetId, AttackKind.Normal, HandSlotKind.Left)
    {
    }

    public AttackCommand(int targetId, AttackKind attackKind)
        : this(targetId, attackKind, HandSlotKind.Left)
    {
    }

    public AttackCommand(int targetId, AttackKind attackKind, HandSlotKind handSlot)
    {
        TargetId = targetId;
        AttackKind = attackKind;
        HandSlot = handSlot;
    }

    public int TargetId { get; }
    public AttackKind AttackKind { get; }
    public HandSlotKind HandSlot { get; }

    public void Execute(GameModel model, int playerId)
    {
        if (!AttackFactories.TryGetValue(AttackKind, out var factory))
        {
            throw new InvalidOperationException($"Unsupported attack kind: {AttackKind}.");
        }

        if (!AttackActions.TryGetValue(HandSlot, out var attackAction))
        {
            throw new InvalidOperationException($"Unsupported hand slot: {HandSlot}.");
        }

        attackAction(model, playerId, TargetId, factory());
    }

    public ClientCommandDto ToDto(int playerId)
    {
        return new ClientCommandDto
        {
            PlayerId = playerId,
            Kind = ClientCommandKind.Attack,
            AttackKind = AttackKind,
            HandSlot = HandSlot,
            TargetId = TargetId
        };
    }
}
