using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpgGame.Engine.Input
{
    public class KeybindManager
    {
        private static KeybindManager _instance;
        private readonly Dictionary<string, Dictionary<string, ConsoleKey>> _keybinds;
        private KeybindManager()
        {
            _keybinds = new Dictionary<string, Dictionary<string, ConsoleKey>>();
            LoadKeybinds("keybinds.txt");
        }
        public static KeybindManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new KeybindManager();
                }
                return _instance;
            }
        }
        public void LoadKeybinds(string filePath)
        {
            if (!File.Exists(filePath)) throw new FileNotFoundException($"BŁĄD: Nie znaleziono: {Path.GetFullPath(filePath)}");
            _keybinds.Clear();
            string currentState = "Default";
            var lines = File.ReadAllLines(filePath);

            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();
                if (string.IsNullOrWhiteSpace(trimmedLine)) continue;

                // Wykrywanie sekcji (kontekstu), np. [Playing]
                if (trimmedLine.StartsWith("[") && trimmedLine.EndsWith("]"))
                {
                    // Pobieramy nazwę ze środka nawiasów (usuwamy [ i ])
                    currentState = trimmedLine.Substring(1, trimmedLine.Length - 2);

                    if (!_keybinds.ContainsKey(currentState))
                    {
                        _keybinds[currentState] = new Dictionary<string, ConsoleKey>();
                    }
                    continue;
                }

                // Parsowanie akcji i klawisza, np. MoveUp=W
                var parts = trimmedLine.Split('=');
                if (parts.Length == 2)
                {
                    var action = parts[0].Trim();
                    var keyString = parts[1].Trim();

                    if (Enum.TryParse<ConsoleKey>(keyString, true, out var consoleKey))
                    {
                        if (!_keybinds.ContainsKey(currentState))
                        {
                            _keybinds[currentState] = new Dictionary<string, ConsoleKey>();
                        }
                        if (_keybinds[currentState].ContainsValue(consoleKey))
                        {
                            // Rzucamy wyjątek, który przerwie działanie i jasno poinformuje o błędzie
                            throw new InvalidOperationException($"Konflikt klawiszy! Klawisz '{consoleKey}' jest już przypisany do innej akcji w sekcji [{currentState}]. Popraw plik {filePath}.");
                        }

                        _keybinds[currentState][action] = consoleKey;
                    }
                }
            }
        }

        // Metoda do pobierania przypisanego klawisza
        public ConsoleKey GetKey(string state, string action)
        {
            if (_keybinds.ContainsKey(state) && _keybinds[state].ContainsKey(action))
            {
                return _keybinds[state][action];
            }

            // Domyślny fallback, jeśli brakuje klawisza w pliku
            return ConsoleKey.None;
        }
    }
}
