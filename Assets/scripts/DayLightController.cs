using UnityEngine;

public class DayLightController : MonoBehaviour
{
    public DayCycleManager dayCycle;
    public Material[] worldMaterials;

    void Start()
    {
        dayCycle.OnTimeChanged += OnTimeChanged;
    }

    void OnTimeChanged(DayCycleManager.TimeOfDay time)
    {
        float brightness = 1f;

        switch (time)
        {
            case DayCycleManager.TimeOfDay.Morning:
                brightness = 1.1f;
                break;
            case DayCycleManager.TimeOfDay.Noon:
                brightness = 1.3f;
                break;
            case DayCycleManager.TimeOfDay.Evening:
                brightness = 0.7f;
                break;
            case DayCycleManager.TimeOfDay.Night:
                brightness = 0.3f;
                break;
        }

        foreach (Material mat in worldMaterials)
        {
            mat.SetFloat("_Brightness", brightness);
        }
    }
}
