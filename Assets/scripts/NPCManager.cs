using UnityEngine;

public class CharacterSpawnManager : MonoBehaviour
{
    public DayCycleManager dayCycle;

    [Header("Characters")]
    public CharacterAppear alice;
    public CharacterAppear nozomi;
    public CharacterAppear hikari;
    public CharacterAppear miyu;
    public CharacterAppear miyako;

    // Static counter to track active dialogues (shared across all NPCs)
    private static int activeDialogueCount = 0;

    private void Start()
    {
        dayCycle.OnTimeChanged += HandleTimeChange;
        HandleTimeChange(dayCycle.currentTime); // apply waktu awal
    }

    private void OnDestroy()
    {
        // Safety: unsubscribe to avoid leaks
        dayCycle.OnTimeChanged -= HandleTimeChange;
    }

    // Di CharacterSpawnManager.cs, modifikasi HandleTimeChange:
    void HandleTimeChange(DayCycleManager.TimeOfDay time)
    {
        Debug.Log($"CharacterSpawnManager: Time changed to {time}");

        // Only apply changes if no dialogue is active
        if (activeDialogueCount > 0)
        {
            Debug.Log($"Dialogue active ({activeDialogueCount}), skipping spawn changes");
            return;
        }

        // Nonaktifkan SEMUA NPC dulu
        DisableAllNPCs();

        // Aktifkan sesuai waktu
        switch (time)
        {
            case DayCycleManager.TimeOfDay.Morning:
                SafeAppear(alice);
                SafeAppear(hikari);
                Debug.Log("Morning NPCs activated");
                break;

            case DayCycleManager.TimeOfDay.Noon:
                SafeAppear(nozomi);
                Debug.Log("Noon NPCs activated");
                break;

            case DayCycleManager.TimeOfDay.Evening:
                SafeAppear(miyu);
                Debug.Log("Evening NPCs activated");
                break;

            case DayCycleManager.TimeOfDay.Night:
                SafeAppear(miyako);
                Debug.Log("Night NPCs activated");
                break;
        }
    }

    void DisableAllNPCs()
    {

        // Nonaktifkan semua CharacterAppear
        CharacterAppear[] allCharacters = FindObjectsOfType<CharacterAppear>(true);
        foreach (CharacterAppear character in allCharacters)
        {
            if (character != null && character.gameObject.activeSelf)
            {
                character.Disappear();
            }
        }

        // Nonaktifkan semua NPCInteraction colliders
        NPCInteraction[] allNPCs = FindObjectsOfType<NPCInteraction>(true);
        foreach (NPCInteraction npc in allNPCs)
        {
            if (npc != null)
            {
                Collider col = npc.GetComponent<Collider>();
                if (col != null) col.enabled = false;

                if (npc.pressFPopup != null)
                    npc.pressFPopup.SetActive(false);
            }
        }
    }

    void SafeAppear(CharacterAppear character)
    {
        if (character == null)
        {
            Debug.LogWarning("Tried to appear null character");
            return;
        }

        character.Appear();

        // Enable NPCInteraction setelah appear
        NPCInteraction npcInteraction = character.GetComponent<NPCInteraction>();
        if (npcInteraction != null)
        {
            // Update state NPCInteraction
            var type = npcInteraction.GetType();
            var updateMethod = type.GetMethod("UpdateNPCState",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);

            if (updateMethod != null)
            {
                DayCycleManager dayCycle = FindObjectOfType<DayCycleManager>();
                updateMethod.Invoke(npcInteraction, new object[] { dayCycle.currentTime, false });
            }
        }
    }

    // Called when a dialogue starts (increment counter)
    public static void EnterDialogue()
    {
        activeDialogueCount++;
    }

    // Called when a dialogue ends (decrement counter)
    public static void ExitDialogue()
    {
        activeDialogueCount = Mathf.Max(0, activeDialogueCount - 1);
    }
}