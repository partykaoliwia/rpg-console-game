using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RpgGame.Characters;
using RpgGame.Engine.Commands;
using RpgGame.Engine.Input;
using RpgGame.Gameboard;
using RpgGame.Items;

namespace RpgGame.Engine.States
{
    public class LootingState : IGameState, IMenuState
    {
        private readonly GameEngine _engine;
        private readonly Player _player;
        private readonly Board _board;
        private string _errorMessage = string.Empty;
        private readonly InputHandler _inputChainHead;

        private ITile CurrentTile => _board.GetTile(_player.X, _player.Y);
        private List<IItem> CurrentTileItems => CurrentTile.Items;
        private int CurrentTileItemsCount => CurrentTileItems.Count;

        public int CursorIndex { get; set; } = 0;
        public int ItemCount => CurrentTile.Items.Count;

        public LootingState(GameEngine engine, Player player, Board board)
        {
            _engine = engine;
            _player = player;
            _board = board;

            string s = "Looting";
            // Przesuwanie kursora
            var upKey = new KeyCommandHandler(s,"MoveUp", new MoveCursorCommand(this, -1));
            var downKey = new KeyCommandHandler(s,"MoveDown",new MoveCursorCommand(this, 1));

            // Podnoszenie przedmiotu
            var collectKey = new KeyCommandHandler(s,"Collect", new LootItemCommand(player, board, this));

            // Wyjście z menu
            var quitKey = new KeyCommandHandler(s,"Quit", new PopStateCommand(engine));

            var reloadKey = new KeyCommandHandler(s, "Reload", new ReloadConfigCommand());


            // Złożenie łańcucha
            upKey.SetNext(downKey).SetNext(collectKey).SetNext(quitKey).SetNext(reloadKey);

            _inputChainHead = upKey;
        }        

        public void HandleInput(ConsoleKey key)
        {
            _inputChainHead.Handle(key, out _errorMessage);

            if (ItemCount == 0)
            {
                _engine.PopState();
            }
        }
        public void Draw(ConsoleRenderer renderer)
        {
            renderer.DrawBoard(_board, _player);
            renderer.DrawLootingMenu(_player, CurrentTile, CursorIndex, _errorMessage);
        }
    }
}
