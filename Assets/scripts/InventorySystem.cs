// using UnityEngine;
// using System.Collections.Generic;

// public class PlayerInventory : MonoBehaviour
// {
//     private Dictionary<int, int> gifts = new Dictionary<int, int>();

//     public void AddGift(int id, int amount)
//     {
//         if (gifts.ContainsKey(id))
//             gifts[id] += amount;
//         else
//             gifts.Add(id, amount);

//         Debug.Log($"Gift {id} total: {gifts[id]}");
//     }

//     // Akses inventory (contoh)
//     public int GetGiftCount(int id)
//     {
//         return gifts.ContainsKey(id) ? gifts[id] : 0;
//     }

//     public int GetTotalGifts()
//     {
//         int total = 0;
//         foreach (var item in gifts)
//             total += item.Value;

//         Debug.Log("Total gift: " + GetTotalGifts());


//         return total;
//     }

// }

