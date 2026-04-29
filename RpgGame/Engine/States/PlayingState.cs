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
        private readonly Random _random = new Random();
        public PlayingState(GameEngine engine, Board board, Player player)
        {
            _engine = engine;
            _board = board;
            _player = player;
            string s = "Playing";

            _inputChainHead = new InputChainBuilder().AddKey(s, "MoveUp", new MoveCommand(engine, player, board, 0, -1),engine.Keybinds)
                                                      .AddKey(s, "MoveDown", new MoveCommand(engine, player, board, 0, 1),engine.Keybinds)
                                                      .AddKey(s,"MoveLeft",new MoveCommand(engine, player, board, -1, 0),engine.Keybinds)
                                                      .AddKey(s,"MoveRight",new MoveCommand(engine,player, board, 1, 0),engine.Keybinds)
                                                      .AddKey(s,"Collect", new CollectCommand(engine, player, board),engine.Keybinds)
                                                      .AddKey(s,"OpenInventory",new PushStateCommand(engine, new InventoryState(engine, player, board)),engine.Keybinds)
                                                      .AddKey(s,"Quit",new QuitCommand(engine),engine.Keybinds)
                                                      .AddKey(s,"Reload", new ReloadConfigCommand(engine.Keybinds),engine.Keybinds)
                                                      .AddKey(s,"LogHistory", new PushStateCommand(engine, new LogHistoryState(engine)),engine.Keybinds)
                                                      .Build(); 
        }
        public void Update()
        {
            MoveEnemies();
        }
        private void MoveEnemies()
        {
            var alreadyUpdated = new HashSet<IAttackable>();
            for(int y=0; y<_board.Height; y++)
            {
                for(int x=0; x<_board.Width; x++)
                {
                    var tile = _board.GetTile(x, y);
                    var enemy = tile.Entity;
                    if(enemy != null && !alreadyUpdated.Contains(enemy))
                    {
                        alreadyUpdated.Add(enemy);
                        if((DateTime.Now - enemy.LastMoved).TotalMilliseconds > enemy.MoveEveryMs)
                        {
                            TryMoveEnemy(x, y, enemy);
                            enemy.LastMoved = DateTime.Now;
                        }
                    }
                }
            }
        }
        private bool TryMoveEnemy(int x, int y, IAttackable enemy)
        {
            int nx = x;
            int ny = y;
            switch(_random.Next(0,5))
            {
                case 0: ny--; break;
                case 1: ny++; break;
                case 2: nx--; break;
                case 3: nx++; break;
                case 4: return false;
            }
            var targetTile = _board.GetTile(nx, ny);
            if(nx == _player.X && ny == _player.Y)
            {
                _engine.PushState(new CombatState(_engine, _player, enemy, _board.GetTile(x, y)));
                return true;
            }
            if(targetTile.IsWalkable && targetTile.Entity == null)
            {
                targetTile.Entity = enemy;
                _board.GetTile(x, y).Entity = null;
                enemy.SetPosition(nx, ny);
                return true;
            }
            return false;
        }

        public void HandleInput(ConsoleKey key)
        {
            _inputChainHead.Handle(key);
        }
       public void Draw(ConsoleRenderer renderer)
        {
            renderer.DrawBoard(_board, _player);
            renderer.DrawPlayingView(_board, _player); 
        }
    }
}
