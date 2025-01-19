using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ShadowEffect : MonoBehaviour
{
    public Vector3 Offset = new Vector3(0.09f, -0.7f);
    public Material material;
    public int orderOffset;


    private GameObject _shadow;

    // Start is called before the first frame update
    void Start()
    {
        _shadow = new GameObject("Shadow");
        _shadow.transform.localPosition = Offset;
        _shadow.transform.localRotation = this.transform.localRotation;
        _shadow.transform.localScale = this.transform.localScale;

        SpriteRenderer sRenderer = GetComponent<SpriteRenderer>();
        SpriteRenderer sr = _shadow.AddComponent<SpriteRenderer>();
        sr.sprite = sRenderer.sprite;
        sr.material = material;

        sr.sortingLayerName = sRenderer.sortingLayerName;
        sr.sortingOrder = sRenderer.sortingOrder - orderOffset;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        _shadow.transform.localPosition = Offset;
        _shadow.transform.localScale = this.transform.localScale;
    }
}
