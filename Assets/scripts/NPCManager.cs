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

    void HandleTimeChange(DayCycleManager.TimeOfDay time)
    {
        // Only apply changes if no dialogue is active
        if (activeDialogueCount > 0)
            return;

        // Semua hilangkan dulu
        alice?.Disappear();
        nozomi?.Disappear();
        hikari?.Disappear();
        miyu?.Disappear();
        miyako?.Disappear();

        // Lalu munculkan sesuai waktu
        switch (time)
        {
            case DayCycleManager.TimeOfDay.Morning:
                alice?.Appear();
                hikari?.Appear();
                break;

            case DayCycleManager.TimeOfDay.Noon:
                nozomi?.Appear();
                break;

            case DayCycleManager.TimeOfDay.Evening:
                miyu?.Appear();
                break;

            case DayCycleManager.TimeOfDay.Night:
                miyako?.Appear();
                break;
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