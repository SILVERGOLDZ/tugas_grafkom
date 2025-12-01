using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class GiftItem
{
    public string giftName;
    public int giftID;
    public int quantity;
    
    public GiftItem(string name, int id)
    {
        giftName = name;
        giftID = id;
        quantity = 1;
    }
}

public class InventorySystem : MonoBehaviour
{
    [Header("Inventory")]
    public List<GiftItem> giftInventory = new List<GiftItem>();
    
    [Header("UI Settings")]
    public bool showInventoryUI = true;
    public KeyCode toggleInventoryKey = KeyCode.Tab;
    
    private bool inventoryVisible = false;

    void Update()
    {
        // Toggle inventory UI dengan Tab
        if (Input.GetKeyDown(toggleInventoryKey))
        {
            inventoryVisible = !inventoryVisible;
        }
    }

    // Fungsi untuk menambah gift ke inventory
    public void AddGift(string giftName, int giftID)
    {
        // Cek apakah gift sudah ada di inventory
        GiftItem existingGift = giftInventory.Find(g => g.giftID == giftID);
        
        if (existingGift != null)
        {
            // Gift sudah ada, tambah quantity
            existingGift.quantity++;
        }
        else
        {
            // Gift baru, tambah ke inventory
            GiftItem newGift = new GiftItem(giftName, giftID);
            giftInventory.Add(newGift);
        }
        
        Debug.Log($"Added {giftName} to inventory. Total: {GetGiftCount(giftID)}");
    }

    // Fungsi untuk menghapus gift dari inventory
    public bool RemoveGift(int giftID, int amount = 1)
    {
        GiftItem gift = giftInventory.Find(g => g.giftID == giftID);
        
        if (gift != null && gift.quantity >= amount)
        {
            gift.quantity -= amount;
            
            if (gift.quantity <= 0)
            {
                giftInventory.Remove(gift);
            }
            
            return true;
        }
        
        return false;
    }

    // Get jumlah gift tertentu
    public int GetGiftCount(int giftID)
    {
        GiftItem gift = giftInventory.Find(g => g.giftID == giftID);
        return gift != null ? gift.quantity : 0;
    }

    // Get total semua gift
    public int GetTotalGiftCount()
    {
        int total = 0;
        foreach (GiftItem gift in giftInventory)
        {
            total += gift.quantity;
        }
        return total;
    }

    // Simple UI untuk menampilkan inventory
    void OnGUI()
    {
        if (!showInventoryUI || !inventoryVisible) return;
        
        // Background panel
        GUI.Box(new Rect(10, 10, 250, 300), "Inventory (Tab to toggle)");
        
        // Display gift items
        int yOffset = 40;
        
        if (giftInventory.Count == 0)
        {
            GUI.Label(new Rect(20, yOffset, 230, 25), "No gifts collected yet");
        }
        else
        {
            foreach (GiftItem gift in giftInventory)
            {
                string displayText = $"{gift.giftName} x{gift.quantity}";
                GUI.Label(new Rect(20, yOffset, 230, 25), displayText);
                yOffset += 30;
            }
            
            // Total count
            yOffset += 10;
            GUI.Label(new Rect(20, yOffset, 230, 25), $"Total Gifts: {GetTotalGiftCount()}");
        }
        
        // Instructions
        GUI.Label(new Rect(20, 270, 230, 25), "Press E to pickup gifts");
    }
}