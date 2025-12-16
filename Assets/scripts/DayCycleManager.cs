using UnityEngine;

public class DayCycleManager : MonoBehaviour
{
    public enum TimeOfDay
    {
        Morning,
        Noon,
        Evening,
        Night
    }

    public TimeOfDay currentTime = TimeOfDay.Morning;

    // Event untuk shader, skybox, dll
    public System.Action<TimeOfDay> OnTimeChanged;

    void Start()
    {
        ApplyTime();
    }

    // DIPANGGIL OLEH NPC
    public void NextTime()
    {
        switch (currentTime)
        {
            case TimeOfDay.Morning:
                currentTime = TimeOfDay.Noon;
                break;
            case TimeOfDay.Noon:
                currentTime = TimeOfDay.Evening;
                break;
            case TimeOfDay.Evening:
                currentTime = TimeOfDay.Night;
                break;
            case TimeOfDay.Night:
                currentTime = TimeOfDay.Morning;
                break;
        }

        ApplyTime();
    }

    void ApplyTime()
    {
        Debug.Log("Time changed to: " + currentTime);
        OnTimeChanged?.Invoke(currentTime);
    }
}
