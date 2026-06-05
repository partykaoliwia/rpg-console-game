using RpgGame.Controller.Shared.Commands;
using RpgGame.Controller.Shared.Input;
using RpgGame.Dto;
using RpgGame.Model.Game;
using RpgGame.Model.Local;
using RpgGame.Model.Local.InputModes;
using RpgGame.Model.Local.Instructions;
using RpgGame.Model.Local.KeyBindings;
using RpgGame.Network;

namespace RpgGame.Controller.Local;

public sealed class ClientInputController : IInputModeHandler
{
    private readonly GameModel _localGameModel;
    private readonly LocalModel _localModel;
    private readonly GameClient _client;
    private readonly Func<KeyBindingProfile> _reloadKeyBindings;
    private readonly Func<KeyBindingProfile, InstructionGuide> _createInstructionGuide;
    private InputHandler _playingInputChain;
    private InputHandler _combatInputChain;
    private InputHandler _weaponInputChain;
    private InputHandler _inventoryInputChain;
    private IPlayerCommand? _pendingCommand;
    private AttackKind? _pendingAttackKind;

    public ClientInputController(
        GameModel localGameModel,
        LocalModel localModel,
        GameClient client,
        Func<KeyBindingProfile> reloadKeyBindings,
        Func<KeyBindingProfile, InstructionGuide> createInstructionGuide)
    {
        _localGameModel = localGameModel;
        _localModel = localModel;
        _client = client;
        _reloadKeyBindings = reloadKeyBindings;
        _createInstructionGuide = createInstructionGuide;
        _playingInputChain = BuildPlayingInputChain();
        _combatInputChain = BuildCombatInputChain();
        _weaponInputChain = BuildWeaponInputChain();
        _inventoryInputChain = BuildInventoryInputChain();
    }

    public bool ShouldQuit { get; private set; }

    public async Task HandleKeyAsync(ConsoleKey key, CancellationToken cancellationToken)
    {
        _pendingCommand = null;
        _localModel.CurrentInputMode.Handle(key, this);

        if (_pendingCommand is not null)
        {
            await _client.SendCommandAsync(_pendingCommand.ToDto(_localModel.LocalPlayerId), cancellationToken);
        }
    }

    public void HandleInstructionKey(ConsoleKey key)
    {
        if (TryReloadKeyBindings(key))
        {
            return;
        }

        if (key == _localModel.KeyBindings.Quit)
        {
            ShouldQuit = true;
            return;
        }

        if (key == _localModel.KeyBindings.ContinueInstructions ||
            key == _localModel.KeyBindings.CloseMenu)
        {
            _localModel.CloseInstructions();
            return;
        }

        RecordUnhandledKey(key);
    }

    public void HandlePlayingKey(ConsoleKey key)
    {
        if (TryReloadKeyBindings(key))
        {
            return;
        }

        if (TryOpenLogHistory(key, _localModel.KeyBindings.OpenLogHistory))
        {
            return;
        }

        if (key == _localModel.KeyBindings.Quit)
        {
            ShouldQuit = true;
            return;
        }

        if (key == _localModel.KeyBindings.OpenInventory)
        {
            _localModel.OpenInventory();
            return;
        }

        if (key == _localModel.KeyBindings.Attack)
        {
            _pendingCommand = new BeginCombatCommand();
            return;
        }

        if (key == _localModel.KeyBindings.Collect)
        {
            BeginLootSelection();
            return;
        }

        _pendingCommand = _playingInputChain.Handle(key);
        if (_pendingCommand is null)
        {
            RecordUnhandledKey(key);
        }
    }

    public void HandleInventoryKey(ConsoleKey key)
    {
        if (TryReloadKeyBindings(key))
        {
            return;
        }

        if (TryOpenLogHistory(key, _localModel.KeyBindings.InventoryOpenLogHistory))
        {
            return;
        }

        if (key == _localModel.KeyBindings.CloseMenu || key == _localModel.KeyBindings.OpenInventory)
        {
            _localModel.CloseMenu();
            return;
        }

        var itemCount = GetInventoryItemCount();
        if (key == _localModel.KeyBindings.MoveUp)
        {
            _localModel.MoveInventoryCursor(-1, itemCount);
            return;
        }

        if (key == _localModel.KeyBindings.MoveDown)
        {
            _localModel.MoveInventoryCursor(1, itemCount);
            return;
        }

        _pendingCommand = _inventoryInputChain.Handle(key);
        if (_pendingCommand is null)
        {
            RecordUnhandledKey(key);
            return;
        }

    }

    public void HandleLootKey(ConsoleKey key)
    {
        if (TryReloadKeyBindings(key))
        {
            return;
        }

        if (TryOpenLogHistory(key, _localModel.KeyBindings.LootOpenLogHistory))
        {
            return;
        }

        if (key == _localModel.KeyBindings.CloseMenu ||
            key == _localModel.KeyBindings.LootCloseMenu ||
            key == _localModel.KeyBindings.Quit)
        {
            _localModel.CloseMenu();
            return;
        }

        var itemCount = GetGroundItemCount();
        if (key == _localModel.KeyBindings.MoveUp)
        {
            _localModel.MoveLootCursor(-1, itemCount);
            return;
        }

        if (key == _localModel.KeyBindings.MoveDown)
        {
            _localModel.MoveLootCursor(1, itemCount);
            return;
        }

        if (key != _localModel.KeyBindings.Collect)
        {
            RecordUnhandledKey(key);
            return;
        }

        _pendingCommand = new CollectCommand(_localModel.LootCursor);
    }

    public void HandleCombatKey(ConsoleKey key)
    {
        if (TryReloadKeyBindings(key))
        {
            return;
        }

        if (TryOpenLogHistory(key, _localModel.KeyBindings.CombatOpenLogHistory))
        {
            return;
        }

        if (key == _localModel.KeyBindings.CancelAttack)
        {
            _pendingCommand = new EndCombatCommand();
            EndAttackSelection();
            return;
        }

        _pendingCommand = _combatInputChain.Handle(key);
        if (_pendingCommand is null)
        {
            RecordUnhandledKey(key);
            return;
        }

        if (_pendingCommand is not null)
        {
            FinishAttackTurn();
        }
    }

    public void HandleWeaponKey(ConsoleKey key)
    {
        if (TryReloadKeyBindings(key))
        {
            return;
        }

        if (TryOpenLogHistory(key, _localModel.KeyBindings.CombatOpenLogHistory))
        {
            return;
        }

        if (key == _localModel.KeyBindings.CancelAttack)
        {
            CancelWeaponChoice();
            return;
        }

        _pendingCommand = _weaponInputChain.Handle(key);
        if (_pendingCommand is null)
        {
            RecordUnhandledKey(key);
            return;
        }

        if (_pendingCommand is not null)
        {
            FinishAttackTurn();
        }
    }

    public void HandleLogHistoryKey(ConsoleKey key)
    {
        if (TryReloadKeyBindings(key))
        {
            return;
        }

        if (key == _localModel.KeyBindings.CloseLogHistory ||
            key == _localModel.KeyBindings.CloseMenu ||
            key == _localModel.KeyBindings.Quit)
        {
            _localModel.CloseMenu();
            return;
        }

        RecordUnhandledKey(key);
    }

    public void HandleGameOverKey(ConsoleKey key)
    {
        ShouldQuit = true;
    }

    private InputHandler BuildPlayingInputChain()
    {
        return new InputChainBuilder(_localModel.KeyBindings)
            .AddKey(bindings => bindings.MoveUp, () => new MoveCommand(0, -1))
            .AddKey(bindings => bindings.MoveDown, () => new MoveCommand(0, 1))
            .AddKey(bindings => bindings.MoveLeft, () => new MoveCommand(-1, 0))
            .AddKey(bindings => bindings.MoveRight, () => new MoveCommand(1, 0))
            .Build();
    }

    private InputHandler BuildCombatInputChain()
    {
        return new InputChainBuilder(_localModel.KeyBindings)
            .AddKey(bindings => bindings.NormalAttack, () => ChooseAttackKind(AttackKind.Normal))
            .AddKey(bindings => bindings.StealthAttack, () => ChooseAttackKind(AttackKind.Stealth))
            .AddKey(bindings => bindings.MagicAttack, () => ChooseAttackKind(AttackKind.Magic))
            .Build();
    }

    private InputHandler BuildWeaponInputChain()
    {
        return new InputChainBuilder(_localModel.KeyBindings)
            .AddKey(bindings => bindings.ChooseLeftWeapon, () => CreatePendingAttackCommand(HandSlotKind.Left))
            .AddKey(bindings => bindings.ChooseRightWeapon, () => CreatePendingAttackCommand(HandSlotKind.Right))
            .Build();
    }

    private InputHandler BuildInventoryInputChain()
    {
        return new InputChainBuilder(_localModel.KeyBindings)
            .AddKey(bindings => bindings.EquipLeft, () => new EquipCommand(_localModel.InventoryCursor, HandSlotKind.Left))
            .AddKey(bindings => bindings.EquipRight, () => new EquipCommand(_localModel.InventoryCursor, HandSlotKind.Right))
            .AddKey(bindings => bindings.UnequipLeft, () => new UnequipCommand(HandSlotKind.Left))
            .AddKey(bindings => bindings.UnequipRight, () => new UnequipCommand(HandSlotKind.Right))
            .AddKey(bindings => bindings.DropItem, () => new DropItemCommand(_localModel.InventoryCursor))
            .Build();
    }

    private void BeginLootSelection()
    {
        var itemCount = GetGroundItemCount();
        if (itemCount <= 0)
        {
            _pendingCommand = new CollectCommand();
            return;
        }

        if (itemCount == 1)
        {
            _pendingCommand = new CollectCommand();
            return;
        }

        _localModel.OpenLoot();
    }

    private IPlayerCommand? ChooseAttackKind(AttackKind attackKind)
    {
        _pendingAttackKind = attackKind;
        if (ShouldChooseWeapon())
        {
            _localModel.BeginWeaponSelection();
            return null;
        }

        return CreateAttackCommand(attackKind, ChooseDefaultHandSlot());
    }

    private IPlayerCommand CreatePendingAttackCommand(HandSlotKind handSlot)
    {
        var attackKind = _pendingAttackKind
            ?? throw new InvalidOperationException("Attack kind must be selected before choosing weapon.");
        return CreateAttackCommand(attackKind, handSlot);
    }

    private IPlayerCommand CreateAttackCommand(AttackKind attackKind, HandSlotKind handSlot)
    {
        var targetId = _localModel.PendingAttackTargetId
            ?? throw new InvalidOperationException("Attack target must be selected before choosing attack kind.");
        return new AttackCommand(targetId, attackKind, handSlot);
    }

    private bool ShouldChooseWeapon()
    {
        lock (_localGameModel.SyncRoot)
        {
            var player = _localGameModel.FindPlayer(_localModel.LocalPlayerId);
            return player?.LeftHand is not null
                && player.RightHand is not null
                && !ReferenceEquals(player.LeftHand, player.RightHand);
        }
    }

    private HandSlotKind ChooseDefaultHandSlot()
    {
        lock (_localGameModel.SyncRoot)
        {
            var player = _localGameModel.FindPlayer(_localModel.LocalPlayerId);
            if (player?.LeftHand is not null)
            {
                return HandSlotKind.Left;
            }

            if (player?.RightHand is not null)
            {
                return HandSlotKind.Right;
            }
        }

        return HandSlotKind.Left;
    }

    private void EndAttackSelection()
    {
        _pendingAttackKind = null;
        _localModel.EndAttackSelection();
    }

    private void CancelWeaponChoice()
    {
        _pendingAttackKind = null;
        _localModel.ReturnToAttackSelection();
    }

    private void FinishAttackTurn()
    {
        var targetId = _localModel.PendingAttackTargetId;
        _pendingAttackKind = null;

        if (targetId.HasValue &&
            _localGameModel.HasEncounterWithEnemy(_localModel.LocalPlayerId, targetId.Value) &&
            _localGameModel.CanPlayerAttackEnemy(_localModel.LocalPlayerId, targetId.Value))
        {
            _localModel.ReturnToAttackSelection();
            return;
        }

        _localModel.EndAttackSelection();
    }

    private bool TryOpenLogHistory(ConsoleKey key, ConsoleKey openLogHistoryKey)
    {
        if (_localModel.CurrentInputMode.ShowsGameOver)
        {
            return false;
        }

        if (key != openLogHistoryKey)
        {
            return false;
        }

        _localModel.OpenLogHistory();
        return true;
    }

    private bool TryReloadKeyBindings(ConsoleKey key)
    {
        if (key != _localModel.KeyBindings.ReloadKeyBindings)
        {
            return false;
        }

        _localModel.ReplaceKeyBindings(_reloadKeyBindings());
        _localModel.ReplaceInstructionGuide(_createInstructionGuide(_localModel.KeyBindings));
        RebuildInputChains();
        _localModel.AddLocalMessage("Keybinds reloaded from file.");
        return true;
    }

    private void RecordUnhandledKey(ConsoleKey key)
    {
        _localModel.AddLocalMessage($"Invalid key, no action assigned to [{key}].");
    }

    private void RebuildInputChains()
    {
        _playingInputChain = BuildPlayingInputChain();
        _combatInputChain = BuildCombatInputChain();
        _weaponInputChain = BuildWeaponInputChain();
        _inventoryInputChain = BuildInventoryInputChain();
    }

    private int GetInventoryItemCount()
    {
        lock (_localGameModel.SyncRoot)
        {
            return _localGameModel.FindPlayer(_localModel.LocalPlayerId)?.Inventory.Count ?? 0;
        }
    }

    private int GetGroundItemCount()
    {
        lock (_localGameModel.SyncRoot)
        {
            var player = _localGameModel.FindPlayer(_localModel.LocalPlayerId);
            if (player is null)
            {
                return 0;
            }

            return _localGameModel.Board.GetTile(player.X, player.Y).Items.Count;
        }
    }
}
