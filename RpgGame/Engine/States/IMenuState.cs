namespace RpgGame.Engine.States
{
    public interface IMenuState
    {
        int CursorIndex { get; set; }
        int ItemCount { get; }
    }
}