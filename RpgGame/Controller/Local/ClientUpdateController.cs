using RpgGame.Dto;
using RpgGame.Model.Game;
using RpgGame.Model.Local;

namespace RpgGame.Controller.Local;

public sealed class ClientUpdateController
{
    private readonly GameModel _localGameModel;
    private readonly LocalModel _localModel;
    private readonly GameStateMapper _mapper;
    private int _lastAppliedStateSequence;

    public ClientUpdateController(
        GameModel localGameModel,
        LocalModel localModel,
        GameStateMapper mapper,
        int initialStateSequence = 0)
    {
        _localGameModel = localGameModel;
        _localModel = localModel;
        _mapper = mapper;
        _lastAppliedStateSequence = initialStateSequence;
    }

    public void Apply(ServerStateDto serverState)
    {
        if (serverState.StateSequence <= _lastAppliedStateSequence)
        {
            return;
        }

        _lastAppliedStateSequence = serverState.StateSequence;
        _mapper.ApplyTo(_localGameModel, serverState.State);
        SyncMenuState();
        SyncEncounter();
        SyncCombatSelection();
        SyncGameOver();
    }

    private void SyncMenuState()
    {
        var mode = _localModel.CurrentInputMode;
        if (mode.ShowsInventory)
        {
            var itemCount = GetInventoryItemCount();
            if (itemCount <= 0)
            {
                _localModel.CloseMenu();
                return;
            }

            _localModel.ClampInventoryCursor(itemCount);
            if (mode.ShowsSlots)
            {
                var slotCount = GetCurrentSlotCount();
                if (slotCount < 0)
                {
                    _localModel.CloseMenu();
                    return;
                }

                _localModel.ClampSlotCursor(slotCount);
            }

            return;
        }

        if (mode.ShowsLoot)
        {
            var itemCount = GetGroundItemCount();
            if (itemCount <= 0)
            {
                _localModel.CloseMenu();
                return;
            }

            _localModel.ClampLootCursor(itemCount);
        }
    }

    private void SyncEncounter()
    {
        if (!CanOpenEncounter())
        {
            return;
        }

        lock (_localGameModel.SyncRoot)
        {
            var encounter = _localGameModel.FindEncounterForPlayer(_localModel.LocalPlayerId);
            if (encounter == null ||
                encounter.Sequence <= _localModel.LastHandledEncounterSequence)
            {
                return;
            }

            if (!_localGameModel.CanPlayerAttackEnemy(_localModel.LocalPlayerId, encounter.EnemyId))
            {
                _localModel.MarkEncounterHandled(encounter.Sequence);
                return;
            }

            _localModel.BeginAttackSelectionFromEncounter(encounter.Sequence, encounter.EnemyId);
        }
    }

    private void SyncCombatSelection()
    {
        var targetId = _localModel.PendingAttackTargetId;
        if (!targetId.HasValue)
        {
            return;
        }

        if (!_localGameModel.HasEncounterWithEnemy(_localModel.LocalPlayerId, targetId.Value) ||
            !_localGameModel.CanPlayerAttackEnemy(_localModel.LocalPlayerId, targetId.Value))
        {
            _localModel.EndAttackSelection();
        }
    }

    private bool CanOpenEncounter()
    {
        var mode = _localModel.CurrentInputMode;
        return _localModel.PendingAttackTargetId == null &&
            !mode.ShowsInstructions &&
            !mode.ShowsInventory &&
            !mode.ShowsLoot &&
            !mode.ShowsLogHistory &&
            !mode.ShowsGameOver;
    }

    private void SyncGameOver()
    {
        lock (_localGameModel.SyncRoot)
        {
            var player = _localGameModel.FindPlayer(_localModel.LocalPlayerId);
            if (player == null || player.IsAlive || _localModel.CurrentInputMode.ShowsGameOver)
            {
                return;
            }

            _localModel.OpenGameOver();
        }
    }

    private int GetInventoryItemCount()
    {
        lock (_localGameModel.SyncRoot)
        {
            return _localGameModel.FindPlayer(_localModel.LocalPlayerId)?.Inventory.Count ?? 0;
        }
    }

    private int GetCurrentSlotCount()
    {
        lock (_localGameModel.SyncRoot)
        {
            var player = _localGameModel.FindPlayer(_localModel.LocalPlayerId);
            var item = SlotBrowserNavigator.GetCurrentItem(player, _localModel);
            return item == null ? -1 : item.Slots.Count;
        }
    }

    private int GetGroundItemCount()
    {
        lock (_localGameModel.SyncRoot)
        {
            var player = _localGameModel.FindPlayer(_localModel.LocalPlayerId);
            if (player == null)
            {
                return 0;
            }

            return _localGameModel.Board.GetTile(player.X, player.Y).Items.Count;
        }
    }
}
