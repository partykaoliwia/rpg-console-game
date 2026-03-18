using System;
using RpgGame.Engine.States; // jeśli GameEngine jest w tym samym namespace co komendy, ten using może nie być potrzebny, ale zależy od Twojej struktury

namespace RpgGame.Engine.Commands
{
    public class PopStateCommand : ICommand
    {
        private readonly GameEngine _engine;

        public PopStateCommand(GameEngine engine)
        {
            _engine = engine ?? throw new ArgumentNullException(nameof(engine));
        }

        public void Execute()
        {
            // Zdejmujemy aktualny stan (np. InventoryState lub LootingState) ze stosu,
            // co spowoduje automatyczny powrót do poprzedniego stanu (np. PlayingState).
            _engine.PopState();
        }
    }
}