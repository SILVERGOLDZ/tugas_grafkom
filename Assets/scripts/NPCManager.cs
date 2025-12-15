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

    private void Start()
    {
        dayCycle.OnTimeChanged += HandleTimeChange;
        HandleTimeChange(dayCycle.currentTime); // apply waktu awal
    }

    void HandleTimeChange(DayCycleManager.TimeOfDay time)
    {
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
}
