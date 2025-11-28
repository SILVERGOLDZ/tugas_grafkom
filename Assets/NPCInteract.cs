using UnityEngine;

public class NPCInteract : MonoBehaviour
{
    private GameObject prompt;
    public Camera npcCamera;             // NPC’s face camera
    private CameraTransition camSwitch;  // The transition manager

    private bool playerInRange = false;

    void Start()
    {
        camSwitch = FindFirstObjectByType<CameraTransition>();

        // Create a simple on-screen prompt
        prompt = new GameObject("InteractPrompt");
        var canvas = prompt.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        var textObj = new GameObject("Text");
        textObj.transform.SetParent(prompt.transform);

        var txt = textObj.AddComponent<UnityEngine.UI.Text>();
        txt.text = "Press F to Talk";
        txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        txt.alignment = TextAnchor.MiddleCenter;
        txt.fontSize = 32;

        RectTransform rect = textObj.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(400, 100);
        rect.anchoredPosition = new Vector2(0, -200);

        prompt.SetActive(false);
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.F))
        {
            camSwitch.StartTransitionTo(npcCamera);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.root.CompareTag("Player"))
        {
            Debug.Log("Player entered NPC trigger!");
            playerInRange = true;
            prompt.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.root.CompareTag("Player"))
        {
            Debug.Log("Player exited NPC trigger!");
            playerInRange = false;
            prompt.SetActive(false);
        }
    }
}