using RpgGame.Model.Global;

namespace RpgGame.Model.Game.Observers;

public interface ISpeciesSubscriber
{
    void UpdateOnDeath(GameEventLog eventLog);
}
