using UnityEngine;
using UnityEngine.UI;

public class NPCInteraction : MonoBehaviour
{
    [Header("UI References")]
    public GameObject pressFPopup;
    public GameObject dialoguePanel;
    public GameObject giftSelectionPanel; // Hanya ini yang digunakan

    [Header("Camera References")]
    public Camera mcCamera;
    public Camera npcCamera;

    [Header("Player References")]
    public PlayerMovement playerMovement;
    public PlayerGiftInventory playerInventory;
    public NPCAffinity npcAffinity;

    [Header("Gift Visuals")]
    public Transform giftSpawnPoint;
    public GameObject rosePrefab;
    public GameObject chocolatePrefab;
    public GameObject sundaePrefab;

    [Header("UI Text Elements")]
    public Text roseTextUI;
    public Text chocolateTextUI;
    public Text sundaeTextUI;

    [Header("Settings")]
    public float cameraTransitionSpeed = 5f;

    // Private variables
    private bool playerNearby;
    private bool inDialogue;
    private bool transitioning;
    private bool givingGift;

    private Vector3 transitionStartPos;
    private Quaternion transitionStartRot;
    private Vector3 transitionTargetPos;
    private Quaternion transitionTargetRot;
    private float transitionProgress;

    private Renderer[] playerRenderers;
    private Vector3 npcCameraLocalPos;
    private Quaternion npcCameraLocalRot;

    private float giftTimer;
    private GameObject currentGift;

    void Awake()
    {
        if (npcCamera != null)
        {
            npcCameraLocalPos = npcCamera.transform.localPosition;
            npcCameraLocalRot = npcCamera.transform.localRotation;
            npcCamera.gameObject.SetActive(false);
        }
    }

    void Start()
    {
        // Non-aktifkan SEMUA UI di awal
        if (pressFPopup != null)
            pressFPopup.SetActive(false);

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        if (giftSelectionPanel != null)
            giftSelectionPanel.SetActive(false);

        if (playerMovement != null)
            playerRenderers = playerMovement.GetComponentsInChildren<Renderer>();
    }

    void Update()
    {
        if (playerNearby && !inDialogue && Input.GetKeyDown(KeyCode.F))
        {
            StartDialogue();
        }

        if (inDialogue && !transitioning && !givingGift)
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

        // Handle gift selection input (1,2,3)
        if (giftSelectionPanel != null && giftSelectionPanel.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
                TryGiveGift(GiftType.Rose);
            else if (Input.GetKeyDown(KeyCode.Alpha2))
                TryGiveGift(GiftType.Chocolate);
            else if (Input.GetKeyDown(KeyCode.Alpha3))
                TryGiveGift(GiftType.Sundae);
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                // Kembali ke dialog
                giftSelectionPanel.SetActive(false);
                if (dialoguePanel != null)
                    dialoguePanel.SetActive(true);
            }
        }
    }

    public void OnGiveGift()
    {
        if (givingGift) return;

        if (giftSelectionPanel != null)
        {
            giftSelectionPanel.SetActive(true);
            if (dialoguePanel != null)
                dialoguePanel.SetActive(false);
            UpdateGiftUI();
        }
        else
        {
            Debug.LogError("giftSelectionPanel belum diassign!");
        }
    }

    void UpdateGiftUI()
    {
        if (playerInventory == null) return;

        if (roseTextUI != null)
            roseTextUI.text = "Mawar (You have " + playerInventory.roseCount + ")";

        if (chocolateTextUI != null)
            chocolateTextUI.text = "Coklat (You have " + playerInventory.chocolateCount + ")";

        if (sundaeTextUI != null)
            sundaeTextUI.text = "Sundae (You have " + playerInventory.sundaeCount + ")";
    }

    void TryGiveGift(GiftType type)
    {
        if (playerInventory == null || npcAffinity == null)
        {
            Debug.LogError("PlayerInventory atau NPCAffinity belum diassign!");
            return;
        }

        if (!playerInventory.HasGift(type))
        {
            Debug.Log("You don't have this gift!");
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

        // Start gift giving sequence
        StartGiftSequence(type);
    }

    void StartGiftSequence(GiftType type)
    {
        if (giftSelectionPanel != null)
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

        if (prefab != null && giftSpawnPoint != null)
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
            if (npcCamera != null)
            {
                Vector3 rot = npcCamera.transform.rotation.eulerAngles;
                rot.x += 20f * Time.deltaTime; // Look down gradually
                npcCamera.transform.rotation = Quaternion.Euler(rot);
            }
        }
        // Phase 2: Wait with gift (0.5-2 seconds)
        else if (giftTimer < 2f)
        {
            // Gift stays visible
        }
        // Phase 3: Finish sequence (>2 seconds)
        else
        {
            // Clean up gift
            if (currentGift != null)
                Destroy(currentGift);

            // Reset NPC rotation
            if (npcCamera != null)
            {
                // Reset ke posisi awal kamera NPC
                Transform npcParent = npcCamera.transform.parent;
                if (npcParent != null)
                {
                    Vector3 targetPos = npcParent.TransformPoint(npcCameraLocalPos);
                    Quaternion targetRot = npcParent.rotation * npcCameraLocalRot;
                    npcCamera.transform.position = targetPos;
                    npcCamera.transform.rotation = targetRot;
                }
            }

            givingGift = false;
            Debug.Log("NPC: Thank you for the gift!");

            // Kembali ke dialog atau end dialogue
            OnLeave();
        }
    }

    void StartDialogue()
    {
        inDialogue = true;
        if (pressFPopup != null)
            pressFPopup.SetActive(false);

        if (playerMovement != null)
            playerMovement.canMove = false;

        SetPlayerVisible(false);

        if (npcCamera != null && mcCamera != null)
        {
            Transform npcParent = npcCamera.transform.parent;
            if (npcParent != null)
            {
                transitionTargetPos = npcParent.TransformPoint(npcCameraLocalPos);
                transitionTargetRot = npcParent.rotation * npcCameraLocalRot;

                transitionStartPos = mcCamera.transform.position;
                transitionStartRot = mcCamera.transform.rotation;

                npcCamera.transform.position = transitionStartPos;
                npcCamera.transform.rotation = transitionStartRot;
                npcCamera.gameObject.SetActive(true);
                mcCamera.gameObject.SetActive(false);
            }
        }

        transitionProgress = 0f;
        transitioning = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void EndDialogue()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        if (giftSelectionPanel != null)
            giftSelectionPanel.SetActive(false);

        if (npcCamera != null && mcCamera != null)
        {
            transitionStartPos = npcCamera.transform.position;
            transitionStartRot = npcCamera.transform.rotation;
            transitionTargetPos = mcCamera.transform.position;
            transitionTargetRot = mcCamera.transform.rotation;

            transitionProgress = 0f;
            transitioning = true;
        }

        if (playerMovement != null)
            playerMovement.canMove = true;

        SetPlayerVisible(true);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        inDialogue = false;
        givingGift = false;
    }

    void CameraTransition()
    {
        if (npcCamera == null) return;

        transitionProgress += Time.deltaTime * cameraTransitionSpeed;

        npcCamera.transform.position = Vector3.Lerp(transitionStartPos, transitionTargetPos, transitionProgress);
        npcCamera.transform.rotation = Quaternion.Slerp(transitionStartRot, transitionTargetRot, transitionProgress);

        if (transitionProgress >= 1f)
        {
            transitioning = false;

            if (inDialogue)
            {
                if (dialoguePanel != null)
                    dialoguePanel.SetActive(true);
            }
            else
            {
                if (npcCamera != null)
                    npcCamera.gameObject.SetActive(false);

                if (mcCamera != null)
                    mcCamera.gameObject.SetActive(true);
            }
        }
    }

    void SetPlayerVisible(bool visible)
    {
        if (playerRenderers == null) return;

        foreach (Renderer r in playerRenderers)
            r.enabled = visible;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;
            if (pressFPopup != null)
                pressFPopup.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
            if (pressFPopup != null)
                pressFPopup.SetActive(false);
        }
    }

    public void OnLeave()
    {
        EndDialogue();
    }
}