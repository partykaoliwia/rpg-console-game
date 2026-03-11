using RpgGame.Engine;
using RpgGame.Gameboard;
using RpgGame.Items;
using RpgGame.Player;

namespace RpgGame
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Board board = new Board();
            var player = new Player.Player(0, 0);
            board.GetTile(3, 3).Items.Add(new Coin(10));
            board.GetTile(5, 5).Items.Add(new Coin(15));
            board.GetTile(6, 5).Items.Add(new Sword());
            board.GetTile(7, 5).Items.Add(new Dagger());
            board.GetTile(8, 5).Items.Add(new Axe());
            GameEngine engine = new GameEngine(board, player);
            engine.RunGame();
        }
    }
}
