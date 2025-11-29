using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems; // FIXED: Required for EventSystem

public class NPCDialog : MonoBehaviour
{
    public Camera npcCamera;
    private CameraTransition camSwitch;
    private PlayerMovement playerMovement;
    private bool playerInRange = false;

    // UI Elements
    private GameObject dialogUI;
    private Text dialogText;
    private Button optionGiftButton;
    private Button optionLeaveButton;
    private GameObject prompt;

    private bool isTalking = false;

    public bool IsTalking => isTalking;

    void Start()
    {
        camSwitch = FindFirstObjectByType<CameraTransition>(); // FIXED: Modern API
        playerMovement = FindFirstObjectByType<PlayerMovement>(); // FIXED: Modern API
        if (playerMovement == null)
            Debug.LogError("NPCDialog: No PlayerMovement found in scene!");

        CreateEventSystem();
        CreatePrompt();
        CreateDialogUI();
        dialogUI.SetActive(false);
        prompt.SetActive(false);
    }

    void Update()
    {
        if (playerInRange && !isTalking && Input.GetKeyDown(KeyCode.F))
        {
            StartConversation();
        }

        bool shouldShowPrompt = playerInRange && !isTalking;
        if (prompt.activeSelf != shouldShowPrompt)
        {
            prompt.SetActive(shouldShowPrompt);
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
        prompt.SetActive(false);
    }

    // ------------------------------
    // EventSystem for UI Buttons
    // ------------------------------
    private void CreateEventSystem()
    {
        if (FindObjectsByType<EventSystem>(FindObjectsSortMode.None).Length == 0) // FIXED: Modern API
        {
            GameObject esObj = new GameObject("EventSystem");
            esObj.AddComponent<EventSystem>();
            esObj.AddComponent<StandaloneInputModule>();
            DontDestroyOnLoad(esObj);
            Debug.Log("NPCDialog: Created EventSystem for UI input.");
        }
    }

    // ------------------------------
    // Prompt UI Creation
    // ------------------------------
    private void CreatePrompt()
    {
        prompt = new GameObject("InteractPrompt");
        Canvas canvas = prompt.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 1;

        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(prompt.transform);
        Text txt = textObj.AddComponent<Text>();
        txt.text = "Press F to Talk";
        txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        txt.fontSize = 32;
        txt.color = Color.white;
        txt.alignment = TextAnchor.MiddleCenter;

        RectTransform rect = textObj.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(400, 100);
        rect.anchoredPosition = new Vector2(0, -200);
    }

    // ------------------------------
    // Dialog UI Creation
    // ------------------------------
    private void CreateDialogUI()
    {
        dialogUI = new GameObject("DialogCanvas");
        Canvas canvas = dialogUI.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 10;
        dialogUI.AddComponent<CanvasScaler>();
        dialogUI.AddComponent<GraphicRaycaster>();

        GameObject panelObj = new GameObject("Panel");
        panelObj.transform.SetParent(dialogUI.transform);
        Image panelImage = panelObj.AddComponent<Image>();
        panelImage.color = new Color(0, 0, 0, 0.6f);

        RectTransform panelRect = panelObj.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.2f, 0.1f);
        panelRect.anchorMax = new Vector2(0.8f, 0.35f);
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

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

        optionGiftButton = CreateButton(panelObj.transform, "Give Gift", new Vector2(0.25f, 0.2f));
        optionGiftButton.onClick.AddListener(OnGiveGift);

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

    public void StartConversation()
    {
        isTalking = true;
        camSwitch.StartTransitionTo(npcCamera);

        if (playerMovement != null)
            playerMovement.enabled = false;

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

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        if (playerMovement != null)
            playerMovement.enabled = true;

        camSwitch.ReturnToMC();
        isTalking = false;
    }
}