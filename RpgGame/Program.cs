using RpgGame.Characters;
using RpgGame.Engine;
using RpgGame.Engine.Builders;
using RpgGame.Engine.States;

class Program
{
    static void Main(string[] args)
    {
        // 1. Inicjalizacja Kierownika budowy (Director)
        var director = new DungeonDirector();

        // 2. Budujemy planszę
        var mapBuilder = new MapBuilder();
        director.SetBuilder(mapBuilder);
        director.ConstructMinotaursLabyrinth(); // Odpalamy mityczną strategię
        var finalBoard = mapBuilder.BuildResult();

        // 3. Budujemy spójną instrukcję na podstawie TEJ SAMEJ strategii!
        var instructionBuilder = new InstructionBuilder();
        director.SetBuilder(instructionBuilder);
        director.ConstructMinotaursLabyrinth();
        var instructions = instructionBuilder.BuildResult();

        // 4. Konfigurujemy obiekty gry
        Player player = new Player();
        // Konstruktor GameEngine od razu wrzuca PlayingState na dno stosu
        GameEngine engine = new GameEngine(finalBoard, player);

        // 5. Wrzucamy InstructionState na SAM WIERZCH
        engine.PushState(new InstructionState(engine, instructions));

        // 6. Odpalamy pętlę!
        engine.RunGame();
    }
}