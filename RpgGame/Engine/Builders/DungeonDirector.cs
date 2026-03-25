using System;

namespace RpgGame.Engine.Builders
{
    public class DungeonDirector
    {
        private IDungeonBuilder _builder;

        // Metoda pozwalająca wstrzyknąć dowolnego budowniczego (Mapę lub Instrukcję)
        public void SetBuilder(IDungeonBuilder builder)
        {
            _builder = builder;
        }

        // --- STRATEGIA 1: Prawdziwy Labirynt (Główny poziom) ---
        // Pełne doświadczenie: ściany, komnaty, Minotaur, łupy i broń.
        public void ConstructMinotaursLabyrinth()
        {
            // Zawsze zaczynamy od bloku początkowego
            _builder.BuildFilledDungeon();
            // Następnie dowolna kombinacja metod
            _builder.AddCentralHall(9, 6); // Tu może czekać Boss
            _builder.AddRooms();
            _builder.AddCorridors();
            _builder.AddItems(10); // Kłębki nici, pióra Ikara itp.
            _builder.AddWeapons(6); // Xiphosy i Labrysy
        }

        // --- STRATEGIA 2: Arena Walki (Tryb Przetrwania / Trening) ---
        // Otwarta przestrzeń, tylko broń, zero przedmiotów fabularnych.
        public void ConstructAresArena()
        {
            // Zaczynamy od pustego lochu
            _builder.BuildEmptyDungeon();

            _builder.AddWeapons(10);
            // Celowo nie wywołujemy AddItems ani AddCorridors.
            // Instrukcja w tym trybie po prostu nie wspomni o szukaniu ukrytego lootu!
        }

        // --- STRATEGIA 3: Skarbiec Dedala (Poziom bonusowy) ---
        // Dużo korytarzy i komnat, brak potworów i broni, samo bogactwo.
        public void ConstructDaedalusTreasury()
        {
            _builder.BuildFilledDungeon();

            _builder.AddCorridors();
            _builder.AddRooms();
            _builder.AddItems(25); // Mnóstwo przedmiotów do zebrania
            // Brak wywołania AddWeapons().
        }
    }
}