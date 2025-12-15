using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public int giftCount = 0;

    public void AddGift()
    {
        giftCount++;
        Debug.Log("Gift collected: " + giftCount);
    }
}
