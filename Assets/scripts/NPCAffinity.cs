using UnityEngine;

public class NPCAffinity : MonoBehaviour
{
    public int affinityLevel; // 0 - 4
    public int currentPoints;

    int[] levelThresholds = { 100, 250, 500, 1000 };

    public void AddPoints(int amount)
    {
        if (affinityLevel >= 4) return;

        currentPoints += amount;

        while (affinityLevel < 4 &&
               currentPoints >= levelThresholds[affinityLevel])
        {
            currentPoints -= levelThresholds[affinityLevel];
            affinityLevel++;
        }
    }
}
