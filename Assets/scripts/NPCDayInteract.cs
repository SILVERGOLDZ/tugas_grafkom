using UnityEngine;
using TMPro;

public class NPCDayInteract : MonoBehaviour
{
    [Header("UI")]
    public GameObject pressFPopup;
    public TextMeshProUGUI pressFText;

    [Header("NPC Renderer")]
    public Renderer[] npcRenderers;

    [Header("Day Cycle")]
    public DayCycleManager dayCycle;

    private bool playerNearby;

    void Start()
    {
        pressFPopup.SetActive(false);
        SetInteractStrength(0f);
    }

    void Update()
    {
        if (playerNearby && Input.GetKeyDown(KeyCode.F))
        {
            dayCycle.NextTime(); // ganti waktu
        }
    }

    void SetInteractStrength(float value)
    {
        foreach (Renderer r in npcRenderers)
        {
            r.material.SetFloat("_InteractStrength", value);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;

            pressFPopup.SetActive(true);
            pressFText.text = "Press F to change time";

            SetInteractStrength(1f); // TERANG
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;

            pressFPopup.SetActive(false);
            SetInteractStrength(0f); // NORMAL
        }
    }
}
