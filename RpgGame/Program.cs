using System.Security.AccessControl;
using RpgGame.Characters;
using RpgGame.Engine;
using RpgGame.Engine.Builders;
using RpgGame.Engine.Input;
using RpgGame.Engine.States;

// TO DO
// poprawic estetyke combatstate
// poprawic labirynt zeby dol nie byl rozjechany
// dodac lepsza obsluge logow
// w main poprawic
class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        var keybinds = new KeybindManager();
        IFactory factory = new StandardFactory();

        // kierownik budowy:)
        var director = new DungeonDirector();

        // budowa plansza
        var mapBuilder = new MapBuilder(factory);
        director.SetBuilder(mapBuilder);
        //director.ConstructTestItemkow();
        director.ConstructMinotaursLabyrinth(); 
        var finalBoard = mapBuilder.BuildResult();

        // budowa instrukcji
        var instructionBuilder = new InstructionBuilder(keybinds);
        director.SetBuilder(instructionBuilder);
        //director.ConstructTestItemkow();
        director.ConstructMinotaursLabyrinth();
        var instructions = instructionBuilder.BuildResult();

        Player player = new Player();
        GameEngine engine = new GameEngine(finalBoard, player, keybinds);
        engine.PushState(new InstructionState(engine, instructions));

        engine.RunGame();
    }
}