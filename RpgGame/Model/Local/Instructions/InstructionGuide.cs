namespace RpgGame.Model.Local.Instructions;

public sealed class InstructionGuide
{
    public InstructionGuide(IReadOnlyList<string> title, IReadOnlyList<string> lines)
    {
        Title = title;
        Lines = lines;
    }

    public IReadOnlyList<string> Title { get; }
    public IReadOnlyList<string> Lines { get; }
}
