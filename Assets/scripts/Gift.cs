using UnityEngine;
using System;

public class Gift : MonoBehaviour
{
    [Header("Gift Info")]
    public string giftName = "Gift";
    public int giftID = 0; // ID unik untuk setiap jenis gift
    
    [Header("Animation Settings")]
    public float spawnDuration = 1f; // Durasi membesar
    public float rotateDuration = 3f; // Durasi berputar di tempat
    public float despawnDuration = 1f; // Durasi mengecil
    public float rotationSpeed = 180f; // Kecepatan rotasi (derajat per detik)
    
    [Header("Scale Settings")]
    public float targetScale = 1f; // Skala akhir saat spawn selesai
    
    // State management
    private enum GiftState { Spawning, Rotating, Despawning, Collected }
    private GiftState currentState = GiftState.Spawning;
    
    // Transform values - MANUAL CALCULATION
    private Vector3 initialScale;
    private Vector3 currentScale;
    private float stateTimer = 0f;
    private float currentRotationY = 0f;
    
    // Event untuk notifikasi ke spawner
    public event Action OnGiftDestroyed;
    
    // Material untuk shader animation
    private Material giftMaterial;
    private Color originalColor;

    void Start()
    {
        // Simpan scale awal (harus 0 untuk efek spawn)
        initialScale = Vector3.zero;
        currentScale = initialScale;
        transform.localScale = currentScale;
        
        // Get material untuk shader animation
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            giftMaterial = renderer.material;
            originalColor = giftMaterial.color;
        }
    }

    void Update()
    {
        stateTimer += Time.deltaTime;
        
        switch (currentState)
        {
            case GiftState.Spawning:
                UpdateSpawning();
                break;
            case GiftState.Rotating:
                UpdateRotating();
                break;
            case GiftState.Despawning:
                UpdateDespawning();
                break;
        }
    }

    // TRANSFORMASI 1: SKALA - Membesar saat spawn (MANUAL)
    void UpdateSpawning()
    {
        float progress = stateTimer / spawnDuration;
        
        if (progress >= 1f)
        {
            // Spawn selesai, pindah ke state rotating
            currentScale = Vector3.one * targetScale;
            transform.localScale = currentScale;
            currentState = GiftState.Rotating;
            stateTimer = 0f;
        }
        else
        {
            // Interpolasi scale dari 0 ke targetScale (MANUAL - tidak pakai Vector3.Lerp built-in effect)
            float t = progress; // Linear interpolation
            currentScale.x = initialScale.x + (targetScale - initialScale.x) * t;
            currentScale.y = initialScale.y + (targetScale - initialScale.y) * t;
            currentScale.z = initialScale.z + (targetScale - initialScale.z) * t;
            
            transform.localScale = currentScale;
        }
    }

    // TRANSFORMASI 2: ROTASI - Berputar di tempat (MANUAL)
    void UpdateRotating()
    {
        // Rotasi manual dengan mengubah rotation.y
        currentRotationY += rotationSpeed * Time.deltaTime;
        
        // Normalisasi sudut (0-360)
        if (currentRotationY >= 360f)
            currentRotationY -= 360f;
        
        // Apply rotasi manual menggunakan Euler angles
        Vector3 currentRotation = transform.rotation.eulerAngles;
        currentRotation.y = currentRotationY;
        transform.rotation = Quaternion.Euler(currentRotation);
        
        // Setelah durasi rotasi, mulai despawn
        if (stateTimer >= rotateDuration)
        {
            currentState = GiftState.Despawning;
            stateTimer = 0f;
        }
    }

    // TRANSFORMASI 3: SKALA - Mengecil saat despawn (MANUAL)
    void UpdateDespawning()
    {
        float progress = stateTimer / despawnDuration;
        
        if (progress >= 1f)
        {
            // Despawn selesai, hancurkan object
            DestroyGift();
        }
        else
        {
            // Interpolasi scale dari targetScale ke 0 (MANUAL)
            float t = progress;
            float startScale = targetScale;
            float endScale = 0f;
            
            float newScale = startScale + (endScale - startScale) * t;
            currentScale = Vector3.one * newScale;
            transform.localScale = currentScale;
            
            // Shader animation: fade out saat despawn
            if (giftMaterial != null)
            {
                Color fadedColor = originalColor;
                fadedColor.a = 1f - t; // Fade transparency
                giftMaterial.color = fadedColor;
            }
        }
    }

    // Fungsi untuk collect gift oleh player
    public void CollectGift()
    {
        if (currentState != GiftState.Collected)
        {
            currentState = GiftState.Collected;
            
            // Shader animation: flash effect saat diambil
            if (giftMaterial != null)
            {
                StartCoroutine(FlashEffect());
            }
            
            // Langsung despawn setelah flash
            Invoke("DestroyGift", 0.2f);
        }
    }

    // Shader animation: efek flash ketika gift diambil
    System.Collections.IEnumerator FlashEffect()
    {
        Color brightColor = Color.white;
        
        for (int i = 0; i < 2; i++)
        {
            giftMaterial.color = brightColor;
            yield return new WaitForSeconds(0.05f);
            giftMaterial.color = originalColor;
            yield return new WaitForSeconds(0.05f);
        }
    }

    void DestroyGift()
    {
        // Trigger event dulu sebelum destroy
        if (OnGiftDestroyed != null)
        {
            OnGiftDestroyed.Invoke();
        }
        
        Destroy(gameObject);
    }

    // Getter untuk info gift
    public string GetGiftName() => giftName;
    public int GetGiftID() => giftID;
}