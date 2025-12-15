using UnityEngine;

public class GiftSpawner : MonoBehaviour
{
    [Header("Gift Prefabs")]
    public GameObject[] giftPrefabs;

    [Header("Spawn Settings")]
    public float spawnInterval = 5f;
    public int maxGiftsAtOnce = 10;

    public Vector3 spawnAreaMin = new Vector3(-11f, 1f, -60f);
    public Vector3 spawnAreaMax = new Vector3(-4f, 1f, 0f);

    private float spawnTimer;

    void Update()
    {
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnInterval && GetCurrentGiftCount() < maxGiftsAtOnce)
        {
            SpawnGift();
            spawnTimer = 0f;
        }
    }

    int GetCurrentGiftCount()
    {
        // Hitung gift yang masih ada di scene
        return GameObject.FindGameObjectsWithTag("Gift").Length;
    }

    void SpawnGift()
    {
        if (giftPrefabs == null || giftPrefabs.Length == 0)
        {
            Debug.LogError("GiftSpawner: No gift prefabs assigned!");
            return;
        }

        int randomIndex = Random.Range(0, giftPrefabs.Length);
        GameObject prefab = giftPrefabs[randomIndex];

        Vector3 randomPosition = new Vector3(
            Random.Range(spawnAreaMin.x, spawnAreaMax.x),
            spawnAreaMin.y,
            Random.Range(spawnAreaMin.z, spawnAreaMax.z)
        );

        Instantiate(prefab, randomPosition, Quaternion.identity);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(
            (spawnAreaMin + spawnAreaMax) / 2f,
            spawnAreaMax - spawnAreaMin
        );
    }
}
