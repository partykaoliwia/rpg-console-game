using RpgGame.Engine.States;

namespace RpgGame.Engine.Commands
{
    public class MoveCursorCommand : ICommand
    {
        private readonly IMenuState _menu;
        private readonly int _direction;

        public MoveCursorCommand(IMenuState menu, int direction)
        {
            _menu = menu;
            _direction = direction; // -1 w górę, 1 w dół
        }

        public void Execute()
        {
            if (_menu.ItemCount == 0) return;

            _menu.CursorIndex += _direction;

            if (_menu.CursorIndex < 0)
            {
                _menu.CursorIndex = _menu.ItemCount - 1;
            }
            else if (_menu.CursorIndex >= _menu.ItemCount)
            {
                _menu.CursorIndex = 0;
            }
        }
    }
}