using UnityEngine;

public class PlayerGiftInventory : MonoBehaviour
{
    public int roseCount;
    public int chocolateCount;
    public int sundaeCount;

    public bool HasGift(GiftType type)
    {
        return GetCount(type) > 0;
    }

    public int GetCount(GiftType type)
    {
        switch (type)
        {
            case GiftType.Rose: return roseCount;
            case GiftType.Chocolate: return chocolateCount;
            case GiftType.Sundae: return sundaeCount;
        }
        return 0;
    }

    public void AddGift(GiftType type, int amount = 1)
    {
        switch (type)
        {
            case GiftType.Rose: roseCount += amount; break;
            case GiftType.Chocolate: chocolateCount += amount; break;
            case GiftType.Sundae: sundaeCount += amount; break;
        }
    }

    public bool ConsumeGift(GiftType type)
    {
        if (!HasGift(type)) return false;

        switch (type)
        {
            case GiftType.Rose: roseCount--; break;
            case GiftType.Chocolate: chocolateCount--; break;
            case GiftType.Sundae: sundaeCount--; break;
        }
        return true;
    }
}
