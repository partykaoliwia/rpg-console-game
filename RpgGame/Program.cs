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
            board.GetTile(5, 3).Items.Add(new Paper());
            board.GetTile(5, 3).Items.Add(new Stick());
            board.GetTile(8, 2).Items.Add(new Sword());
            board.GetTile(3, 2).Items.Add(new Sword());
            board.GetTile(8, 2).Items.Add(new Dagger());
            board.GetTile(8, 2).Items.Add(new Dagger());
            board.GetTile(9, 4).Items.Add(new Rock());
            board.GetTile(15, 3).Items.Add(new Gold(100));
            board.GetTile(15, 3).Items.Add(new Coin(50));
            board.GetTile(15, 3).Items.Add(new Axe());
            board.GetTile(20, 5).Items.Add(new Gold(25));

            GameEngine engine = new GameEngine(board, player);
            engine.RunGame();
        }
    }
}
