using System.Security.AccessControl;
using RpgGame.Characters;
using RpgGame.Engine;
using RpgGame.Engine.Builders;
using RpgGame.Engine.States;

// TO DO
// kolorki dodac
// poprawic estetyke instrukcji
// layout
// poprawic labirynt zeby dol nie byl rozjechany
// logike dodawania itemkow (zeby byla gwarancja ze niektore blisko startu)
// popraw tez np nazwa pieniedzy
class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        // kierownik budowy:)
        var director = new DungeonDirector();

        // budowa plansza
        var mapBuilder = new MapBuilder();
        director.SetBuilder(mapBuilder);
        //director.ConstructTestItemkow();
        director.ConstructMinotaursLabyrinth(); 
        var finalBoard = mapBuilder.BuildResult();

        // budowa instrukcji
        var instructionBuilder = new InstructionBuilder();
        director.SetBuilder(instructionBuilder);
        //director.ConstructTestItemkow();
        director.ConstructMinotaursLabyrinth();
        var instructions = instructionBuilder.BuildResult();

        Player player = new Player();
        GameEngine engine = new GameEngine(finalBoard, player);
        engine.PushState(new InstructionState(engine, instructions));

        engine.RunGame();
    }
}