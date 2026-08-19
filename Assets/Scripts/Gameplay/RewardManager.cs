using UnityEngine;
using System.Collections.Generic;

public class RewardManager {

    private readonly List<RewardDataSO> _collected = new List<RewardDataSO>();
    public IReadOnlyList<RewardDataSO> Collected => _collected;
    public void AddReward(RewardDataSO reward) {
        if (reward == null) 
            return;

        _collected.Add(reward);  } 

    public void ClearAll() {
        _collected.Clear();
    }

    public int TotalCurrency {

        get {
            int sum = 0;
            for (int i = 0; i < _collected.Count; i++) {
                RewardDataSO item = _collected[i];
                if (item.rewardType == RewardType.Currency)
                    sum += item.value;      
        }
        return sum;
    }

}
}
