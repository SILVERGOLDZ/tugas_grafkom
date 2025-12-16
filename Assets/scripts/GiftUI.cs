using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GiftUI : MonoBehaviour
{
    public Button roseButton;
    public Button chocolateButton;
    public Button sundaeButton;

    public Text roseCountText;
    public Text chocolateCountText;
    public Text sundaeCountText;

    private PlayerGiftInventory inventory;
    private System.Action<GiftType> onGiftSelected;

    void Start()
    {
        inventory = FindObjectOfType<PlayerGiftInventory>();

        if (roseButton) roseButton.onClick.AddListener(() => SelectGift(GiftType.Rose));
        if (chocolateButton) chocolateButton.onClick.AddListener(() => SelectGift(GiftType.Chocolate));
        if (sundaeButton) sundaeButton.onClick.AddListener(() => SelectGift(GiftType.Sundae));

        UpdateUI();
    }

    public void Show(System.Action<GiftType> callback)
    {
        gameObject.SetActive(true);
        onGiftSelected = callback;
        UpdateUI();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    void UpdateUI()
    {
        if (!inventory) return;

        if (roseCountText)
            roseCountText.text = inventory.roseCount.ToString();

        if (chocolateCountText)
            chocolateCountText.text = inventory.chocolateCount.ToString();

        if (sundaeCountText)
            sundaeCountText.text = inventory.sundaeCount.ToString();

        // Enable/disable buttons based on inventory
        if (roseButton)
            roseButton.interactable = inventory.roseCount > 0;

        if (chocolateButton)
            chocolateButton.interactable = inventory.chocolateCount > 0;

        if (sundaeButton)
            sundaeButton.interactable = inventory.sundaeCount > 0;
    }

    void SelectGift(GiftType type)
    {
        if (inventory.HasGift(type))
        {
            onGiftSelected?.Invoke(type);
            Hide();
        }
    }

    void Update()
    {
        // Keyboard shortcuts
        if (Input.GetKeyDown(KeyCode.Alpha1) && inventory.roseCount > 0)
            SelectGift(GiftType.Rose);
        else if (Input.GetKeyDown(KeyCode.Alpha2) && inventory.chocolateCount > 0)
            SelectGift(GiftType.Chocolate);
        else if (Input.GetKeyDown(KeyCode.Alpha3) && inventory.sundaeCount > 0)
            SelectGift(GiftType.Sundae);
        else if (Input.GetKeyDown(KeyCode.Escape))
            Hide();
    }
}