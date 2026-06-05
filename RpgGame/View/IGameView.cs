using RpgGame.Model.Game;
using RpgGame.Model.Local;

namespace RpgGame.View;

public interface IGameView
{
    void Render(GameModel gameModel, LocalModel localModel);
}
