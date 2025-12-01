using UnityEngine;

public class GiftSpawner : MonoBehaviour
{
    [Header("Gift Prefabs")]
    public GameObject[] giftPrefabs; // Array untuk 3 jenis gift
    
    [Header("Spawn Settings")]
    public float spawnInterval = 5f; // Interval spawn gift
    public int maxGiftsAtOnce = 10; // Maksimal gift yang ada di scene
    public Vector3 spawnAreaMin = new Vector3(-20f, 1f, -20f); // Area spawn minimum
    public Vector3 spawnAreaMax = new Vector3(20f, 1f, 20f); // Area spawn maximum
    
    private float spawnTimer = 0f;
    private int currentGiftCount = 0;

    void Update()
    {
        // Timer untuk spawn gift secara prosedural
        spawnTimer += Time.deltaTime;
        
        if (spawnTimer >= spawnInterval && currentGiftCount < maxGiftsAtOnce)
        {
            SpawnGift();
            spawnTimer = 0f;
        }
    }

    void SpawnGift()
    {
        // Validasi prefab array
        if (giftPrefabs == null || giftPrefabs.Length == 0)
        {
            Debug.LogError("GiftSpawner: No gift prefabs assigned!");
            return;
        }
        
        // Pilih gift random dari array
        int randomIndex = Random.Range(0, giftPrefabs.Length);
        GameObject giftPrefab = giftPrefabs[randomIndex];
        
        if (giftPrefab == null)
        {
            Debug.LogError($"GiftSpawner: Gift prefab at index {randomIndex} is null!");
            return;
        }
        
        // Generate posisi random dalam area spawn (TRANSLASI MANUAL)
        Vector3 randomPosition = new Vector3(
            Random.Range(spawnAreaMin.x, spawnAreaMax.x),
            Random.Range(spawnAreaMin.y, spawnAreaMax.y),
            Random.Range(spawnAreaMin.z, spawnAreaMax.z)
        );
        
        // Spawn gift
        GameObject gift = Instantiate(giftPrefab, randomPosition, Quaternion.identity);
        currentGiftCount++;
        
        // Subscribe ke event ketika gift dihancurkan
        Gift giftScript = gift.GetComponent<Gift>();
        if (giftScript != null)
        {
            giftScript.OnGiftDestroyed += OnGiftDestroyedHandler;
        }
        else
        {
            Debug.LogWarning("GiftSpawner: Spawned gift doesn't have Gift component!");
        }
    }
    
    void OnGiftDestroyedHandler()
    {
        currentGiftCount--;
        if (currentGiftCount < 0) currentGiftCount = 0; // Safety check
    }

    // Visualisasi area spawn di editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(
            (spawnAreaMin + spawnAreaMax) / 2f,
            spawnAreaMax - spawnAreaMin
        );
    }
}