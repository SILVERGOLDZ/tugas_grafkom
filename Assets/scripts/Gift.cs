using UnityEngine;
using System.Collections;

public enum GiftType
{
    Rose,
    Chocolate,
    Sundae
}

public class Gift : MonoBehaviour
{
    public GiftType giftType;
    public float spawnSpeed = 2f;
    public float rotateSpeed = 90f;
    public float collectSpeed = 5f;

    private bool spawning = true;
    private bool collected = false;

    void Start()
    {
        // Mulai dari scale 0
        transform.localScale = Vector3.zero;
    }

    void Update()
    {
        if (spawning)
        {
            Vector3 scale = transform.localScale;

            scale.x += spawnSpeed * Time.deltaTime;
            scale.y += spawnSpeed * Time.deltaTime;
            scale.z += spawnSpeed * Time.deltaTime;

            if (scale.x >= 1f)
            {
                scale = Vector3.one;
                spawning = false;
            }

            transform.localScale = scale;
        }
        else if (!collected)
        {
            // ROTASI MANUAL
            Vector3 rot = transform.rotation.eulerAngles;
            rot.y += rotateSpeed * Time.deltaTime;
            transform.rotation = Quaternion.Euler(rot);
        }
        else
        {
            // SHRINK SAAT COLLECT
            Vector3 scale = transform.localScale;

            scale.x -= collectSpeed * Time.deltaTime;
            scale.y -= collectSpeed * Time.deltaTime;
            scale.z -= collectSpeed * Time.deltaTime;

            if (scale.x <= 0f)
            {
                Destroy(gameObject);
            }
            else
            {
                transform.localScale = scale;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (collected) return;
        if (!other.CompareTag("Player")) return;

        PlayerGiftInventory inventory = other.GetComponent<PlayerGiftInventory>();
        if (inventory == null) return;

        inventory.AddGift(giftType, 1);
        collected = true;
    }
}
