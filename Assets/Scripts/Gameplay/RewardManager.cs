using System.Collections.Generic;

public class RewardManager
{
    private readonly List<InventoryEntry> _entries = new List<InventoryEntry>();

    public IReadOnlyList<InventoryEntry> Entries => _entries;

    public void AddReward(RewardDataSO reward, int amount)
    {
        if (reward == null || amount <= 0)
            return;

        string key = string.IsNullOrEmpty(reward.id) ? reward.displayName : reward.id;
        if (string.IsNullOrEmpty(key))
            key = reward.name;

        for (int i = 0; i < _entries.Count; i++)
        {
            if (_entries[i].Id == key)
            {
                _entries[i].Amount += amount;
                return;
            }
        }

        bool isItem = reward.rewardType == RewardType.SpecialItem;
        _entries.Add(new InventoryEntry
        {
            Id = key,
            DisplayName = reward.displayName,
            Icon = reward.icon,
            Amount = amount,
            IsItemCount = isItem,
            RewardType = reward.rewardType
        });
    }

    public void ClearAll()
    {
        _entries.Clear();
    }

    public int TotalCurrency
    {
        get
        {
            int sum = 0;
            for (int i = 0; i < _entries.Count; i++)
            {
                if (_entries[i].RewardType == RewardType.Currency)
                    sum += _entries[i].Amount;
            }
            return sum;
        }
    }
}
