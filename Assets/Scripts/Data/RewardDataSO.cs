using UnityEngine;

[CreateAssetMenu(fileName = "RewardData", menuName = "Vertigo/Reward Data")]
public class RewardDataSO : ScriptableObject
{
    public string id;
    public string displayName;
    public Sprite icon;
    public RewardType rewardType;
    public int value;
}