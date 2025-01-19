using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public Sprite face;
    public Sprite back;

    public int value;
    public string suit;

    private SpriteRenderer sRenderer;

    private void Awake()
    {
        sRenderer = GetComponent<SpriteRenderer>();
        sRenderer.sprite = back;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FlipCard()
    {
        sRenderer.sprite = face;
    }

    public void AnimateFlipCard()
    {
        StartCoroutine(FlipCardCoroutine());
    }

    IEnumerator FlipCardCoroutine()
    {
        yield return RotateTo(90, 0.5f);

        // Rotate the Sprite
        sRenderer.sprite = face;

        // Rotate Back
        yield return RotateTo(0, 0.5f);

        
        transform.eulerAngles = new Vector3(0, 0, 0);
    }

    IEnumerator RotateTo(float targetYRotation, float duration)
    {
        float time = 0;
        float startRotationY = transform.eulerAngles.y;
        float endRotationY = targetYRotation;

        while (time < duration)
        {
            float newYRotation = Mathf.Lerp(startRotationY, endRotationY, time / duration);
            transform.eulerAngles = new Vector3(0, newYRotation, 0);
            time += Time.deltaTime;
            yield return null;
        }

        transform.eulerAngles = new Vector3(0, endRotationY, 0);
    }

   
}
