using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LowButtonObject : MonoBehaviour
{
    public Vector3 scaleTo = new Vector3(0.5f, 0.5f, 0.5f);
    public float scaleTime = 0.5f;
    public float punchScaleTime = 1.1f;
    public float punchScale = 0.5f;
    public Sprite hoverSprite;
    public AudioClip hoverSound;
    public AudioClip pressSound;

    private Sprite defaultSprite;
    private Vector3 originalScale;
    private SpriteRenderer sRenderer;
    private GameManager manager;

    private void Awake()
    {
        originalScale = this.gameObject.transform.localScale;
        sRenderer = GetComponent<SpriteRenderer>();
        manager = FindObjectOfType<GameManager>();
        defaultSprite = sRenderer.sprite;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseDown()
    {
        if (manager.canGuess)
        {
            AudioSource.PlayClipAtPoint(pressSound, new Vector3(0, 0, 0));
            FindObjectOfType<GameManager>().GetComponent<GameManager>().GuessLow();
            iTween.PunchScale(gameObject, new Vector3(punchScale, punchScale, punchScale), punchScaleTime);
        }
    }

    private void OnMouseEnter()
    {
        sRenderer.sprite = hoverSprite;
        iTween.ScaleTo(gameObject, iTween.Hash("scale", scaleTo, "time", scaleTime,
            "looptype", iTween.LoopType.pingPong, "easetype", iTween.EaseType.linear));
        AudioSource.PlayClipAtPoint(hoverSound, new Vector3(0, 0, 0));
    }

    private void OnMouseExit()
    {
        iTween.ScaleTo(gameObject, iTween.Hash("scale", originalScale, "time", scaleTime));
        sRenderer.sprite = defaultSprite;
    }
}
