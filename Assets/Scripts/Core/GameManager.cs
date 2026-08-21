using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private WheelConfigSO wheelConfig;
    [SerializeField] private HUDController hud;
    [SerializeField] private WheelController wheel;
    [SerializeField] private WheelUIController wheelUI;

    private ZoneManager _zones;
    private GameStateMachine _state;
    private RewardManager _rewards;
    private WheelResolver _resolver;

    private void Awake() {
        _zones = new ZoneManager();
        _state = new GameStateMachine();
        _rewards = new RewardManager();
        _resolver = new WheelResolver();
        RefreshHud();
        RefreshButtons();

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
    RefreshButtons();
}

    private void TrySpin()
{
    if (wheel != null && wheel.IsSpinning)
        return;

    if (!_state.TryStateTransition(GameState.Spinning))
        return;

    WheelSliceData slice = _resolver.Resolve(wheelConfig);

    if (slice == null)
    {
        Debug.LogWarning("No slice resolved");
        _state.TryStateTransition(GameState.Idle);
        return;
    }

    int index = wheelConfig.slices.IndexOf(slice);
    if (index < 0)
    {
        Debug.LogWarning("Resolved slice not in config");
        _state.TryStateTransition(GameState.Idle);
        return;
    }

    if (wheel == null)
    {
        ApplySpinResult(slice);
        return;
    }
    RefreshButtons();
    wheel.SpinToIndex(index, () => ApplySpinResult(slice));
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

        private void ApplySpinResult(WheelSliceData slice){
            if (!_state.TryStateTransition(GameState.Result))
            return;

            if (slice.isBomb){

                _rewards.ClearAll();
                _state.TryStateTransition(GameState.GameOver);
                Debug.Log("BOMB HIT GAME OVER | Zone " + _zones.CurrentZone + " | Total " + _rewards.TotalCurrency);
                RefreshHud();
                return;
            }

            ApplyReward(slice);
            _zones.AdvanceZone();
            _state.TryStateTransition(GameState.Idle);

            Debug.Log(
                $"Reward OK | Zone {_zones.CurrentZone} ({_zones.CurrentType}) | Total {_rewards.TotalCurrency}"
            );
            RefreshHud();
            RefreshButtons();
        }
        
        public void RequestSpin(){
            if(_state.CurrentState != GameState.Idle)
                return;
            TrySpin();
            RefreshButtons();
        }
        
        public void RequestLeave(){
            if(_state.CurrentState != GameState.Idle)
                return;
            if(!_zones.IsSafeZone && !_zones.IsSuperZone)
                return;
            if(wheel != null && wheel.IsSpinning)
                return;
            if(!_state.TryStateTransition(GameState.GameOver))
                return;
            Debug.Log($"LEFT WITH REWARDS | Zone {_zones.CurrentZone} | Total {_rewards.TotalCurrency}");
            RefreshHud();
            RefreshButtons();
        }

        private void RefreshButtons(){
            if (wheelUI == null)
                return;

            bool idle = _state.CurrentState == GameState.Idle;
            bool spinning = wheel != null && wheel.IsSpinning;
            bool canSpin = idle && !spinning;
            bool canLeave = idle && !spinning && (_zones.IsSafeZone || _zones.IsSuperZone);
            wheelUI.Refresh(canSpin, canLeave);
        }

        [ContextMenu("Debug Jump To Zone 5")]
        private void DebugJumpToZone5(){
            if (!Application.isPlaying)
                return;
            _zones.Reset();
            while (_zones.CurrentZone < 5)
                _zones.AdvanceZone();
            RefreshHud();
            RefreshButtons();
            Debug.Log("Debug -> Zone 5 (Safe). Leave should be active.");
        }
}   
