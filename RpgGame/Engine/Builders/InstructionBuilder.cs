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

        public List<string> BuildResult() => _instructions;

        public void BuildEmptyDungeon()
        {
            _instructions.Add("Wkroczyłeś na bezkresne, krwawe piaski areny Aresa.");
            _instructions.Add("Przestrzeń jest całkowicie otwarta, sprzyjając walce.");
            _instructions.Add(""); // Pusta linia dla zachowania odstępu
        }

        public void BuildFilledDungeon()
        {
            _instructions.Add("Stoisz przed litą skałą, w której zmyślny Dedal wykuł swój");
            _instructions.Add("mroczny Labirynt. Otaczają Cię grube, kamienne ściany.");
            _instructions.Add("");
        }

        public void AddCorridors()
        {
            _instructions.Add("Przed Tobą plątanina krętych korytarzy. Uważaj, bo w tym miejscu");
            _instructions.Add("niezwykle łatwo zgubić drogę, jeśli nie posiadasz nici Ariadny.");
            _instructions.Add("");
        }

        public void AddRooms()
        {
            _instructions.Add("W głębi skały wyryto ukryte kryjówki – niegdyś służyły za ");
            _instructions.Add("bezpieczne skrytki lub schronienie dla zagubionych herosów.");
            _instructions.Add("");
        }

        public void AddCentralHall(int width, int height)
        {
            _instructions.Add($"Gdzieś w samym sercu tego przeklętego miejsca znajduje się");
            _instructions.Add($"potężna Komnata. To tam czai się największe niebezpieczeństwo.");
            _instructions.Add("");
        }

        public void AddItems(int count)
        {
            _instructions.Add($"Bogowie i dawni królowie ukryli tu swoje skarby. W mroku");
            _instructions.Add($"rozrzucono dokładnie {count} starożytnych artefaktów");
            _instructions.Add("Podejdź do nich i wciśnij [E], aby je podnieść.");
            _instructions.Add("");
        }

        public void AddWeapons(int count)
        {
            _instructions.Add($"W pyle leży {count} sztuk oręża poległych hoplitów.");
            _instructions.Add("Znajdziesz tu mityczne bronie, takie jak Xiphos czy Labrys.");
            _instructions.Add("Użyj klawisza [E], by dobyć broni z ziemi.");
            _instructions.Add("Zarządzaj nią w swoim ekwipunku naciskając [I].");
            _instructions.Add("");
        }
        public void AddMoney(int count)
        {

        }

    }
}