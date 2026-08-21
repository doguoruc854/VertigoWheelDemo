using UnityEngine;
using UnityEngine.UI;

public class WheelUIController : MonoBehaviour
{
    [SerializeField] private Button spinButton;
    [SerializeField] private Button leaveButton;
    [SerializeField] private GameManager gameManager;

    private void OnValidate()
    {
        if (spinButton == null || leaveButton == null)
        {
            var buttons = GetComponentsInChildren<Button>(true);
            for (int i = 0; i < buttons.Length; i++)
            {
                if (buttons[i].name == "ui_button_spin_main")
                    spinButton = buttons[i];
                if (buttons[i].name == "ui_button_leave_main")
                    leaveButton = buttons[i];
            }
        }
    }

    private void OnEnable()
    {
        if (spinButton != null)
            spinButton.onClick.AddListener(OnSpinClicked);
        if (leaveButton != null)
            leaveButton.onClick.AddListener(OnLeaveClicked);
    }

    private void OnDisable()
    {
        if (spinButton != null)
            spinButton.onClick.RemoveListener(OnSpinClicked);
        if (leaveButton != null)
            leaveButton.onClick.RemoveListener(OnLeaveClicked);
    }

    private void OnSpinClicked()
    {
        if (gameManager != null)
            gameManager.RequestSpin();
    }

    private void OnLeaveClicked()
    {
        if (gameManager != null)
            gameManager.RequestLeave();
    }

    public void Refresh(bool canSpin, bool canLeave)
    {
        if (spinButton != null)
            spinButton.interactable = canSpin;
        if (leaveButton != null)
            leaveButton.interactable = canLeave;
    }
}