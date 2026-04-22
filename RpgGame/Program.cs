using System.Security.AccessControl;
using RpgGame.Characters;
using RpgGame.Engine;
using RpgGame.Engine.Builders;
using RpgGame.Engine.Configuration;
using RpgGame.Engine.Input;
using RpgGame.Engine.Logging;
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
        var config = ConfigLoader.LoadConfig("config.json");
        if(!Directory.Exists(config.LogDirectory))
        {
            Directory.CreateDirectory(config.LogDirectory);
        }
        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string logFilePath = Path.Combine(config.LogDirectory, $"{config.PlayerName}_{timestamp}.txt");

        Logger.Instance.AddWriter(new FileLogWriter(logFilePath));
        Logger.Instance.SetMemoryWriter(new MemoryLogWriter());

        var keybinds = new KeybindManager();


        IFactory factory;
        Random rnd = new Random();
        int themeChoice = rnd.Next(0, 3);

        //if (themeChoice == 0) factory = new LibraryFactory();
        //else if (themeChoice == 1) factory = new ArenaFactory();
        //else factory = new LabyrinthFactory();
        factory = new ArenaFactory();
        // kierownik budowy:)
        var director = new DungeonDirector();
        director.SetStrategy(factory.GenerationStrategy);

        // budowa plansza
        var mapBuilder = new MapBuilder(factory);
        director.SetBuilder(mapBuilder);
        director.ConstructDungeon();
        var finalBoard = mapBuilder.BuildResult();

        // budowa instrukcji
        var instructionBuilder = new InstructionBuilder(keybinds,factory);
        director.SetBuilder(instructionBuilder);
        director.ConstructDungeon();
        var instructions = instructionBuilder.BuildResult();

        Player player = new Player();
        GameEngine engine = new GameEngine(finalBoard, player, keybinds, logFilePath);
        engine.PushState(new InstructionState(engine, instructions,factory.Title));

        engine.RunGame();
    }
}