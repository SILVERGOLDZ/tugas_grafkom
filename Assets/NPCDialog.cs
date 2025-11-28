using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NPCDialog : MonoBehaviour
{
    public Camera npcCamera;
    private CameraTransition camSwitch;
    private bool playerInRange = false;

    // UI Elements
    private GameObject dialogUI;
    private Text dialogText;
    private Button optionGiftButton;
    private Button optionLeaveButton;

    private bool isTalking = false;

    void Start()
    {
        camSwitch = FindFirstObjectByType<CameraTransition>();

        CreateDialogUI();
        dialogUI.SetActive(false);
    }

    void Update()
    {
        if (playerInRange && !isTalking && Input.GetKeyDown(KeyCode.F))
        {
            StartConversation();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.root.CompareTag("Player"))
            playerInRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.root.CompareTag("Player"))
            playerInRange = false;
    }

    // ------------------------------
    // UI Creation
    // ------------------------------
    private void CreateDialogUI()
    {
        // Canvas
        dialogUI = new GameObject("DialogCanvas");
        Canvas canvas = dialogUI.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        dialogUI.AddComponent<CanvasScaler>();
        dialogUI.AddComponent<GraphicRaycaster>();

        // Background panel
        GameObject panelObj = new GameObject("Panel");
        panelObj.transform.SetParent(dialogUI.transform);
        Image panelImage = panelObj.AddComponent<Image>();
        panelImage.color = new Color(0, 0, 0, 0.6f);

        RectTransform panelRect = panelObj.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.2f, 0.1f);
        panelRect.anchorMax = new Vector2(0.8f, 0.35f);
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        // Dialog text
        GameObject textObj = new GameObject("DialogText");
        textObj.transform.SetParent(panelObj.transform);
        dialogText = textObj.AddComponent<Text>();
        dialogText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        dialogText.fontSize = 28;
        dialogText.alignment = TextAnchor.MiddleCenter;
        dialogText.color = Color.white;

        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0.1f, 0.55f);
        textRect.anchorMax = new Vector2(0.9f, 0.95f);
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        // Button: Give Gift
        optionGiftButton = CreateButton(panelObj.transform, "Give Gift", new Vector2(0.25f, 0.2f));
        optionGiftButton.onClick.AddListener(OnGiveGift);

        // Button: Leave
        optionLeaveButton = CreateButton(panelObj.transform, "Leave", new Vector2(0.75f, 0.2f));
        optionLeaveButton.onClick.AddListener(OnLeave);
    }

    private Button CreateButton(Transform parent, string label, Vector2 anchorPos)
    {
        GameObject btnObj = new GameObject(label + "Button");
        btnObj.transform.SetParent(parent);

        Button button = btnObj.AddComponent<Button>();
        Image img = btnObj.AddComponent<Image>();
        img.color = new Color(1f, 1f, 1f, 0.8f);

        RectTransform rect = btnObj.GetComponent<RectTransform>();
        rect.anchorMin = anchorPos - new Vector2(0.15f, 0.15f);
        rect.anchorMax = anchorPos + new Vector2(0.15f, 0.15f);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        // Label
        GameObject txtObj = new GameObject("Text");
        txtObj.transform.SetParent(btnObj.transform);
        Text txt = txtObj.AddComponent<Text>();
        txt.text = label;
        txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        txt.fontSize = 26;
        txt.alignment = TextAnchor.MiddleCenter;
        txt.color = Color.black;

        RectTransform txtRect = txtObj.GetComponent<RectTransform>();
        txtRect.anchorMin = Vector2.zero;
        txtRect.anchorMax = Vector2.one;
        txtRect.offsetMin = Vector2.zero;
        txtRect.offsetMax = Vector2.zero;

        return button;
    }

    // ------------------------------
    // Conversation Logic
    // ------------------------------
    private void StartConversation()
    {
        isTalking = true;
        camSwitch.StartTransitionTo(npcCamera);

        // Unlock mouse for UI
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        dialogUI.SetActive(true);
        dialogText.text = "Hello. What do you want to do?";
    }

    private void OnGiveGift()
    {
        dialogText.text = "Coming soon.";
    }

    private void OnLeave()
    {
        StartCoroutine(LeaveRoutine());
    }

    private IEnumerator LeaveRoutine()
    {
        dialogText.text = "Okay, take care...";
        yield return new WaitForSeconds(1.2f);

        dialogText.text = "See you next time.";
        yield return new WaitForSeconds(1.2f);

        dialogUI.SetActive(false);

        // Lock the cursor back for FPS movement
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        camSwitch.ReturnToMC();
        isTalking = false;
    }
}
