using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpgGame.Engine.Builders
{
    public class InstructionBuilder : IDungeonBuilder
    {
        private List<string> _instructions = new List<string>();

        // Zwraca gotową listę instrukcji dla ekranu początkowego
        public List<string> BuildResult() => _instructions;

        public void BuildEmptyDungeon()
        {
            // Idealne pod: ConstructAresArena()
            _instructions.Add("Wkroczyłeś na bezkresne, krwawe piaski areny Aresa. Przestrzeń jest całkowicie otwarta, sprzyjając walce – nie ma tu gdzie się ukryć, a każdy krok zależy tylko od Ciebie.");
        }

        public void BuildFilledDungeon()
        {
            // Idealne pod: ConstructMinotaursLabyrinth() oraz ConstructDaedalusTreasury()
            _instructions.Add("Stoisz przed litą skałą, w której zmyślny Dedal wykuł swój mroczny Labirynt. Otaczają Cię grube, nieprzeniknione, kamienne ściany.");
        }

        public void AddCorridors()
        {
            _instructions.Add("Przed Tobą plątanina mrocznych, krętych korytarzy. Uważaj, w tym miejscu niezwykle łatwo zgubić drogę, jeśli nie posiadasz nici Ariadny.");
        }

        public void AddRooms()
        {
            _instructions.Add("W głębi skały wyryto ukryte komnaty – niegdyś służyły za bezpieczne skrytki lub schronienie dla zagubionych herosów.");
        }

        public void AddCentralHall(int width, int height)
        {
            _instructions.Add($"Gdzieś w samym sercu tego przeklętego miejsca znajduje się potężna Komnata Sądu (o wymiarach {width}x{height}). To tam zwykle czai się największe niebezpieczeństwo.");
        }

        public void AddItems(int count)
        {
            // Idealne pod skarbiec lub eksplorację
            _instructions.Add($"Bogowie i dawni królowie ukryli tu swoje skarby. W mroku rozrzucono dokładnie {count} starożytnych artefaktów (np. pęknięte amfory, skamieniałe oczy cyklopa).");
            _instructions.Add("Podejdź do nich i wciśnij klawisz [E], aby je podnieść i zgromadzić w swoim ekwipunku.");
        }

        public void AddWeapons(int count)
        {
            // Idealne pod arenę walki lub zbrojenie się w labiryncie
            _instructions.Add("Na piasku i w pyle leży rozrzucony oręż poległych hoplitów. Znajdziesz tu mityczne bronie, takie jak krótki miecz Xiphos czy potężny, dwuręczny Labrys.");
            _instructions.Add("Użyj klawisza [E], by dobyć broni z ziemi, a następnie zarządzaj nią w swoim ekwipunku naciskając [I].");
        }
    }
}