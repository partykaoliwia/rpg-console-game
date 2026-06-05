namespace RpgGame.Model.Game;

public interface ITileVisitor
{
    void Visit(FloorTile floor);
    void Visit(WallTile wall);
}
