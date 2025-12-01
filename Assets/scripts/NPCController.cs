using UnityEngine;

public class CharacterAppear : MonoBehaviour
{
    public float animationDuration = 0.5f;
    private Vector3 targetScale;
    private Vector3 hiddenScale = Vector3.zero;
    private float timer = 0f;
    private bool animating = false;
    private bool appearing = false;

    private void Awake()
    {
        targetScale = transform.localScale;
        transform.localScale = Vector3.zero; // mulai tersembunyi
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!animating) return;

        timer += Time.deltaTime;
        float t = timer / animationDuration;

        if (appearing)
            transform.localScale = Vector3.Lerp(hiddenScale, targetScale, t);
        else
            transform.localScale = Vector3.Lerp(targetScale, hiddenScale, t);

        if (t >= 1f)
        {
            animating = false;

            if (!appearing)
                gameObject.SetActive(false);
        }
    }

    public void Appear()
    {
        gameObject.SetActive(true);
        timer = 0f;
        appearing = true;
        animating = true;
        transform.localScale = Vector3.zero;
    }

    public void Disappear()
    {
        timer = 0f;
        appearing = false;
        animating = true;
    }
}
