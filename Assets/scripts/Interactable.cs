using UnityEngine;
using UnityEngine.UI;

public class Interactable : MonoBehaviour
{
    public float interactRange = 3f;
    public Transform headTransform;
    public Material outlineMaterial; // if using material duplication approach
    GameObject outlineDup;

    void Start()
    {
        // prepare outline duplicate
        MeshFilter mf = GetComponentInChildren<MeshFilter>();
        if (mf)
        {
            outlineDup = new GameObject("outline");
            outlineDup.transform.SetParent(mf.transform, false);
            MeshFilter of = outlineDup.AddComponent<MeshFilter>();
            of.mesh = mf.mesh;
            MeshRenderer or = outlineDup.AddComponent<MeshRenderer>();
            or.material = outlineMaterial;
            outlineDup.transform.localScale = Vector3.one * 1.02f;
            outlineDup.SetActive(false);
        }
    }

    public void SetHighlighted(bool on)
    {
        if (outlineDup) outlineDup.SetActive(on);
    }
}
