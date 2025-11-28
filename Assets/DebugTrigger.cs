using UnityEngine;

public class DebugTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("ENTERED trigger by: " + other.name);
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("EXIT trigger by: " + other.name);
    }
}
