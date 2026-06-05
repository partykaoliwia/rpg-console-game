using RpgGame.Model.Game.Items;

namespace RpgGame.View.Visitors;

public sealed class ItemVisitor : IItemVisitor
{
    public RenderIcon Icon { get; private set; } = new("?", ConsoleColor.DarkGray);

    public void Visit(Gold gold)
    {
        Icon = new RenderIcon("¤", ConsoleColor.Yellow);
    }

    public void Visit(Coin coin)
    {
        Icon = new RenderIcon("©", ConsoleColor.Yellow);
    }

    public void Visit(Kopis kopis)
    {
        Icon = new RenderIcon("P", ConsoleColor.Green);
    }

    public void Visit(Xiphos xiphos)
    {
        Icon = new RenderIcon("†", ConsoleColor.Green);
    }

    public void Visit(Labrys labrys)
    {
        Icon = new RenderIcon("T", ConsoleColor.Green);
    }

    public void Visit(StymphalianQuill stymphalianQuill)
    {
        Icon = new RenderIcon("»", ConsoleColor.Green);
    }

    public void Visit(Dory dory)
    {
        Icon = new RenderIcon("/", ConsoleColor.Green);
    }

    public void Visit(OldBook oldBook)
    {
        Icon = new RenderIcon("#", ConsoleColor.DarkYellow);
    }

    public void Visit(AriadnesThread ariadnesThread)
    {
        Icon = new RenderIcon("&", ConsoleColor.Blue);
    }

    public void Visit(CyclopsEye cyclopsEye)
    {
        Icon = new RenderIcon("•", ConsoleColor.Cyan);
    }

    public void Visit(IcarusFeather icarusFeather)
    {
        Icon = new RenderIcon("~", ConsoleColor.Cyan);
    }

    public void Visit(ShatteredShield shatteredShield)
    {
        Icon = new RenderIcon("0", ConsoleColor.DarkGray);
    }

    public void Visit(SpartanModifier modifier)
    {
        modifier.InnerItem.Accept(this);
    }

    public void Visit(AthenianModifier modifier)
    {
        modifier.InnerItem.Accept(this);
    }

    public void Visit(HephaesteanModifier modifier)
    {
        modifier.InnerItem.Accept(this);
    }

    public void Visit(MidasTouchModifier modifier)
    {
        modifier.InnerItem.Accept(this);
    }

    public void Visit(PrometheanModifier modifier)
    {
        modifier.InnerItem.Accept(this);
    }

    public void Visit(AchillesBaneModifier modifier)
    {
        modifier.InnerItem.Accept(this);
    }

    public void Visit(AphroditesKissModifier modifier)
    {
        modifier.InnerItem.Accept(this);
    }

    public void Visit(DionysianModifier modifier)
    {
        modifier.InnerItem.Accept(this);
    }

    public void Visit(HydrasVenomModifier modifier)
    {
        modifier.InnerItem.Accept(this);
    }

    public void Visit(IcarianModifier modifier)
    {
        modifier.InnerItem.Accept(this);
    }
}
