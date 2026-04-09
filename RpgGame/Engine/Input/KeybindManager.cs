using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpgGame.Engine.Input
{
    public class KeybindManager
    {
        private readonly Dictionary<string, Dictionary<string, ConsoleKey>> _keybinds;
        public KeybindManager()
        {
            _keybinds = new Dictionary<string, Dictionary<string, ConsoleKey>>();
            LoadKeybinds("keybinds.txt");
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

                // wykrywanie state'u
                if (trimmedLine.StartsWith("[") && trimmedLine.EndsWith("]"))
                {
                    currentState = trimmedLine.Substring(1, trimmedLine.Length - 2);
                    if (!_keybinds.ContainsKey(currentState))
                    {
                        _keybinds[currentState] = new Dictionary<string, ConsoleKey>();
                    }
                    continue;
                }

                // parsowanie akcji i klawisza
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
                            throw new InvalidOperationException($"Konflikt klawiszy! Klawisz '{consoleKey}' jest już przypisany do innej akcji w sekcji [{currentState}]. Popraw plik {filePath}.");
                        }

                        _keybinds[currentState][action] = consoleKey;
                    }
                }
            }
        }

        public ConsoleKey GetKey(string state, string action)
        {
            if (_keybinds.ContainsKey(state) && _keybinds[state].ContainsKey(action))
            {
                return _keybinds[state][action];
            }

            return ConsoleKey.None;
        }
        public string GetKeyName(string state, string action)
        {
            ConsoleKey key = GetKey(state, action);
            if (key == ConsoleKey.None) return "?";

            string keyName = key.ToString();

            // D1 -> 1, D2 -> 2 itd.
            if (keyName.StartsWith("D") && keyName.Length == 2 && char.IsDigit(keyName[1]))
            {
                return keyName.Substring(1);
            }

            if (keyName.StartsWith("NumPad")) return keyName.Replace("NumPad", "");
            if (keyName == "Oem3") return "~";

            return keyName;
        }
    }
}
