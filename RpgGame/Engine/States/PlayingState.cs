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
            string s = "Playing";

            _inputChainHead = new InputChainBuilder().AddKey(s, "MoveUp", new MoveCommand(engine, player, board, 0, -1))
                                                      .AddKey(s, "MoveDown", new MoveCommand(engine, player, board, 0, 1))
                                                      .AddKey(s,"MoveLeft",new MoveCommand(engine, player, board, -1, 0))
                                                      .AddKey(s,"MoveRight",new MoveCommand(engine,player, board, 1, 0))
                                                      .AddKey(s,"Collect", new CollectCommand(engine, player, board))
                                                      .AddKey(s,"OpenInventory",new PushStateCommand(engine, new InventoryState(engine, player, board)))
                                                      .AddKey(s,"Quit",new QuitCommand(engine))
                                                      .AddKey(s,"Reload", new ReloadConfigCommand())
                                                      .Build(); 
        }

        public void HandleInput(ConsoleKey key)
        {
            _inputChainHead.Handle(key, out _errorMessage);
        }
       public void Draw(ConsoleRenderer renderer)
        {
            renderer.DrawBoard(_board, _player);
            renderer.DrawPlayingView(_board, _player, _errorMessage); 
        }
    }
}
