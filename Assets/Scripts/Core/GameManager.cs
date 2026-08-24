using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private HUDController hud;
    [SerializeField] private InventoryUIController inventoryUI;
    [SerializeField] private WheelController wheel;
    [SerializeField] private WheelUIController wheelUI;
    [SerializeField] private ResultUIController resultUI;
    [SerializeField] private BombReviveUIController bombReviveUI;
    [SerializeField] private LeaveSuccessUIController leaveSuccessUI;
    [SerializeField] private WheelConfigSO normalConfig;
    [SerializeField] private WheelConfigSO safeConfig;
    [SerializeField] private WheelConfigSO superConfig;
    [SerializeField] private string reviveCurrencyId = "gold";

    private ZoneManager _zones;
    private GameStateMachine _state;
    private RewardManager _rewards;
    private WheelResolver _resolver;

    private void Awake()
    {
        _zones = new ZoneManager();
        _state = new GameStateMachine();
        _rewards = new RewardManager();
        _resolver = new WheelResolver();
        RefreshHud();
        RefreshButtons();
    }

    private void OnEnable()
    {
        WireBombEvents(true);
        WireLeaveEvents(true);
    }

    private void OnDisable()
    {
        WireBombEvents(false);
        WireLeaveEvents(false);
    }

    private void Start()
    {
        if (bombReviveUI == null)
            bombReviveUI = FindObjectOfType<BombReviveUIController>(true);
        if (leaveSuccessUI == null)
            leaveSuccessUI = FindObjectOfType<LeaveSuccessUIController>(true);

        WireBombEvents(false);
        WireBombEvents(true);
        WireLeaveEvents(false);
        WireLeaveEvents(true);

        if (bombReviveUI != null)
            bombReviveUI.Hide();
        if (leaveSuccessUI != null)
            leaveSuccessUI.Hide();

        ApplyCurrentZoneWheel();
        RefreshHud();
        RefreshButtons();
    }

    private void WireBombEvents(bool subscribe)
    {
        if (bombReviveUI == null)
            return;
        if (subscribe)
        {
            bombReviveUI.GiveUpClicked += OnBombGiveUp;
            bombReviveUI.ReviveClicked += OnBombRevive;
        }
        else
        {
            bombReviveUI.GiveUpClicked -= OnBombGiveUp;
            bombReviveUI.ReviveClicked -= OnBombRevive;
        }
    }

    private void WireLeaveEvents(bool subscribe)
    {
        if (leaveSuccessUI == null)
            return;
        if (subscribe)
            leaveSuccessUI.ContinueClicked += OnLeaveContinue;
        else
            leaveSuccessUI.ContinueClicked -= OnLeaveContinue;
    }

    private void RefreshHud()
    {
        if (hud != null)
            hud.RefreshZone(_zones.CurrentZone);

        if (inventoryUI != null)
            inventoryUI.Refresh(_rewards.Entries);
    }

    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Space))
            return;

        if (_state.CurrentState == GameState.Idle)
        {
            TrySpin();
            return;
        }

        if (_state.CurrentState == GameState.GameOver)
        {
            OnBombGiveUp();
            return;
        }

        if (_state.CurrentState == GameState.Ended)
            OnLeaveContinue();
    }

    private void RestartAfterBomb()
    {
        _rewards.ClearAll();
        _zones.Reset();
        _state.TryStateTransition(GameState.Idle);
        Debug.Log("Restart → Zone 1, rewards cleared");
        if (resultUI != null)
            resultUI.Hide();
        if (bombReviveUI != null)
            bombReviveUI.Hide();
        if (leaveSuccessUI != null)
            leaveSuccessUI.Hide();
        RefreshHud();
        RefreshButtons();
        ApplyCurrentZoneWheel();
    }

    private void OnBombGiveUp()
    {
        if (_state.CurrentState != GameState.GameOver)
            return;
        RestartAfterBomb();
    }

    private void OnBombRevive()
    {
        if (_state.CurrentState != GameState.GameOver)
            return;

        int cost = bombReviveUI != null ? bombReviveUI.ReviveCost : 25;
        if (!_rewards.TrySpend(reviveCurrencyId, cost))
        {
            Debug.LogWarning($"Revive failed — need {cost} {reviveCurrencyId}");
            if (bombReviveUI != null)
                bombReviveUI.Show(_rewards.GetAmount(reviveCurrencyId));
            return;
        }

        if (!_state.TryStateTransition(GameState.Idle))
            return;

        if (bombReviveUI != null)
            bombReviveUI.Hide();
        if (resultUI != null)
            resultUI.Hide();

        Debug.Log($"REVIVE OK | paid {cost} {reviveCurrencyId} | Zone {_zones.CurrentZone} | Entries {_rewards.Entries.Count}");
        RefreshHud();
        RefreshButtons();
        ApplyCurrentZoneWheel();
    }

    private void OnLeaveContinue()
    {
        if (_state.CurrentState != GameState.Ended)
            return;

        _rewards.ClearAll();
        _zones.Reset();
        if (!_state.TryStateTransition(GameState.Idle))
            return;

        if (leaveSuccessUI != null)
            leaveSuccessUI.Hide();
        if (resultUI != null)
            resultUI.Hide();

        Debug.Log("Leave CONTINUE → new run Zone 1");
        RefreshHud();
        RefreshButtons();
        ApplyCurrentZoneWheel();
    }

    private void TrySpin()
    {
        if (wheel != null && wheel.IsSpinning)
            return;

        if (resultUI != null)
            resultUI.Hide();
        if (bombReviveUI != null)
            bombReviveUI.Hide();
        if (leaveSuccessUI != null)
            leaveSuccessUI.Hide();

        if (!_state.TryStateTransition(GameState.Spinning))
            return;

        WheelConfigSO config = GetConfigForZone();
        WheelSliceData slice = _resolver.Resolve(config);

        if (slice == null)
        {
            Debug.LogWarning("No slice resolved");
            _state.TryStateTransition(GameState.Idle);
            return;
        }

        int index = config.slices.IndexOf(slice);
        if (index < 0)
        {
            Debug.LogWarning("Resolved slice not in config");
            _state.TryStateTransition(GameState.Idle);
            return;
        }

        if (wheel == null)
        {
            ApplySpinResult(slice, index);
            return;
        }

        RefreshButtons();
        wheel.SpinToIndex(index, () => ApplySpinResult(slice, index));
    }

    private int ApplyReward(WheelSliceData slice, Sprite icon)
    {
        if (slice.reward == null)
            return 0;

        int amount = slice.reward.RollAmount(_zones.CurrentZone);
        var context = new RewardContext(_rewards, slice.reward, amount, icon);

        if (slice.reward.rewardType == RewardType.Currency)
            new CurrencyRewardEffect().Apply(context);
        else if (slice.reward.rewardType == RewardType.SpecialItem)
            new SpecialItemRewardEffect().Apply(context);
        else if (slice.reward.rewardType == RewardType.Multiplier)
            new MultiplierRewardEffect().Apply(context);

        return amount;
    }

    private void ApplySpinResult(WheelSliceData slice, int slotIndex)
    {
        if (!_state.TryStateTransition(GameState.Result))
            return;

        if (slice.isBomb)
        {
            if (resultUI != null)
                resultUI.Hide();

            _state.TryStateTransition(GameState.GameOver);
            Debug.Log("BOMB HIT | rewards kept until Give Up | Zone " + _zones.CurrentZone);

            if (bombReviveUI != null)
                bombReviveUI.Show(_rewards.GetAmount(reviveCurrencyId));

            RefreshHud();
            RefreshButtons();
            return;
        }

        Sprite icon = null;
        if (wheel != null)
            icon = wheel.GetSlotIcon(slotIndex);
        if (icon == null && slice.reward != null)
            icon = slice.reward.PickRandomIcon();

        int rolled = ApplyReward(slice, icon);

        if (resultUI != null)
            resultUI.ShowReward(icon, rolled);

        _zones.AdvanceZone();
        _state.TryStateTransition(GameState.Idle);

        Debug.Log(
            $"Reward OK | Zone {_zones.CurrentZone} ({_zones.CurrentType}) | Entries {_rewards.Entries.Count}");
        RefreshHud();
        RefreshButtons();
        ApplyCurrentZoneWheel();
    }

    public void RequestSpin()
    {
        if (_state.CurrentState != GameState.Idle)
            return;

        TrySpin();
        RefreshButtons();
    }

    public void RequestLeave()
    {
        if (_state.CurrentState != GameState.Idle)
            return;
        if (!_zones.IsSafeZone && !_zones.IsSuperZone)
            return;
        if (wheel != null && wheel.IsSpinning)
            return;
        if (!_state.TryStateTransition(GameState.Ended))
            return;

        if (resultUI != null)
            resultUI.Hide();
        if (bombReviveUI != null)
            bombReviveUI.Hide();

        Debug.Log($"LEFT WITH REWARDS | Zone {_zones.CurrentZone} | Entries {_rewards.Entries.Count}");

        if (leaveSuccessUI != null)
            leaveSuccessUI.Show(_rewards.Entries, _zones.CurrentZone);

        RefreshHud();
        RefreshButtons();
    }

    private void RefreshButtons()
    {
        if (wheelUI == null)
            return;

        bool idle = _state.CurrentState == GameState.Idle;
        bool spinning = wheel != null && wheel.IsSpinning;
        bool canSpin = idle && !spinning;
        bool canLeave = idle && !spinning && (_zones.IsSafeZone || _zones.IsSuperZone);
        wheelUI.Refresh(canSpin, canLeave);
    }

    private WheelConfigSO GetConfigForZone()
    {
        if (_zones.IsSuperZone)
            return superConfig != null ? superConfig : normalConfig;
        if (_zones.IsSafeZone)
            return safeConfig != null ? safeConfig : normalConfig;
        return normalConfig;
    }

    private void ApplyCurrentZoneWheel()
    {
        WheelConfigSO config = GetConfigForZone();
        if (wheel == null || config == null)
            return;
        wheel.BuildSlots(config);
        wheel.ApplyZoneLook(_zones.CurrentType);
    }

    [ContextMenu("Debug Jump To Zone 5")]
    private void DebugJumpToZone5()
    {
        if (!Application.isPlaying)
            return;
        _zones.Reset();
        while (_zones.CurrentZone < 5)
            _zones.AdvanceZone();
        RefreshHud();
        RefreshButtons();
        Debug.Log("Debug -> Zone 5 (Safe). Leave should be active.");
        ApplyCurrentZoneWheel();
    }
}
