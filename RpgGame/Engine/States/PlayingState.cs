using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RpgGame.Characters;
using RpgGame.Engine.Commands;
using RpgGame.Engine.Input;
using RpgGame.Gameboard;

namespace RpgGame.Engine.States
{
    public class PlayingState : IGameState
    {
        private readonly GameEngine _engine;
        private readonly Board _board;
        private readonly Player _player;
        private readonly InputHandler _inputChainHead;
        private string _errorMessage = string.Empty;
        public PlayingState(GameEngine engine, Board board, Player player)
        {
            _engine = engine;
            _board = board;
            _player = player;

            // Tworzymy poszczególne "ogniwa" łańcucha
            var wKey = new KeyCommandHandler(ConsoleKey.W, new MoveCommand(player, board, 0, -1));
            var sKey = new KeyCommandHandler(ConsoleKey.S, new MoveCommand(player, board, 0, 1));
            var aKey = new KeyCommandHandler(ConsoleKey.A, new MoveCommand(player, board, -1, 0));
            var dKey = new KeyCommandHandler(ConsoleKey.D, new MoveCommand(player, board, 1, 0));

            var eKey = new KeyCommandHandler(ConsoleKey.E, new CollectCommand(engine, player, board));
            var iKey = new KeyCommandHandler(ConsoleKey.I, new PushStateCommand(engine, new InventoryState(engine, player, board)));
            var escKey = new KeyCommandHandler(ConsoleKey.Escape, new QuitCommand(engine));

            // Łączymy je w jeden łańcuch
            wKey.SetNext(sKey)
                .SetNext(aKey)
                .SetNext(dKey)
                .SetNext(eKey)
                .SetNext(iKey)
                .SetNext(escKey);

            // Zapisujemy referencję do początku łańcucha
            _inputChainHead = wKey;
        }

        public void HandleInput(ConsoleKey key)
        {
            // Przekazujemy żądanie wzdłuż łańcucha
            _inputChainHead.Handle(key, out _errorMessage);
        }
       public void Draw(ConsoleRenderer renderer)
        {
            renderer.DrawBoard(_board, _player);
            // Przekazujemy pusty błąd lub rzeczywisty komunikat. 
            // DrawPlayingView zajmie się narysowaniem wszystkiego!
            renderer.DrawPlayingView(_board, _player, _errorMessage); 
        }
    }
}
