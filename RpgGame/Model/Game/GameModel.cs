using RpgGame.Model.Game.Combat;
using RpgGame.Model.Game.Behaviors;
using RpgGame.Model.Game.Builders;
using RpgGame.Model.Game.Factories;
using RpgGame.Model.Game.Items;
using RpgGame.Model.Game.Observers;
using RpgGame.Model.Global;

namespace RpgGame.Model.Game;

public sealed class GameModel
{
    private readonly List<Player> _players = new();
    private readonly List<Enemy> _enemies = new();
    private readonly Dictionary<int, CombatEncounter> _encountersByPlayerId = new();
    private readonly SoundPublisher _soundPublisher = new();
    private readonly SpeciesPublisher _harpySpecies = new("Harpy");
    private readonly SpeciesPublisher _giantSpecies = new("Giant");
    private readonly SpeciesPublisher _satyrSpecies = new("Satyr");
    private readonly Random _enemyMovementRandom = new();
    private int _nextEncounterSequence;

    public GameModel(Board board, string dungeonThemeName = "")
    {
        Board = board;
        DungeonThemeName = dungeonThemeName;
    }

    public object SyncRoot { get; } = new();
    public Board Board { get; private set; }
    public string DungeonThemeName { get; private set; }
    public GameEventLog EventLog { get; } = new();
    public IReadOnlyList<Player> Players => _players;
    public IReadOnlyList<Enemy> Enemies => _enemies;
    public IReadOnlyList<CombatEncounter> Encounters => _encountersByPlayerId.Values.ToList();

    public Player? FindPlayer(int playerId)
    {
        return _players.FirstOrDefault(player => player.Id == playerId);
    }

    public Enemy? FindEnemy(int enemyId)
    {
        return _enemies.FirstOrDefault(enemy => enemy.Id == enemyId);
    }

    public Player AddPlayer(string name)
    {
        lock (SyncRoot)
        {
            if (_players.Count >= 9)
            {
                throw new InvalidOperationException("The game supports at most 9 players.");
            }

            var id = CreatePlayerId();
            var startPosition = FindFreeStartPosition();
            var player = new Player(id, name, startPosition.X, startPosition.Y);
            _players.Add(player);
            EventLog.Add($"{player.Name} joined the game.");
            return player;
        }
    }

    public void AddPlayer(Player player)
    {
        lock (SyncRoot)
        {
            _players.Add(player);
        }
    }

    public bool RemovePlayer(int playerId)
    {
        lock (SyncRoot)
        {
            var player = FindPlayer(playerId);
            if (player == null)
            {
                return false;
            }

            var tile = Board.GetTile(player.X, player.Y);
            foreach (var item in player.DropAllCarriedItems())
            {
                tile.AddItem(item);
            }

            _players.Remove(player);
            _encountersByPlayerId.Remove(player.Id);
            EventLog.Add($"{player.Name} left the game.");
            return true;
        }
    }

    public void RecordRejectedCommand(int playerId, string reason)
    {
        lock (SyncRoot)
        {
            var player = FindPlayer(playerId);
            var playerName = player?.Name ?? $"Player {playerId}";
            EventLog.Add($"{playerName}'s command was rejected: {reason}");
        }
    }

    public void AddEnemy(Enemy enemy)
    {
        lock (SyncRoot)
        {
            RegisterEnemy(enemy);
            _enemies.Add(enemy);
        }
    }

    public void MovePlayer(int playerId, int dx, int dy)
    {
        lock (SyncRoot)
        {
            var player = FindPlayer(playerId);
            if (player == null || !player.IsAlive)
            {
                return;
            }

            var newX = player.X + dx;
            var newY = player.Y + dy;
            var targetTile = Board.GetTile(newX, newY);

            if (!targetTile.IsWalkable)
            {
                EventLog.Add($"{player.Name} tried to walk into a wall.");
                return;
            }

            var enemy = FindLivingEnemyAt(newX, newY);
            if (enemy != null)
            {
                RegisterEncounter(player, enemy);
                return;
            }

            var otherPlayer = FindLivingPlayerAt(newX, newY);
            if (otherPlayer != null)
            {
                EventLog.Add($"{player.Name} cannot move onto {otherPlayer.Name}.");
                return;
            }

            player.SetPosition(newX, newY);
        }
    }

    public void BeginEncounterWithAdjacentEnemy(int playerId)
    {
        lock (SyncRoot)
        {
            var player = FindPlayer(playerId);
            if (player == null || !player.IsAlive)
            {
                return;
            }

            var enemy = FindFirstAdjacentEnemy(playerId);
            if (enemy == null)
            {
                EventLog.Add($"{player.Name} found no enemy nearby.");
                return;
            }

            RegisterEncounter(player, enemy);
        }
    }

    public void EndEncounterForPlayer(int playerId)
    {
        lock (SyncRoot)
        {
            var player = FindPlayer(playerId);
            if (!_encountersByPlayerId.Remove(playerId) || player == null)
            {
                return;
            }

            EventLog.Add($"{player.Name} left combat.");
        }
    }

    public bool UpdateEnemies()
    {
        lock (SyncRoot)
        {
            var didChangeState = false;
            var now = DateTime.UtcNow;

            foreach (var enemy in _enemies.ToList())
            {
                if (!enemy.IsAlive || (now - enemy.LastMovedUtc).TotalMilliseconds < enemy.MoveEveryMs)
                {
                    continue;
                }

                enemy.LastMovedUtc = now;
                didChangeState = TryMoveEnemy(enemy) || didChangeState;
            }

            return didChangeState;
        }
    }

    public void CollectTopItem(int playerId)
    {
        CollectItem(playerId, 0);
    }

    public void CollectItem(int playerId, int itemIndex)
    {
        lock (SyncRoot)
        {
            var player = FindPlayer(playerId);
            if (player == null || !player.IsAlive)
            {
                return;
            }

            var tile = Board.GetTile(player.X, player.Y);
            var item = FindTileItem(tile, itemIndex);
            if (item == null)
            {
                EventLog.Add($"{player.Name} found nothing to collect.");
                return;
            }

            if (!item.PickUp(player))
            {
                EventLog.Add($"{player.Name} cannot carry {item.Name}.");
                return;
            }

            tile.TryRemoveItem(item);
            EventLog.Add($"{player.Name} picked up {item.Name}.");
            NotifySound(player.X, player.Y, item.SoundRange);
        }
    }

    public void AttackEnemy(int playerId, int enemyId)
    {
        AttackEnemy(playerId, enemyId, new NormalAttack());
    }

    public void AttackEnemy(int playerId, int enemyId, IAttackVisitor attack)
    {
        AttackEnemy(playerId, enemyId, attack, player => player.LeftHand ?? player.RightHand);
    }

    public void AttackEnemyWithLeftHand(int playerId, int enemyId, IAttackVisitor attack)
    {
        AttackEnemy(playerId, enemyId, attack, player => player.LeftHand);
    }

    public void AttackEnemyWithRightHand(int playerId, int enemyId, IAttackVisitor attack)
    {
        AttackEnemy(playerId, enemyId, attack, player => player.RightHand);
    }

    private void AttackEnemy(
        int playerId,
        int enemyId,
        IAttackVisitor attack,
        Func<Player, IItem?> heldItemSelector)
    {
        lock (SyncRoot)
        {
            var player = FindPlayer(playerId);
            var enemy = FindEnemy(enemyId);

            if (player == null || enemy == null || !player.IsAlive || !enemy.IsAlive)
            {
                return;
            }

            if (!AreAdjacentOrSame(player.X, player.Y, enemy.X, enemy.Y))
            {
                EventLog.Add($"{player.Name} is too far away to attack {enemy.Name}.");
                return;
            }

            var heldItem = heldItemSelector(player);
            var combatStats = heldItem?.AcceptAttack(attack, player)
                ?? CreateUnarmedCombatStats(player);
            var soundRange = heldItem?.SoundRange ?? 3;
            var weaponName = heldItem?.Name ?? "bare hands";

            var dealtDamage = enemy.TakeDamage(combatStats.Damage);
            EventLog.Add($"{player.Name} used {attack.Name} with {weaponName} against {enemy.Name} for {dealtDamage} damage.");

            if (!enemy.IsAlive)
            {
                enemy.Die(EventLog);
                _enemies.Remove(enemy);
                ClearEncountersForEnemy(enemy.Id);
                EventLog.Add($"{enemy.Name} was defeated.");
                NotifySound(player.X, player.Y, soundRange);
                return;
            }

            enemy.UpdateAfterAttack(EventLog);
            NotifySound(player.X, player.Y, soundRange);

            var receivedDamage = player.TakeDamage(Math.Max(0, enemy.Strength - combatStats.Defense));
            EventLog.Add($"{enemy.Name} counterattacked {player.Name} for {receivedDamage} damage.");

            if (!player.IsAlive)
            {
                _encountersByPlayerId.Remove(player.Id);
                EventLog.Add($"{player.Name} died.");
            }
        }
    }

    public void EquipLeftHand(int playerId, int inventoryIndex)
    {
        EquipItem(playerId, inventoryIndex, (item, player) => item.EquipToLeftHand(player), "left hand");
    }

    public void EquipRightHand(int playerId, int inventoryIndex)
    {
        EquipItem(playerId, inventoryIndex, (item, player) => item.EquipToRightHand(player), "right hand");
    }

    public void UnequipLeftHand(int playerId)
    {
        UnequipItem(playerId, player => player.UnequipLeftHand(), "left hand");
    }

    public void UnequipRightHand(int playerId)
    {
        UnequipItem(playerId, player => player.UnequipRightHand(), "right hand");
    }

    public void DropInventoryItem(int playerId, int inventoryIndex)
    {
        lock (SyncRoot)
        {
            var player = FindPlayer(playerId);
            if (player == null || !player.IsAlive)
            {
                return;
            }

            var droppedItem = player.DropItem(inventoryIndex);
            if (droppedItem == null)
            {
                EventLog.Add($"{player.Name} tried to drop an empty inventory slot.");
                return;
            }

            Board.GetTile(player.X, player.Y).AddItem(droppedItem);
            EventLog.Add($"{player.Name} dropped {droppedItem.Name}.");
            NotifySound(player.X, player.Y, droppedItem.SoundRange);
        }
    }

    public Enemy? FindLivingEnemyAt(int x, int y)
    {
        return _enemies.FirstOrDefault(enemy => enemy.IsAlive && enemy.X == x && enemy.Y == y);
    }

    public Player? FindLivingPlayerAt(int x, int y)
    {
        return _players.FirstOrDefault(player => player.IsAlive && player.X == x && player.Y == y);
    }

    public bool CanPlayerAttackEnemy(int playerId, int enemyId)
    {
        lock (SyncRoot)
        {
            var player = FindPlayer(playerId);
            var enemy = FindEnemy(enemyId);
            return player != null &&
                enemy != null &&
                player.IsAlive &&
                enemy.IsAlive &&
                AreAdjacentOrSame(player.X, player.Y, enemy.X, enemy.Y);
        }
    }

    public Enemy? FindFirstAdjacentEnemy(int playerId)
    {
        var player = FindPlayer(playerId);
        if (player == null)
        {
            return null;
        }

        return _enemies.FirstOrDefault(enemy =>
            enemy.IsAlive && AreAdjacentOrSame(player.X, player.Y, enemy.X, enemy.Y));
    }

    public CombatEncounter? FindEncounterForPlayer(int playerId)
    {
        return _encountersByPlayerId.TryGetValue(playerId, out var encounter) ? encounter : null;
    }

    public bool HasEncounterWithEnemy(int playerId, int enemyId)
    {
        lock (SyncRoot)
        {
            return _encountersByPlayerId.TryGetValue(playerId, out var encounter) &&
                encounter.EnemyId == enemyId &&
                FindEnemy(enemyId)?.IsAlive == true;
        }
    }

    public void ReplaceState(
        Board board,
        string dungeonThemeName,
        IEnumerable<Player> players,
        IEnumerable<Enemy> enemies,
        IEnumerable<string> events,
        IEnumerable<CombatEncounter> encounters)
    {
        lock (SyncRoot)
        {
            Board = board;
            DungeonThemeName = dungeonThemeName;
            foreach (var enemy in _enemies)
            {
                enemy.LeaveObservers();
            }

            _players.Clear();
            _players.AddRange(players);
            _enemies.Clear();
            foreach (var enemy in enemies)
            {
                RegisterEnemy(enemy);
                _enemies.Add(enemy);
            }

            _encountersByPlayerId.Clear();
            _nextEncounterSequence = 0;
            foreach (var encounter in encounters)
            {
                _encountersByPlayerId[encounter.PlayerId] = encounter;
                _nextEncounterSequence = Math.Max(_nextEncounterSequence, encounter.Sequence);
            }

            EventLog.ReplaceWith(events);
        }
    }

    public static GameModel CreateDemoGame()
    {
        return CreateDemoGame("Tezeusz");
    }

    public static GameModel CreateDemoGame(string playerName)
    {
        var model = CreateGame(new LabyrinthFactory());
        model.AddPlayer(playerName);
        return model;
    }

    public static GameModel CreateDemoWorld()
    {
        return CreateGame(new LabyrinthFactory());
    }

    public static GameModel CreateGame(IDungeonThemeFactory factory)
    {
        var director = new DungeonDirector();
        var builder = new MapBuilder(factory);
        director.SetBuilder(builder);
        director.SetStrategy(factory.GenerationStrategy);
        director.ConstructDungeon();

        var generatedDungeon = builder.BuildResult();
        var model = new GameModel(generatedDungeon.Board, factory.ThemeName);
        foreach (var enemy in generatedDungeon.Enemies)
        {
            model.AddEnemy(enemy);
        }

        model.EventLog.Add(factory.IntroMessage);
        return model;
    }

    private static bool AreAdjacentOrSame(int ax, int ay, int bx, int by)
    {
        return Math.Abs(ax - bx) + Math.Abs(ay - by) <= 1;
    }

    private static CombatStats CreateUnarmedCombatStats(Player player)
    {
        return new CombatStats
        {
            Damage = player.Strength,
            Defense = player.Dexterity
        };
    }

    private static IItem? FindTileItem(ITile tile, int itemIndex)
    {
        return itemIndex >= 0 && itemIndex < tile.Items.Count ? tile.Items[itemIndex] : null;
    }

    private bool TryMoveEnemy(Enemy enemy)
    {
        if (IsEnemyInEncounter(enemy.Id))
        {
            return false;
        }

        var context = new EnemyBehaviorContext(Board, _players, _enemies, _enemyMovementRandom);
        var decision = enemy.DecideBehavior(context);
        if (decision.ShouldAttack)
        {
            var targetPlayer = FindPlayer(decision.AttackPlayerId.Value);
            return targetPlayer != null && targetPlayer.IsAlive && RegisterEncounter(targetPlayer, enemy);
        }

        if (!decision.ShouldMove)
        {
            return false;
        }

        var newX = enemy.X + decision.DeltaX;
        var newY = enemy.Y + decision.DeltaY;
        var player = FindLivingPlayerAt(newX, newY);
        if (player != null)
        {
            return RegisterEncounter(player, enemy);
        }

        var targetTile = Board.GetTile(newX, newY);
        if (!targetTile.IsWalkable || FindLivingEnemyAt(newX, newY) != null)
        {
            return false;
        }

        enemy.SetPosition(newX, newY);
        return true;
    }

    private bool IsEnemyInEncounter(int enemyId)
    {
        return _encountersByPlayerId.Values.Any(encounter => encounter.EnemyId == enemyId);
    }

    private (int X, int Y) FindFreeStartPosition()
    {
        for (var y = 1; y < Board.Height - 1; y++)
        {
            for (var x = 1; x < Board.Width - 1; x++)
            {
                if (Board.GetTile(x, y).IsWalkable &&
                    FindLivingPlayerAt(x, y) == null &&
                    FindLivingEnemyAt(x, y) == null)
                {
                    return (x, y);
                }
            }
        }

        throw new InvalidOperationException("No free player spawn position is available.");
    }

    private int CreatePlayerId()
    {
        for (var id = 1; id <= 9; id++)
        {
            if (_players.All(player => player.Id != id))
            {
                return id;
            }
        }

        throw new InvalidOperationException("The game supports at most 9 players.");
    }

    private bool RegisterEncounter(Player player, Enemy enemy)
    {
        if (_encountersByPlayerId.TryGetValue(player.Id, out var current) &&
            current.EnemyId == enemy.Id &&
            enemy.IsAlive &&
            player.IsAlive)
        {
            return false;
        }

        var encounter = new CombatEncounter(++_nextEncounterSequence, player.Id, enemy.Id);
        _encountersByPlayerId[player.Id] = encounter;
        EventLog.Add($"{enemy.Name} approached {player.Name}. Entering combat.");
        return true;
    }

    private void ClearEncountersForEnemy(int enemyId)
    {
        foreach (var encounter in _encountersByPlayerId.Values.ToList())
        {
            if (encounter.EnemyId == enemyId)
            {
                _encountersByPlayerId.Remove(encounter.PlayerId);
            }
        }
    }

    private void EquipItem(
        int playerId,
        int inventoryIndex,
        Func<IItem, Player, bool> equipAction,
        string handName)
    {
        lock (SyncRoot)
        {
            var player = FindPlayer(playerId);
            if (player == null || !player.IsAlive)
            {
                return;
            }

            var item = player.FindInventoryItem(inventoryIndex);
            if (item == null)
            {
                EventLog.Add($"{player.Name} tried to equip an empty inventory slot.");
                return;
            }

            if (equipAction(item, player))
            {
                EventLog.Add($"{player.Name} equipped {item.Name} to {handName}.");
                return;
            }

            EventLog.Add($"{player.Name} cannot equip {item.Name}.");
        }
    }

    private void UnequipItem(int playerId, Func<Player, bool> unequipAction, string handName)
    {
        lock (SyncRoot)
        {
            var player = FindPlayer(playerId);
            if (player == null || !player.IsAlive)
            {
                return;
            }

            if (unequipAction(player))
            {
                EventLog.Add($"{player.Name} unequipped {handName}.");
                return;
            }

            EventLog.Add($"{player.Name} cannot unequip {handName}.");
        }
    }

    private void NotifySound(int x, int y, int range)
    {
        if (range <= 0)
        {
            return;
        }

        _soundPublisher.NotifySubscribers(x, y, range, Board, EventLog);
    }

    private void RegisterEnemy(Enemy enemy)
    {
        var visitor = new EnemyObserverRegistrationVisitor(_harpySpecies, _giantSpecies, _satyrSpecies, _soundPublisher);
        enemy.Accept(visitor);
    }

    private sealed class EnemyObserverRegistrationVisitor : ICharacterVisitor
    {
        private readonly SpeciesPublisher _harpies;
        private readonly SpeciesPublisher _giants;
        private readonly SpeciesPublisher _satyrs;
        private readonly SoundPublisher _sound;

        public EnemyObserverRegistrationVisitor(
            SpeciesPublisher harpies,
            SpeciesPublisher giants,
            SpeciesPublisher satyrs,
            SoundPublisher sound)
        {
            _harpies = harpies;
            _giants = giants;
            _satyrs = satyrs;
            _sound = sound;
        }

        public void Visit(Player player)
        {
        }

        public void Visit(Harpy harpy)
        {
            harpy.JoinObservers(_harpies, _sound);
        }

        public void Visit(Giant giant)
        {
            giant.JoinObservers(_giants, _sound);
        }

        public void Visit(Satyr satyr)
        {
            satyr.JoinObservers(_satyrs, _sound);
        }
    }
}
