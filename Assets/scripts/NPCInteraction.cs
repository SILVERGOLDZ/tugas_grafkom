using UnityEngine;
using UnityEngine.UI;

public class NPCInteraction : MonoBehaviour
{
    public GameObject pressFPopup;
    public GameObject dialoguePanel;
    public GameObject giftSelectionPanel;

    public Camera mcCamera;
    public Camera npcCamera;

    public PlayerMovement playerMovement;

    public float cameraTransitionSpeed = 5f;

    private bool playerNearby;
    private bool inDialogue;
    private bool transitioning;

    private Vector3 transitionStartPos;
    private Quaternion transitionStartRot;
    private Vector3 transitionTargetPos;
    private Quaternion transitionTargetRot;
    private float transitionProgress;

    private Renderer[] playerRenderers;

    // Gift System References
    public PlayerGiftInventory playerInventory;
    public NPCAffinity npcAffinity;

    // UI References
    public Text roseText;
    public Text chocolateText;
    public Text sundaeText;
    public Text affinityText;

    // Gift Visuals
    public Transform giftSpawnPoint;
    public GameObject rosePrefab;
    public GameObject chocolatePrefab;
    public GameObject sundaePrefab;

    // Gift Giving Sequence
    private bool givingGift;
    private float giftTimer;
    private GameObject currentGift;

    // Camera Original Positions
    private Vector3 npcCameraLocalPos;
    private Quaternion npcCameraLocalRot;

    void Awake()
    {
        npcCameraLocalPos = npcCamera.transform.localPosition;
        npcCameraLocalRot = npcCamera.transform.localRotation;
        npcCamera.gameObject.SetActive(false);
    }

    void Start()
    {
        pressFPopup.SetActive(false);
        dialoguePanel.SetActive(false);
        giftSelectionPanel.SetActive(false);

        playerRenderers = playerMovement.GetComponentsInChildren<Renderer>();
        UpdateGiftUI();
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

        if (givingGift)
        {
            HandleGiftSequence();
        }
    }

    void StartDialogue()
    {
        inDialogue = true;
        pressFPopup.SetActive(false);

        playerMovement.canMove = false;
        SetPlayerVisible(false);

        Transform npcParent = npcCamera.transform.parent;
        transitionTargetPos = npcParent.TransformPoint(npcCameraLocalPos);
        transitionTargetRot = npcParent.rotation * npcCameraLocalRot;

        transitionStartPos = mcCamera.transform.position;
        transitionStartRot = mcCamera.transform.rotation;

        npcCamera.transform.position = transitionStartPos;
        npcCamera.transform.rotation = transitionStartRot;
        npcCamera.gameObject.SetActive(true);
        mcCamera.gameObject.SetActive(false);

        transitionProgress = 0f;
        transitioning = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        UpdateAffinityUI();
    }

    void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        giftSelectionPanel.SetActive(false);

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
        givingGift = false;
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

    // GIFT SYSTEM FUNCTIONS
    public void OnGiveGift()
    {
        if (givingGift) return;

        giftSelectionPanel.SetActive(true);
        dialoguePanel.SetActive(false);
        UpdateGiftUI();

        // Start listening for number key presses
        StartCoroutine(GiftSelectionInput());
    }

    System.Collections.IEnumerator GiftSelectionInput()
    {
        bool waitingForInput = true;

        while (waitingForInput && giftSelectionPanel.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                TryGiveGift(GiftType.Rose);
                waitingForInput = false;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                TryGiveGift(GiftType.Chocolate);
                waitingForInput = false;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                TryGiveGift(GiftType.Sundae);
                waitingForInput = false;
            }
            else if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.E))
            {
                giftSelectionPanel.SetActive(false);
                dialoguePanel.SetActive(true);
                waitingForInput = false;
            }

            yield return null;
        }
    }

    void TryGiveGift(GiftType type)
    {
        if (!playerInventory.HasGift(type))
        {
            Debug.Log("You don't have this gift!");
            giftSelectionPanel.SetActive(false);
            dialoguePanel.SetActive(true);
            return;
        }

        // Consume gift
        playerInventory.ConsumeGift(type);
        UpdateGiftUI();

        // Calculate points
        int points = type switch
        {
            GiftType.Rose => 10,
            GiftType.Chocolate => 50,
            GiftType.Sundae => 100,
            _ => 0
        };

        // Add affinity points
        npcAffinity.AddPoints(points);
        UpdateAffinityUI();

        // Start gift giving sequence
        StartGiftSequence(type);
    }

    void StartGiftSequence(GiftType type)
    {
        giftSelectionPanel.SetActive(false);
        givingGift = true;
        giftTimer = 0f;

        // Spawn gift visual
        GameObject prefab = type switch
        {
            GiftType.Rose => rosePrefab,
            GiftType.Chocolate => chocolatePrefab,
            GiftType.Sundae => sundaePrefab,
            _ => null
        };

        if (prefab && giftSpawnPoint)
        {
            currentGift = Instantiate(prefab, giftSpawnPoint.position, Quaternion.identity);
            currentGift.transform.localScale = Vector3.one * 0.3f;
        }
    }

    void HandleGiftSequence()
    {
        giftTimer += Time.deltaTime;

        // Phase 1: NPC looks down (0-0.5 seconds)
        if (giftTimer < 0.5f)
        {
            Vector3 rot = npcCamera.transform.rotation.eulerAngles;
            rot.x += 20f * Time.deltaTime; // Look down gradually
            transform.rotation = Quaternion.Euler(rot);
        }
        // Phase 2: Wait with gift (0.5-2 seconds)
        else if (giftTimer < 2f)
        {
            // Gift stays visible
        }
        // Phase 3: Finish sequence (>2 seconds)
        else
        {
            // Clean up
            if (currentGift)
                Destroy(currentGift);

            // Reset NPC rotation
            transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);

            // Show thank you message
            Debug.Log("NPC: Thank you for the gift!");

            // End dialogue
            EndDialogue();
        }
    }

    void UpdateGiftUI()
    {
        if (roseText)
            roseText.text = "Mawar (You have " + playerInventory.roseCount + ")";
        if (chocolateText)
            chocolateText.text = "Coklat (You have " + playerInventory.chocolateCount + ")";
        if (sundaeText)
            sundaeText.text = "Sundae (You have " + playerInventory.sundaeCount + ")";
    }

    void UpdateAffinityUI()
    {
        if (affinityText && npcAffinity)
        {
            affinityText.text = "Level " + npcAffinity.affinityLevel +
                              " (" + npcAffinity.currentPoints + "/" +
                              GetNextLevelThreshold() + " points)";
        }
    }

    int GetNextLevelThreshold()
    {
        if (npcAffinity.affinityLevel >= 4) return 0;

        int[] thresholds = { 100, 250, 500, 1000 };
        return thresholds[npcAffinity.affinityLevel];
    }

    void SetPlayerVisible(bool visible)
    {
        foreach (Renderer r in playerRenderers)
            r.enabled = visible;
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

    public void OnLeave()
    {
        EndDialogue();
    }
}