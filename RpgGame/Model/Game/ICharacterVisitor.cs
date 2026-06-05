namespace RpgGame.Model.Game;

public interface ICharacterVisitor
{
    void Visit(Player player);
    void Visit(Harpy harpy);
    void Visit(Giant giant);
    void Visit(Satyr satyr);
}
