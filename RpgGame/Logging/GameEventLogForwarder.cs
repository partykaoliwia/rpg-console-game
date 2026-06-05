using RpgGame.Model.Game;

namespace RpgGame.Logging;

public sealed class GameEventLogForwarder
{
    private int _nextEventIndex;

    public void Flush(GameModel model)
    {
        IReadOnlyList<string> entries;

        lock (model.SyncRoot)
        {
            if (_nextEventIndex > model.EventLog.Count)
            {
                _nextEventIndex = 0;
            }

            entries = model.EventLog.GetFrom(_nextEventIndex);
            _nextEventIndex += entries.Count;
        }

        foreach (var entry in entries)
        {
            Logger.Instance.Log(entry);
        }
    }
}
