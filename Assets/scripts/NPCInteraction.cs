using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    public GameObject pressFPopup;
    public GameObject dialoguePanel;
    public GameObject comingSoonPopup;

    public Camera mcCamera;
    public Camera npcCamera;

    public PlayerMovement playerMovement;

    public float cameraTransitionSpeed = 5f; // slightly faster feels better

    private bool playerNearby;
    private bool inDialogue;
    private bool transitioning;

    private Vector3 transitionStartPos;
    private Quaternion transitionStartRot;
    private Vector3 transitionTargetPos;
    private Quaternion transitionTargetRot;
    private float transitionProgress;

    private Renderer[] playerRenderers;

    // These are the **original local** offsets you set in the editor — never change
    private Vector3 npcCameraLocalPos;
    private Quaternion npcCameraLocalRot;

    public CharacterSpawnManager spawnManager;

    void Awake()
    {
        // Cache ONCE, in Awake — before anything moves
        npcCameraLocalPos = npcCamera.transform.localPosition;
        npcCameraLocalRot = npcCamera.transform.localRotation;

        // Optional: make sure NPC camera is disabled from the start
        npcCamera.gameObject.SetActive(false);
    }

    void Start()
    {
        pressFPopup.SetActive(false);
        dialoguePanel.SetActive(false);
        comingSoonPopup.SetActive(false);

        playerRenderers = playerMovement.GetComponentsInChildren<Renderer>();

        if (spawnManager == null)
            spawnManager = FindObjectOfType<CharacterSpawnManager>();
    }

    void Update()
    {
        if (playerNearby && !inDialogue && Input.GetKeyDown(KeyCode.F))
        {
            StartDialogue();
        }

        if (inDialogue && !transitioning)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                OnLeave();
            }
            if (Input.GetKeyDown(KeyCode.Q))
            {
                OnGiveGift();
            }
        }

        if (transitioning)
        {
            CameraTransition();
        }
    }

    void SetPlayerVisible(bool visible)
    {
        foreach (Renderer r in playerRenderers)
            r.enabled = visible;
    }

    void StartDialogue()
    {
        inDialogue = true;
        pressFPopup.SetActive(false);

        playerMovement.canMove = false;
        SetPlayerVisible(false);

        // Calculate correct target = original local offset applied to current NPC position
        Transform npcParent = npcCamera.transform.parent;
        transitionTargetPos = npcParent.TransformPoint(npcCameraLocalPos);
        transitionTargetRot = npcParent.rotation * npcCameraLocalRot;

        // Start from player's current view
        transitionStartPos = mcCamera.transform.position;
        transitionStartRot = mcCamera.transform.rotation;

        // Snap NPC camera to player view and enable it
        npcCamera.transform.position = transitionStartPos;
        npcCamera.transform.rotation = transitionStartRot;
        npcCamera.gameObject.SetActive(true);
        mcCamera.gameObject.SetActive(false);

        transitionProgress = 0f;
        transitioning = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        CharacterSpawnManager.EnterDialogue();
    }

    void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        comingSoonPopup.SetActive(false);

        // Now transition NPC camera back to player's current view
        transitionStartPos = npcCamera.transform.position;
        transitionStartRot = npcCamera.transform.rotation;
        transitionTargetPos = mcCamera.transform.position;
        transitionTargetRot = mcCamera.transform.rotation;

        transitionProgress = 0f;
        transitioning = true;

        playerMovement.canMove = true;
        SetPlayerVisible(true);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        inDialogue = false;

        CharacterSpawnManager.ExitDialogue();
    }

    void CameraTransition()
    {
        transitionProgress += Time.deltaTime * cameraTransitionSpeed;

        npcCamera.transform.position = Vector3.Lerp(transitionStartPos, transitionTargetPos, transitionProgress);
        npcCamera.transform.rotation = Quaternion.Slerp(transitionStartRot, transitionTargetRot, transitionProgress);

        if (transitionProgress >= 1f)
        {
            transitioning = false;

            if (inDialogue)
            {
                dialoguePanel.SetActive(true);
            }
            else
            {
                npcCamera.gameObject.SetActive(false);
                mcCamera.gameObject.SetActive(true);
            }
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

    public void OnGiveGift() => comingSoonPopup.SetActive(true);
    public void OnLeave() => EndDialogue();
}