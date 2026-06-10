using RpgGame.Model.Game;
using RpgGame.Model.Game.Items;

namespace RpgGame.Dto;

public sealed class GameStateMapper
{
    private readonly Dictionary<TileKind, Func<ITile>> _tileFactories;
    private readonly Dictionary<ItemKind, Func<ItemDto, IItem>> _itemFactories;
    private readonly Dictionary<ItemModifierKind, Func<IItem, IItem>> _modifierFactories;
    private readonly Dictionary<EnemyKind, Func<EnemyDto, Enemy>> _enemyFactories;

    public GameStateMapper()
    {
        _tileFactories = new Dictionary<TileKind, Func<ITile>>
        {
            { TileKind.Floor, () => new FloorTile() },
            { TileKind.Wall, () => new WallTile() }
        };

        _itemFactories = new Dictionary<ItemKind, Func<ItemDto, IItem>>
        {
            { ItemKind.Gold, dto => new Gold(dto.Value) },
            { ItemKind.Coin, dto => new Coin(dto.Value) },
            { ItemKind.Kopis, dto => new Kopis(dto.Damage) },
            { ItemKind.OldBook, _ => new OldBook() },
            { ItemKind.Xiphos, dto => new Xiphos(dto.Damage) },
            { ItemKind.Labrys, dto => new Labrys(dto.Damage) },
            { ItemKind.StymphalianQuill, dto => new StymphalianQuill(dto.Damage) },
            { ItemKind.Dory, dto => new Dory(dto.Damage) },
            { ItemKind.SocketedSword, dto => new SocketedSword(dto.Damage) },
            { ItemKind.AriadnesThread, _ => new AriadnesThread() },
            { ItemKind.CyclopsEye, _ => new CyclopsEye() },
            { ItemKind.IcarusFeather, _ => new IcarusFeather() },
            { ItemKind.ShatteredShield, _ => new ShatteredShield() },
            { ItemKind.BronzeHandle, _ => new BronzeHandle() },
            { ItemKind.LaurelHandle, _ => new LaurelHandle() },
            { ItemKind.StrengthStone, _ => new StrengthStone() },
            { ItemKind.LuckStone, _ => new LuckStone() },
            { ItemKind.WisdomStone, _ => new WisdomStone() }
        };

        _modifierFactories = new Dictionary<ItemModifierKind, Func<IItem, IItem>>
        {
            { ItemModifierKind.Spartan, item => new SpartanModifier(item) },
            { ItemModifierKind.Athenian, item => new AthenianModifier(item) },
            { ItemModifierKind.Hephaestean, item => new HephaesteanModifier(item) },
            { ItemModifierKind.MidasTouch, item => new MidasTouchModifier(item) },
            { ItemModifierKind.Promethean, item => new PrometheanModifier(item) },
            { ItemModifierKind.AchillesBane, item => new AchillesBaneModifier(item) },
            { ItemModifierKind.AphroditesKiss, item => new AphroditesKissModifier(item) },
            { ItemModifierKind.Dionysian, item => new DionysianModifier(item) },
            { ItemModifierKind.HydrasVenom, item => new HydrasVenomModifier(item) },
            { ItemModifierKind.Icarian, item => new IcarianModifier(item) }
        };

        _enemyFactories = new Dictionary<EnemyKind, Func<EnemyDto, Enemy>>
        {
            { EnemyKind.Harpy, dto => RestoreEnemy(new Harpy(dto.Id, dto.X, dto.Y), dto) },
            { EnemyKind.Giant, dto => RestoreEnemy(new Giant(dto.Id, dto.X, dto.Y), dto) },
            { EnemyKind.Satyr, dto => RestoreEnemy(new Satyr(dto.Id, dto.X, dto.Y), dto) }
        };
    }

    public GameStateDto ToDto(GameModel model)
    {
        lock (model.SyncRoot)
        {
            return new GameStateDto
            {
                DungeonThemeName = model.DungeonThemeName,
                Board = CreateBoardDto(model.Board),
                Players = model.Players.Select(CreatePlayerDto).ToList(),
                Enemies = model.Enemies.Select(CreateEnemyDto).ToList(),
                Encounters = model.Encounters.Select(CreateEncounterDto).ToList(),
                Events = model.EventLog.GetAll().ToList()
            };
        }
    }

    public void ApplyTo(GameModel model, GameStateDto dto)
    {
        var board = CreateBoard(dto.Board);
        var players = dto.Players.Select(CreatePlayer).ToList();
        var enemies = dto.Enemies.Select(CreateEnemy).ToList();
        var encounters = dto.Encounters.Select(CreateEncounter).ToList();

        model.ReplaceState(board, dto.DungeonThemeName, players, enemies, dto.Events, encounters);
    }

    private BoardDto CreateBoardDto(Board board)
    {
        var dto = new BoardDto
        {
            Width = board.Width,
            Height = board.Height
        };

        for (var y = 0; y < board.Height; y++)
        {
            for (var x = 0; x < board.Width; x++)
            {
                var tile = board.GetTile(x, y);
                var visitor = new TileDtoVisitor();
                tile.Accept(visitor);

                dto.Tiles.Add(new TileDto
                {
                    X = x,
                    Y = y,
                    Kind = visitor.Kind,
                    Items = tile.Items.Select(CreateItemDto).ToList()
                });
            }
        }

        return dto;
    }

    private Board CreateBoard(BoardDto dto)
    {
        var board = new Board(dto.Width, dto.Height);

        foreach (var tileDto in dto.Tiles)
        {
            if (!_tileFactories.TryGetValue(tileDto.Kind, out var tileFactory))
            {
                throw new InvalidOperationException($"Unsupported tile kind: {tileDto.Kind}.");
            }

            var tile = tileFactory();
            foreach (var itemDto in tileDto.Items)
            {
                tile.AddItem(CreateItem(itemDto));
            }

            board.SetTile(tileDto.X, tileDto.Y, tile);
        }

        return board;
    }

    private PlayerDto CreatePlayerDto(Player player)
    {
        return new PlayerDto
        {
            Id = player.Id,
            Name = player.Name,
            X = player.X,
            Y = player.Y,
            Health = player.BaseHealth,
            BaseStrength = player.BaseStrength,
            BaseLuck = player.BaseLuck,
            BaseAggression = player.BaseAggression,
            BaseWisdom = player.BaseWisdom,
            BaseDexterity = player.BaseDexterity,
            Armor = player.Armor,
            Wallet = player.Wallet.ToDictionary(pair => pair.Key, pair => pair.Value),
            Inventory = player.Inventory.Select(CreateItemDto).ToList(),
            LeftHand = player.LeftHand == null ? null : CreateItemDto(player.LeftHand),
            RightHand = player.RightHand == null ? null : CreateItemDto(player.RightHand),
            UsesBothHands = player.LeftHand != null && ReferenceEquals(player.LeftHand, player.RightHand)
        };
    }

    private Player CreatePlayer(PlayerDto dto)
    {
        var player = new Player(dto.Id, dto.Name, dto.X, dto.Y)
        {
            BaseStrength = dto.BaseStrength == 0 ? 5 : dto.BaseStrength,
            BaseLuck = dto.BaseLuck == 0 ? 5 : dto.BaseLuck,
            BaseAggression = dto.BaseAggression == 0 ? 5 : dto.BaseAggression,
            BaseWisdom = dto.BaseWisdom == 0 ? 5 : dto.BaseWisdom,
            BaseDexterity = dto.BaseDexterity == 0 ? 5 : dto.BaseDexterity,
            Armor = dto.Armor
        };
        player.RestoreState(dto.X, dto.Y, dto.Health);

        foreach (var walletEntry in dto.Wallet)
        {
            player.AddCurrency(walletEntry.Key, walletEntry.Value);
        }

        foreach (var itemDto in dto.Inventory)
        {
            player.TryAddToInventory(CreateItem(itemDto));
        }

        var leftHand = dto.LeftHand == null ? null : CreateItem(dto.LeftHand);
        var rightHand = dto.RightHand == null ? null : CreateItem(dto.RightHand);
        if (dto.UsesBothHands)
        {
            var twoHandedItem = leftHand ?? rightHand;
            leftHand = twoHandedItem;
            rightHand = twoHandedItem;
        }

        player.RestoreEquipment(leftHand, rightHand);

        return player;
    }

    private EnemyDto CreateEnemyDto(Enemy enemy)
    {
        var visitor = new EnemyDtoVisitor();
        enemy.Accept(visitor);

        return new EnemyDto
        {
            Id = enemy.Id,
            Kind = visitor.Kind,
            Name = enemy.Name,
            X = enemy.X,
            Y = enemy.Y,
            Health = enemy.Health,
            Strength = enemy.Strength,
            Armor = enemy.Armor
        };
    }

    private Enemy CreateEnemy(EnemyDto dto)
    {
        if (!_enemyFactories.TryGetValue(dto.Kind, out var factory))
        {
            throw new InvalidOperationException($"Unsupported enemy kind: {dto.Kind}.");
        }

        return factory(dto);
    }

    private static CombatEncounterDto CreateEncounterDto(CombatEncounter encounter)
    {
        return new CombatEncounterDto
        {
            Sequence = encounter.Sequence,
            PlayerId = encounter.PlayerId,
            EnemyId = encounter.EnemyId
        };
    }

    private static CombatEncounter CreateEncounter(CombatEncounterDto dto)
    {
        return new CombatEncounter(dto.Sequence, dto.PlayerId, dto.EnemyId);
    }

    private static Enemy RestoreEnemy(Enemy enemy, EnemyDto dto)
    {
        var strength = dto.Strength == 0 ? enemy.Strength : dto.Strength;
        var armor = dto.Armor == 0 ? enemy.Armor : dto.Armor;
        enemy.RestoreState(dto.Name, dto.X, dto.Y, dto.Health, strength, armor);
        return enemy;
    }

    private ItemDto CreateItemDto(IItem item)
    {
        var visitor = new ItemDtoVisitor();
        item.Accept(visitor);
        return visitor.Dto;
    }

    private IItem CreateItem(ItemDto dto)
    {
        if (!_itemFactories.TryGetValue(dto.Kind, out var factory))
        {
            throw new InvalidOperationException($"Unsupported item kind: {dto.Kind}.");
        }

        var item = factory(dto);
        for (var slotIndex = 0; slotIndex < dto.Slots.Count; slotIndex++)
        {
            var slotDto = dto.Slots[slotIndex];
            if (slotDto == null)
            {
                continue;
            }

            var slotItem = CreateItem(slotDto);
            if (!item.TryPlaceInSlot(slotIndex, slotItem))
            {
                throw new InvalidOperationException($"Unsupported slot item for {dto.Kind} at slot {slotIndex}.");
            }
        }

        foreach (var modifier in dto.Modifiers)
        {
            if (!_modifierFactories.TryGetValue(modifier, out var modifierFactory))
            {
                throw new InvalidOperationException($"Unsupported item modifier: {modifier}.");
            }

            item = modifierFactory(item);
        }

        return item;
    }

    private sealed class TileDtoVisitor : ITileVisitor
    {
        public TileKind Kind { get; private set; }

        public void Visit(FloorTile floor)
        {
            Kind = TileKind.Floor;
        }

        public void Visit(WallTile wall)
        {
            Kind = TileKind.Wall;
        }
    }

    private sealed class EnemyDtoVisitor : ICharacterVisitor
    {
        public EnemyKind Kind { get; private set; }

        public void Visit(Player player)
        {
        }

        public void Visit(Harpy harpy)
        {
            Kind = EnemyKind.Harpy;
        }

        public void Visit(Giant giant)
        {
            Kind = EnemyKind.Giant;
        }

        public void Visit(Satyr satyr)
        {
            Kind = EnemyKind.Satyr;
        }
    }

    private sealed class ItemDtoVisitor : IItemVisitor
    {
        public ItemDto Dto { get; private set; } = new();

        public void Visit(Gold gold)
        {
            Dto = new ItemDto
            {
                Kind = ItemKind.Gold,
                Name = gold.Name,
                Value = gold.Value
            };
        }

        public void Visit(Coin coin)
        {
            Dto = new ItemDto
            {
                Kind = ItemKind.Coin,
                Name = coin.Name,
                Value = coin.Value
            };
        }

        public void Visit(Kopis kopis)
        {
            CreateWeaponDto(kopis, ItemKind.Kopis);
        }

        public void Visit(Xiphos xiphos)
        {
            CreateWeaponDto(xiphos, ItemKind.Xiphos);
        }

        public void Visit(Labrys labrys)
        {
            CreateWeaponDto(labrys, ItemKind.Labrys);
        }

        public void Visit(StymphalianQuill stymphalianQuill)
        {
            CreateWeaponDto(stymphalianQuill, ItemKind.StymphalianQuill);
        }

        public void Visit(Dory dory)
        {
            CreateWeaponDto(dory, ItemKind.Dory);
        }

        public void Visit(SocketedSword socketedSword)
        {
            CreateWeaponDto(socketedSword, ItemKind.SocketedSword);
        }

        public void Visit(OldBook oldBook)
        {
            CreateJunkDto(oldBook, ItemKind.OldBook);
        }

        public void Visit(AriadnesThread ariadnesThread)
        {
            CreateJunkDto(ariadnesThread, ItemKind.AriadnesThread);
        }

        public void Visit(CyclopsEye cyclopsEye)
        {
            CreateJunkDto(cyclopsEye, ItemKind.CyclopsEye);
        }

        public void Visit(IcarusFeather icarusFeather)
        {
            CreateJunkDto(icarusFeather, ItemKind.IcarusFeather);
        }

        public void Visit(ShatteredShield shatteredShield)
        {
            CreateJunkDto(shatteredShield, ItemKind.ShatteredShield);
        }

        public void Visit(BronzeHandle bronzeHandle)
        {
            CreateHandleDto(bronzeHandle, ItemKind.BronzeHandle);
        }

        public void Visit(LaurelHandle laurelHandle)
        {
            CreateHandleDto(laurelHandle, ItemKind.LaurelHandle);
        }

        public void Visit(StrengthStone strengthStone)
        {
            CreatePassiveDto(strengthStone, ItemKind.StrengthStone);
        }

        public void Visit(LuckStone luckStone)
        {
            CreatePassiveDto(luckStone, ItemKind.LuckStone);
        }

        public void Visit(WisdomStone wisdomStone)
        {
            CreatePassiveDto(wisdomStone, ItemKind.WisdomStone);
        }

        public void Visit(SpartanModifier modifier)
        {
            CreateDecoratedDto(modifier, ItemModifierKind.Spartan);
        }

        public void Visit(AthenianModifier modifier)
        {
            CreateDecoratedDto(modifier, ItemModifierKind.Athenian);
        }

        public void Visit(HephaesteanModifier modifier)
        {
            CreateDecoratedDto(modifier, ItemModifierKind.Hephaestean);
        }

        public void Visit(MidasTouchModifier modifier)
        {
            CreateDecoratedDto(modifier, ItemModifierKind.MidasTouch);
        }

        public void Visit(PrometheanModifier modifier)
        {
            CreateDecoratedDto(modifier, ItemModifierKind.Promethean);
        }

        public void Visit(AchillesBaneModifier modifier)
        {
            CreateDecoratedDto(modifier, ItemModifierKind.AchillesBane);
        }

        public void Visit(AphroditesKissModifier modifier)
        {
            CreateDecoratedDto(modifier, ItemModifierKind.AphroditesKiss);
        }

        public void Visit(DionysianModifier modifier)
        {
            CreateDecoratedDto(modifier, ItemModifierKind.Dionysian);
        }

        public void Visit(HydrasVenomModifier modifier)
        {
            CreateDecoratedDto(modifier, ItemModifierKind.HydrasVenom);
        }

        public void Visit(IcarianModifier modifier)
        {
            CreateDecoratedDto(modifier, ItemModifierKind.Icarian);
        }

        private void CreateDecoratedDto(ItemDecorator modifier, ItemModifierKind kind)
        {
            modifier.InnerItem.Accept(this);
            Dto.Modifiers.Add(kind);
            Dto.Name = modifier.Name;
        }

        private void CreateWeaponDto(Weapon weapon, ItemKind kind)
        {
            Dto = new ItemDto
            {
                Kind = kind,
                Name = weapon.Name,
                Damage = weapon.Damage
            };
            AddSlotDtos(weapon);
        }

        private void CreateJunkDto(JunkItem item, ItemKind kind)
        {
            Dto = new ItemDto
            {
                Kind = kind,
                Name = item.Name
            };
            AddSlotDtos(item);
        }

        private void CreateHandleDto(WeaponHandle handle, ItemKind kind)
        {
            Dto = new ItemDto
            {
                Kind = kind,
                Name = handle.Name
            };
            AddSlotDtos(handle);
        }

        private void CreatePassiveDto(PassiveItem item, ItemKind kind)
        {
            Dto = new ItemDto
            {
                Kind = kind,
                Name = item.Name
            };
        }

        private void AddSlotDtos(IItem item)
        {
            foreach (var slot in item.Slots)
            {
                if (slot.Item == null)
                {
                    Dto.Slots.Add(null);
                    continue;
                }

                var visitor = new ItemDtoVisitor();
                slot.Item.Accept(visitor);
                Dto.Slots.Add(visitor.Dto);
            }
        }
    }
}
