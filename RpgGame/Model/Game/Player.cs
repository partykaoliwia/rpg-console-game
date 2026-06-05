using RpgGame.Model.Game.Items;

namespace RpgGame.Model.Game;

public sealed class Player
{
    private readonly List<IItem> _inventory = new();
    private readonly Dictionary<string, int> _wallet = new();

    public Player(int id, string name, int startX, int startY)
    {
        Id = id;
        Name = name;
        X = startX;
        Y = startY;
    }

    public int Id { get; }
    public string Name { get; set; }
    public int X { get; private set; }
    public int Y { get; private set; }
    public int BaseStrength { get; set; } = 5;
    public int BaseHealth { get; private set; } = 50;
    public int BaseLuck { get; set; } = 5;
    public int BaseAggression { get; set; } = 5;
    public int BaseWisdom { get; set; } = 5;
    public int BaseDexterity { get; set; } = 5;
    public int Strength => BaseStrength + GetBonusStats().Strength;
    public int Health => Math.Max(0, BaseHealth + GetBonusStats().Health);
    public int Luck => BaseLuck + GetBonusStats().Luck;
    public int Aggression => BaseAggression + GetBonusStats().Aggression;
    public int Wisdom => BaseWisdom + GetBonusStats().Wisdom;
    public int Dexterity => BaseDexterity + GetBonusStats().Dexterity;
    public int Armor { get; set; }
    public int MaxInventorySize { get; } = 6;
    public bool IsAlive => BaseHealth > 0;
    public IReadOnlyList<IItem> Inventory => _inventory;
    public IReadOnlyDictionary<string, int> Wallet => _wallet;
    public IItem? LeftHand { get; private set; }
    public IItem? RightHand { get; private set; }

    public void SetPosition(int x, int y)
    {
        X = x;
        Y = y;
    }

    public bool TryAddToInventory(IItem item)
    {
        if (_inventory.Count >= MaxInventorySize)
        {
            return false;
        }

        _inventory.Add(item);
        return true;
    }

    public IItem? FindInventoryItem(int index)
    {
        return IsInventoryIndexValid(index) ? _inventory[index] : null;
    }

    public bool EquipToLeftHand(IItem item)
    {
        if (!_inventory.Remove(item))
        {
            return false;
        }

        ReturnLeftHandToInventory();
        LeftHand = item;
        return true;
    }

    public bool EquipToRightHand(IItem item)
    {
        if (!_inventory.Remove(item))
        {
            return false;
        }

        ReturnRightHandToInventory();
        RightHand = item;
        return true;
    }

    public bool EquipToBothHands(IItem item)
    {
        if (!_inventory.Remove(item))
        {
            return false;
        }

        var heldItems = GetDistinctHeldItems();
        if (_inventory.Count + heldItems.Count > MaxInventorySize)
        {
            _inventory.Add(item);
            return false;
        }

        foreach (var heldItem in heldItems)
        {
            heldItem.Unequip(this);
            _inventory.Add(heldItem);
        }

        LeftHand = item;
        RightHand = item;
        return true;
    }

    public bool UnequipLeftHand()
    {
        if (LeftHand is null || _inventory.Count >= MaxInventorySize)
        {
            return false;
        }

        LeftHand.Unequip(this);
        _inventory.Add(LeftHand);
        if (ReferenceEquals(RightHand, LeftHand))
        {
            RightHand = null;
        }

        LeftHand = null;
        return true;
    }

    public bool UnequipRightHand()
    {
        if (RightHand is null || _inventory.Count >= MaxInventorySize)
        {
            return false;
        }

        RightHand.Unequip(this);
        _inventory.Add(RightHand);
        if (ReferenceEquals(LeftHand, RightHand))
        {
            LeftHand = null;
        }

        RightHand = null;
        return true;
    }

    public IItem? DropItem(int index)
    {
        if (!IsInventoryIndexValid(index))
        {
            return null;
        }

        var item = _inventory[index];
        _inventory.RemoveAt(index);
        return item;
    }

    public IReadOnlyList<IItem> DropAllCarriedItems()
    {
        var droppedItems = new List<IItem>(_inventory);
        _inventory.Clear();

        foreach (var heldItem in GetDistinctHeldItems())
        {
            heldItem.Unequip(this);
            droppedItems.Add(heldItem);
        }

        LeftHand = null;
        RightHand = null;
        return droppedItems;
    }

    public void AddCurrency(string currencyName, int amount)
    {
        if (!_wallet.ContainsKey(currencyName))
        {
            _wallet[currencyName] = 0;
        }

        _wallet[currencyName] += amount;
    }

    public int TakeDamage(int damage)
    {
        if (!IsAlive)
        {
            return 0;
        }

        var actualDamage = Math.Max(0, damage - Armor);
        BaseHealth = Math.Max(0, BaseHealth - actualDamage);
        return actualDamage;
    }

    public void RestoreState(int x, int y, int health)
    {
        X = x;
        Y = y;
        BaseHealth = Math.Max(0, health);
    }

    public void RestoreEquipment(IItem? leftHand, IItem? rightHand)
    {
        LeftHand = leftHand;
        RightHand = rightHand;
    }

    public void Accept(ICharacterVisitor visitor)
    {
        visitor.Visit(this);
    }

    private PlayerStats GetBonusStats()
    {
        var bonusStats = new PlayerStats();
        if (LeftHand is not null)
        {
            bonusStats += LeftHand.Bonuses;
        }

        if (RightHand is not null && !ReferenceEquals(RightHand, LeftHand))
        {
            bonusStats += RightHand.Bonuses;
        }

        return bonusStats;
    }

    private List<IItem> GetDistinctHeldItems()
    {
        var heldItems = new List<IItem>();
        if (LeftHand is not null)
        {
            heldItems.Add(LeftHand);
        }

        if (RightHand is not null && !ReferenceEquals(RightHand, LeftHand))
        {
            heldItems.Add(RightHand);
        }

        return heldItems;
    }

    private bool IsInventoryIndexValid(int index)
    {
        return index >= 0 && index < _inventory.Count;
    }

    private void ReturnLeftHandToInventory()
    {
        if (LeftHand is null)
        {
            return;
        }

        var item = LeftHand;
        if (ReferenceEquals(RightHand, item))
        {
            RightHand = null;
        }

        item.Unequip(this);
        _inventory.Add(item);
        LeftHand = null;
    }

    private void ReturnRightHandToInventory()
    {
        if (RightHand is null)
        {
            return;
        }

        var item = RightHand;
        if (ReferenceEquals(LeftHand, item))
        {
            LeftHand = null;
        }

        item.Unequip(this);
        _inventory.Add(item);
        RightHand = null;
    }
}
