using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    public GameObject pressFPopup;
    public GameObject dialoguePanel;
    public GameObject comingSoonPopup;

    public Camera mcCamera;
    public Camera npcCamera;

    public PlayerMovement playerMovement;
    public GameObject playerModel; // mesh MC saja

    public float cameraTransitionSpeed = 3f;

    private bool playerNearby;
    private bool inDialogue;
    private bool transitioning;
    private Camera activeFromCam;
    private Camera activeToCam;
    private float transitionProgress;
    private Renderer[] playerRenderers;

    // Reference to the shared manager (assign in Inspector or find it)
    public CharacterSpawnManager spawnManager;

    void Start()
    {
        pressFPopup.SetActive(false);
        dialoguePanel.SetActive(false);
        comingSoonPopup.SetActive(false);

        npcCamera.gameObject.SetActive(false);

        // Ambil SEMUA renderer player (Mesh + Skinned)
        playerRenderers = playerMovement.GetComponentsInChildren<Renderer>();

        // If not assigned in Inspector, find it automatically
        if (spawnManager == null)
            spawnManager = FindObjectOfType<CharacterSpawnManager>();
    }

    void Update()
    {
        if (playerNearby && !inDialogue && Input.GetKeyDown(KeyCode.F))
        {
            StartDialogue();
        }

        if (transitioning)
        {
            CameraTransition();
        }
    }

    void SetPlayerVisible(bool visible)
    {
        foreach (Renderer r in playerRenderers)
        {
            r.enabled = visible;
        }
    }

    void StartDialogue()
    {
        inDialogue = true;
        pressFPopup.SetActive(false);

        playerMovement.canMove = false;
        SetPlayerVisible(false);

        activeFromCam = mcCamera;
        activeToCam = npcCamera;

        npcCamera.gameObject.SetActive(true);
        transitionProgress = 0f;
        transitioning = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Disable NPC spawn/despawn changes during dialogue
        CharacterSpawnManager.EnterDialogue();
    }

    void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        comingSoonPopup.SetActive(false);

        activeFromCam = npcCamera;
        activeToCam = mcCamera;

        transitionProgress = 0f;
        transitioning = true;

        playerMovement.canMove = true;
        SetPlayerVisible(true);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        inDialogue = false;

        // Re-enable NPC spawn/despawn changes
        CharacterSpawnManager.ExitDialogue();
    }

    void CameraTransition()
    {
        transitionProgress += Time.deltaTime * cameraTransitionSpeed;

        activeFromCam.transform.position = Vector3.Lerp(
            activeFromCam.transform.position,
            activeToCam.transform.position,
            transitionProgress
        );

        activeFromCam.transform.rotation = Quaternion.Slerp(
            activeFromCam.transform.rotation,
            activeToCam.transform.rotation,
            transitionProgress
        );

        if (transitionProgress >= 1f)
        {
            transitioning = false;
            activeFromCam.gameObject.SetActive(false);

            if (activeToCam == npcCamera)
                dialoguePanel.SetActive(true);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;
            pressFPopup.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
            pressFPopup.SetActive(false);
        }
    }

    // ===== UI BUTTON =====
    public void OnGiveGift()
    {
        comingSoonPopup.SetActive(true);
    }

    public void OnLeave()
    {
        EndDialogue();
    }
}