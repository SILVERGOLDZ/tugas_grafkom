using UnityEngine;

public class DayCycleManager : MonoBehaviour
{
    public enum TimeOfDay { Morning, Noon, Evening, Night }
    public TimeOfDay currentTime; 

    [Header("Cycle Settings")]
    public float cycleDuration = 10f; // durasi tiap fase (detik)
    private float timer = 0f;

    // event yang bisa dipanggil nanti (spawn/despawn karakter)
    public System.Action<TimeOfDay> OnTimeChanged;

    private void Start()
    {
        ChangeTime(TimeOfDay.Morning); 
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= cycleDuration)
        {
            timer = 0f;
            MoveToNextTime();
        }
    }

    void MoveToNextTime()
    {
        switch (currentTime)
        {
            case TimeOfDay.Morning:
                ChangeTime(TimeOfDay.Noon);
                break;
            case TimeOfDay.Noon:
                ChangeTime(TimeOfDay.Evening);
                break;
            case TimeOfDay.Evening:
                ChangeTime(TimeOfDay.Night);
                break;
            case TimeOfDay.Night:
                ChangeTime(TimeOfDay.Morning);
                break;
        }
    }

    void ChangeTime(TimeOfDay newTime)
    {
        currentTime = newTime;
        Debug.Log("Time changed to: " + newTime);

        OnTimeChanged?.Invoke(newTime);
    }
}
