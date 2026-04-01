using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RpgGame.Engine.Input;

namespace RpgGame.Engine.Builders
{
    public class InstructionBuilder : IDungeonBuilder
    {
        private List<string> _instructions = new List<string>();

        public List<string> BuildResult() => _instructions;

        public void BuildEmptyDungeon()
        {
            _instructions.Add("Wkroczyłeś na otwartą arenę pełną piasku i krwi, gdzie toczy się bezlitosna walka.");
        }

        public void BuildFilledDungeon()
        {
            _instructions.Add("Stoisz przed wejściem do mrocznego labiryntu wykutego przez Dedala, pełnego ukrytych ścieżek i tajemnic.");
        }

        public void AddCorridors()
        {
            _instructions.Add("Przed tobą mnóstwo krętych i wąskich korytarzy, w których łatwo się zgubić.");
        }

        public void AddRooms()
        {
            _instructions.Add("Natknąłeś się na kilka ukrytych pomieszczeń wykutych w skale, które mogą kryć skarby lub niebezpieczeństwo.");
        }

        public void AddCentralHall(int width, int height)
        {
            _instructions.Add("W samym sercu tego przeklętego miejsca czai się potężna Komnata. To tam czeka największe niebezpieczeństwo.");
        }

        public void AddItems(int count)
        {
            string collect = KeybindManager.Instance.GetKey("Playing", "Collect").ToString();
            _instructions.Add($"Bogowie pozostawili tu {count} starożytnych artefaktów, które możesz podnieść naciskając [{collect}].");
        }

        public void AddWeapons(int count)
        {
            string collect = KeybindManager.Instance.GetKey("Playing", "Collect").ToString();
            string inv = KeybindManager.Instance.GetKey("Playing", "OpenInventory").ToString();
            _instructions.Add($"Poprzedni śmiałkowie pozostawili tu {count} sztuk potężnej broni - użyj [{collect}] by podnieść, a [{inv}] by zarządzać ekwipunkiem.");
        }

        public void AddMoney(int count)
        {
            _instructions.Add($"Rozrzucono tu {count} monet, zbieraj je, by wzmocnić swoją pozycję w świecie śmiertelników.");
        }

        public void AddEnemies(int count)
        {
            _instructions.Add($"W tym strasznym labiryncie czai się {count} potężnych wrogów oraz minotaur, zagrażających twojemu życiu.");
        }
    }
}