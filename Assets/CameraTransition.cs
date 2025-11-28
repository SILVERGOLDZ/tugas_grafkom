using UnityEngine;
using System.Collections;

public class CameraTransition : MonoBehaviour
{
    private Camera npcActiveCamera;

    public Camera mcCamera;            // Main character camera
    public float transitionDuration = 1.0f;

    private Camera targetCamera;       // The NPC camera we want to switch to
    private bool isTransitioning = false;
    private float t = 0f;

    private Vector3 startPos;
    private Quaternion startRot;

    public void StartTransitionTo(Camera npcCam)
    {
        npcActiveCamera = npcCam;
        targetCamera = npcCam;
        targetCamera.enabled = false;  // Keep NPC camera disabled; we only use it as a transform reference

        startPos = mcCamera.transform.position;
        startRot = mcCamera.transform.rotation;

        t = 0f;
        isTransitioning = true;
    }

    public void ReturnToMC()
    {
        StartCoroutine(ReturnRoutine());
    }

    private IEnumerator ReturnRoutine()
    {
        npcActiveCamera.enabled = false;
        mcCamera.enabled = true;

        mcCamera.transform.position = npcActiveCamera.transform.position;
        mcCamera.transform.rotation = npcActiveCamera.transform.rotation;

        yield return new WaitForSeconds(0.2f);
    }

    void Update()
    {
        if (!isTransitioning)
            return;

        t += Time.deltaTime / transitionDuration;

        mcCamera.transform.position = Vector3.Lerp(startPos, targetCamera.transform.position, t);
        mcCamera.transform.rotation = Quaternion.Lerp(startRot, targetCamera.transform.rotation, t);

        if (t >= 1f)
        {
            // End transition ? MC camera stops rendering
            mcCamera.enabled = false;

            // NPC camera takes over
            targetCamera.enabled = true;

            isTransitioning = false;
        }
    }

    void Start()
    {
        // Disable all cameras except the MC
        Camera[] allCameras = FindObjectsByType<Camera>(FindObjectsSortMode.None);

        foreach (Camera cam in allCameras)
        {
            if (cam != mcCamera)
                cam.enabled = false;
        }
    }

}
