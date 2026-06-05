using RpgGame.Model.Global;

namespace RpgGame.Model.Game.Observers;

public interface ISoundSubscriber
{
    int X { get; }
    int Y { get; }

    void UpdateOnSound(int sourceX, int sourceY, int distance, GameEventLog eventLog);
}
