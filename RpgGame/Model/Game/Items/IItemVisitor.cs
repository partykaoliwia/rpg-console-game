namespace RpgGame.Model.Game.Items;

public interface IItemVisitor
{
    void Visit(Gold gold);
    void Visit(Coin coin);
    void Visit(Kopis kopis);
    void Visit(Xiphos xiphos);
    void Visit(Labrys labrys);
    void Visit(StymphalianQuill stymphalianQuill);
    void Visit(Dory dory);
    void Visit(OldBook oldBook);
    void Visit(AriadnesThread ariadnesThread);
    void Visit(CyclopsEye cyclopsEye);
    void Visit(IcarusFeather icarusFeather);
    void Visit(ShatteredShield shatteredShield);
    void Visit(SpartanModifier modifier);
    void Visit(AthenianModifier modifier);
    void Visit(HephaesteanModifier modifier);
    void Visit(MidasTouchModifier modifier);
    void Visit(PrometheanModifier modifier);
    void Visit(AchillesBaneModifier modifier);
    void Visit(AphroditesKissModifier modifier);
    void Visit(DionysianModifier modifier);
    void Visit(HydrasVenomModifier modifier);
    void Visit(IcarianModifier modifier);
}
