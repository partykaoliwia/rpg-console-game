using RpgGame.Model.Local.InputModes;
using RpgGame.Model.Local.Instructions;
using RpgGame.Model.Local.KeyBindings;

namespace RpgGame.Model.Local;

public sealed class LocalModel
{
    private static readonly TimeSpan FreshLocalMessageAge = TimeSpan.FromSeconds(2);
    private readonly Stack<IInputMode> _inputModes = new();
    private readonly List<LocalMessage> _localMessages = new();
    private int? _attackSelectionBaseDepth;

    public LocalModel(int localPlayerId, KeyBindingProfile keyBindings)
    {
        LocalPlayerId = localPlayerId;
        KeyBindings = keyBindings;
        _inputModes.Push(new PlayingInputMode());
    }

    public int LocalPlayerId { get; set; }
    public KeyBindingProfile KeyBindings { get; private set; }
    public InstructionGuide InstructionGuide { get; private set; } =
        new(Array.Empty<string>(), Array.Empty<string>());

    public IInputMode CurrentInputMode => _inputModes.Peek();
    public string LogFilePath { get; private set; } = string.Empty;
    public int? PendingAttackTargetId { get; private set; }
    public bool IsChoosingAttackWeapon { get; private set; }
    public int LastHandledEncounterSequence { get; private set; }
    public int InventoryCursor { get; set; }
    public int LootCursor { get; set; }

    public void AddLocalMessage(string message)
    {
        _localMessages.Add(new LocalMessage(message, DateTime.UtcNow));
    }

    public IReadOnlyList<string> GetAllLocalMessages()
    {
        return _localMessages.Select(message => message.Text).ToList();
    }

    public void ReplaceLogFilePath(string logFilePath)
    {
        LogFilePath = logFilePath;
    }

    public string? GetFreshLocalMessage()
    {
        var latest = _localMessages.LastOrDefault();
        if (latest == null || DateTime.UtcNow - latest.CreatedUtc > FreshLocalMessageAge)
        {
            return null;
        }

        return latest.Text;
    }

    public void ChangeInputMode(IInputMode inputMode)
    {
        ReplaceCurrentInputMode(inputMode);
    }

    public void ReplaceKeyBindings(KeyBindingProfile keyBindings)
    {
        KeyBindings = keyBindings;
    }

    public void ReplaceInstructionGuide(InstructionGuide instructionGuide)
    {
        InstructionGuide = instructionGuide;
    }

    public void OpenInstructions(InstructionGuide instructionGuide)
    {
        InstructionGuide = instructionGuide;
        PushInputMode(new InstructionInputMode());
    }

    public void CloseInstructions()
    {
        PopInputMode();
    }

    public void OpenInventory()
    {
        InventoryCursor = 0;
        PushInputMode(new InventoryInputMode());
    }

    public void OpenLoot()
    {
        LootCursor = 0;
        PushInputMode(new LootInputMode());
    }

    public void OpenLogHistory()
    {
        PushInputMode(new LogHistoryInputMode());
    }

    public void OpenGameOver()
    {
        PendingAttackTargetId = null;
        IsChoosingAttackWeapon = false;
        _attackSelectionBaseDepth = null;
        PushInputMode(new GameOverInputMode());
    }

    public void CloseMenu()
    {
        PopInputMode();
    }

    public void BeginAttackSelection(int targetId)
    {
        _attackSelectionBaseDepth = _inputModes.Count;
        PendingAttackTargetId = targetId;
        IsChoosingAttackWeapon = false;
        PushInputMode(new CombatInputMode());
    }

    public void BeginAttackSelectionFromEncounter(int encounterSequence, int targetId)
    {
        MarkEncounterHandled(encounterSequence);
        BeginAttackSelection(targetId);
    }

    public void MarkEncounterHandled(int encounterSequence)
    {
        LastHandledEncounterSequence = Math.Max(LastHandledEncounterSequence, encounterSequence);
    }

    public void BeginWeaponSelection()
    {
        IsChoosingAttackWeapon = true;
        PushInputMode(new WeaponInputMode());
    }

    public void ReturnToAttackSelection()
    {
        if (PendingAttackTargetId == null)
        {
            return;
        }

        IsChoosingAttackWeapon = false;
        PopToDepth((_attackSelectionBaseDepth ?? 1) + 1);
    }

    public void EndAttackSelection()
    {
        PendingAttackTargetId = null;
        IsChoosingAttackWeapon = false;
        PopToDepth(_attackSelectionBaseDepth ?? 1);
        _attackSelectionBaseDepth = null;
    }

    public void MoveInventoryCursor(int delta, int itemCount)
    {
        if (itemCount <= 0)
        {
            InventoryCursor = 0;
            return;
        }

        InventoryCursor = MoveCursor(InventoryCursor, delta, itemCount);
    }

    public void ClampInventoryCursor(int itemCount)
    {
        if (itemCount <= 0)
        {
            InventoryCursor = 0;
            return;
        }

        InventoryCursor = Math.Clamp(InventoryCursor, 0, itemCount - 1);
    }

    public void MoveLootCursor(int delta, int itemCount)
    {
        if (itemCount <= 0)
        {
            LootCursor = 0;
            return;
        }

        LootCursor = MoveCursor(LootCursor, delta, itemCount);
    }

    public void ClampLootCursor(int itemCount)
    {
        if (itemCount <= 0)
        {
            LootCursor = 0;
            return;
        }

        LootCursor = Math.Clamp(LootCursor, 0, itemCount - 1);
    }

    private void PushInputMode(IInputMode inputMode)
    {
        _inputModes.Push(inputMode);
    }

    private void PopInputMode()
    {
        if (_inputModes.Count > 1)
        {
            _inputModes.Pop();
        }
    }

    private void PopToDepth(int depth)
    {
        while (_inputModes.Count > Math.Max(1, depth))
        {
            _inputModes.Pop();
        }
    }

    private void ReplaceCurrentInputMode(IInputMode inputMode)
    {
        if (_inputModes.Count > 0)
        {
            _inputModes.Pop();
        }

        _inputModes.Push(inputMode);
    }

    private static int MoveCursor(int cursor, int delta, int itemCount)
    {
        var next = (cursor + delta) % itemCount;
        return next < 0 ? itemCount - 1 : next;
    }

    private sealed class LocalMessage
    {
        public LocalMessage(string text, DateTime createdUtc)
        {
            Text = text;
            CreatedUtc = createdUtc;
        }

        public string Text { get; }
        public DateTime CreatedUtc { get; }
    }
}
