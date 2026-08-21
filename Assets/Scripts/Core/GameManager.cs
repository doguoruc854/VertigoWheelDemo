using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private WheelConfigSO wheelConfig;
    [SerializeField] private HUDController hud;

    private ZoneManager _zones;
    private GameStateMachine _state;
    private RewardManager _rewards;
    private WheelResolver _resolver;

    private void Awake() {
        _zones = new ZoneManager();
        _state = new GameStateMachine();
        _rewards = new RewardManager();
        _resolver = new WheelResolver();

    }
    private void RefreshHud(){
    if (hud != null)
        hud.Refresh(_rewards.TotalCurrency, _zones.CurrentZone);
}


    private void Update() {
        if (!Input.GetKeyDown(KeyCode.Space))
            return;

        if (_state.CurrentState == GameState.Idle)    {
            TrySpin();
            return;
        }
        if (_state.CurrentState == GameState.GameOver){
            RestartAfterBomb();
        }
    }


    private void RestartAfterBomb(){
    _rewards.ClearAll();
    _zones.Reset();
    _state.TryStateTransition(GameState.Idle);
    Debug.Log("Restart → Zone 1, rewards cleared");
    RefreshHud();
}

    private void TrySpin(){
        if (!_state.TryStateTransition(GameState.Spinning))
        return;

        WheelSliceData slice = _resolver.Resolve(wheelConfig);

        if (!_state.TryStateTransition(GameState.Result))
        return;

        if (slice == null){
            Debug.LogWarning("No slice resolved");
            _state.TryStateTransition(GameState.Idle);
            return;
        }

        if (slice.isBomb) {
            _rewards.ClearAll();
            _state.TryStateTransition(GameState.GameOver);
            Debug.Log($"BOMB HIT GAME OVER | Zone {_zones.CurrentZone} | Total {_rewards.TotalCurrency}");
            RefreshHud();
            return;
        }
        ApplyReward (slice);
        _zones.AdvanceZone();
        _state.TryStateTransition(GameState.Idle);

        Debug.Log(
            $"Reward OK | Zone {_zones.CurrentZone} ({_zones.CurrentType}) | Total {_rewards.TotalCurrency}");
        RefreshHud();
        return;
        }    

        private void ApplyReward(WheelSliceData slice){
            if (slice.reward == null)
            return;

            var context = new RewardContext(_rewards, slice.reward);

            if (slice.reward.rewardType == RewardType.Currency)
                new CurrencyRewardEffect().Apply(context);
            else if (slice.reward.rewardType == RewardType.Multiplier)
                new MultiplierRewardEffect().Apply(context);
        } 
}
