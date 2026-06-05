using RpgGame.Dto;

namespace RpgGame.Controller.Shared.Commands;

public sealed class CommandRegistry
{
    private readonly Dictionary<ClientCommandKind, Func<ClientCommandDto, IPlayerCommand>> _fromDto;

    public CommandRegistry()
    {
        _fromDto = new Dictionary<ClientCommandKind, Func<ClientCommandDto, IPlayerCommand>>
        {
            { ClientCommandKind.Move, dto => new MoveCommand(dto.DeltaX, dto.DeltaY) },
            { ClientCommandKind.Collect, dto => new CollectCommand(dto.ItemIndex) },
            { ClientCommandKind.Attack, dto => new AttackCommand(dto.TargetId, dto.AttackKind, dto.HandSlot) },
            { ClientCommandKind.Equip, dto => new EquipCommand(dto.ItemIndex, dto.HandSlot) },
            { ClientCommandKind.Unequip, dto => new UnequipCommand(dto.HandSlot) },
            { ClientCommandKind.DropItem, dto => new DropItemCommand(dto.ItemIndex) },
            { ClientCommandKind.BeginCombat, _ => new BeginCombatCommand() },
            { ClientCommandKind.EndCombat, _ => new EndCombatCommand() }
        };
    }

    public IPlayerCommand FromDto(ClientCommandDto dto)
    {
        if (!_fromDto.TryGetValue(dto.Kind, out var factory))
        {
            throw new InvalidOperationException($"Unsupported command kind: {dto.Kind}.");
        }

        return factory(dto);
    }
}
