using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpgGame.Engine.Observers
{
    public class SpeciesPublisher
    {
        public string Name { get; }
        private readonly HashSet<ISpeciesSubscriber> _subscribers = new HashSet<ISpeciesSubscriber>();
        public SpeciesPublisher(string name)
        {
            Name = name;
        }
        public void Subscribe(ISpeciesSubscriber subscriber)
        {
            _subscribers.Add(subscriber);
        }
        public void Unsubscribe(ISpeciesSubscriber subscriber)
        {
            _subscribers.Remove(subscriber);
        }
        public void NotifySubscribers()
        {
            foreach(var enemy in _subscribers.ToList())
            {
                enemy.UpdateOnDeath();
            }
        }


    }
}
