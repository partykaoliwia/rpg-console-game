namespace RpgGame.Model.Local.KeyBindings;

public sealed class KeyBindingProfile
{
    public ConsoleKey MoveUp { get; init; } = ConsoleKey.W;
    public ConsoleKey MoveDown { get; init; } = ConsoleKey.S;
    public ConsoleKey MoveLeft { get; init; } = ConsoleKey.A;
    public ConsoleKey MoveRight { get; init; } = ConsoleKey.D;
    public ConsoleKey Collect { get; init; } = ConsoleKey.E;
    public ConsoleKey Attack { get; init; } = ConsoleKey.J;
    public ConsoleKey NormalAttack { get; init; } = ConsoleKey.D1;
    public ConsoleKey StealthAttack { get; init; } = ConsoleKey.D2;
    public ConsoleKey MagicAttack { get; init; } = ConsoleKey.D3;
    public ConsoleKey CancelAttack { get; init; } = ConsoleKey.Q;
    public ConsoleKey ChooseLeftWeapon { get; init; } = ConsoleKey.L;
    public ConsoleKey ChooseRightWeapon { get; init; } = ConsoleKey.R;
    public ConsoleKey OpenInventory { get; init; } = ConsoleKey.I;
    public ConsoleKey CloseMenu { get; init; } = ConsoleKey.Escape;
    public ConsoleKey DropItem { get; init; } = ConsoleKey.D;
    public ConsoleKey EquipLeft { get; init; } = ConsoleKey.L;
    public ConsoleKey EquipRight { get; init; } = ConsoleKey.R;
    public ConsoleKey UnequipLeft { get; init; } = ConsoleKey.D1;
    public ConsoleKey UnequipRight { get; init; } = ConsoleKey.D2;
    public ConsoleKey OpenSlots { get; init; } = ConsoleKey.Enter;
    public ConsoleKey LeaveSlots { get; init; } = ConsoleKey.Backspace;
    public ConsoleKey OpenLogHistory { get; init; } = ConsoleKey.H;
    public ConsoleKey InventoryOpenLogHistory { get; init; } = ConsoleKey.H;
    public ConsoleKey LootOpenLogHistory { get; init; } = ConsoleKey.H;
    public ConsoleKey CombatOpenLogHistory { get; init; } = ConsoleKey.H;
    public ConsoleKey CloseLogHistory { get; init; } = ConsoleKey.H;
    public ConsoleKey LootCloseMenu { get; init; } = ConsoleKey.Q;
    public ConsoleKey ReloadKeyBindings { get; init; } = ConsoleKey.F5;
    public ConsoleKey ContinueInstructions { get; init; } = ConsoleKey.Enter;
    public ConsoleKey Quit { get; init; } = ConsoleKey.Q;
}
