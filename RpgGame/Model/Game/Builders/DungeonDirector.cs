namespace RpgGame.Model.Game.Builders;

public sealed class DungeonDirector
{
    private IDungeonBuilder? _builder;
    private IGenerationStrategy? _strategy;

    public void SetBuilder(IDungeonBuilder builder)
    {
        _builder = builder;
    }

    public void SetStrategy(IGenerationStrategy strategy)
    {
        _strategy = strategy;
    }

    public void ConstructDungeon()
    {
        if (_builder is null || _strategy is null)
        {
            throw new InvalidOperationException("Dungeon builder and generation strategy must be configured before construction.");
        }

        _strategy.GenerateMap(_builder);
    }
}
