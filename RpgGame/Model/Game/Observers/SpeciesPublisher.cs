using RpgGame.Model.Global;

namespace RpgGame.Model.Game.Observers;

public sealed class SpeciesPublisher
{
    private readonly HashSet<ISpeciesSubscriber> _subscribers = new();

    public SpeciesPublisher(string name)
    {
        Name = name;
    }

    public string Name { get; }

    public void Subscribe(ISpeciesSubscriber subscriber)
    {
        _subscribers.Add(subscriber);
    }

    public void Unsubscribe(ISpeciesSubscriber subscriber)
    {
        _subscribers.Remove(subscriber);
    }

    public void NotifySubscribers(GameEventLog eventLog)
    {
        foreach (var subscriber in _subscribers.ToList())
        {
            subscriber.UpdateOnDeath(eventLog);
        }
    }
}
