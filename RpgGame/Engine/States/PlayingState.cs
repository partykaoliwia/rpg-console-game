using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RpgGame.Engine.Commands;
using RpgGame.Characters;
using RpgGame.Gameboard;

namespace RpgGame.Engine.States
{
    public class PlayingState : IGameState
    {
        private readonly GameEngine _engine;
        private readonly Board _board;
        private readonly Player _player;
        private readonly Dictionary<ConsoleKey, ICommand> _keyBindings;
        private string _errorMessage = string.Empty;
        public PlayingState(GameEngine engine, Board board, Player player)
        {
            _engine = engine;
            _board = board;
            _player = player;

            _keyBindings = new Dictionary<ConsoleKey, ICommand>
            {
                { ConsoleKey.W, new MoveCommand(player, board, 0, -1) },
                { ConsoleKey.A, new MoveCommand(player, board, -1, 0) },
                { ConsoleKey.S, new MoveCommand(player, board, 0, 1) },
                { ConsoleKey.D, new MoveCommand(player, board, 1, 0) },
                {ConsoleKey.E, new CollectCommand(engine, player, board) },
                { ConsoleKey.I, new PushStateCommand(engine, new InventoryState(engine, player, board)) },
                { ConsoleKey.Escape, new QuitCommand(engine) }
            };
        }
        public void Draw(ConsoleRenderer renderer)
        {
            renderer.DrawBoard(_board, _player);
            if(!string.IsNullOrEmpty(_errorMessage))
            {
                renderer.DrawMessage(_errorMessage);
                renderer.DrawPlayerStats(_player);
            }
            else
            {
                renderer.DrawTutorial(_board, _player);
            }
        }

        public void HandleInput(ConsoleKey key)
        {
            _errorMessage = string.Empty; // czyścimy komunikat co turę
            
            if (_keyBindings.TryGetValue(key, out ICommand command)) // jeśli pod danym klawiszem jest jakaś akcja
            {
                command.Execute();
            }
            else // gdy klawisz nieznany
            {
                _errorMessage = $"Klawisz '{key}' nie jest przypisany do żadnej akcji.";
            }
        }
    }
    }
