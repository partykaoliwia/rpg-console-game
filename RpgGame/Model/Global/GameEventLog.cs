namespace RpgGame.Model.Global;

public sealed class GameEventLog
{
    private readonly List<string> _events = new();

    public int Count => _events.Count;

    public void Add(string message)
    {
        _events.Add(message);
    }

    public IReadOnlyList<string> GetAll()
    {
        return _events.ToList();
    }

    public IReadOnlyList<string> GetRecent(int count)
    {
        return _events.Skip(Math.Max(0, _events.Count - count)).ToList();
    }

    public IReadOnlyList<string> GetFrom(int startIndex)
    {
        return _events.Skip(Math.Clamp(startIndex, 0, _events.Count)).ToList();
    }

    public void ReplaceWith(IEnumerable<string> events)
    {
        _events.Clear();
        _events.AddRange(events);
    }
}
