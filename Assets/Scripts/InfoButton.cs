using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This will be for a pause screen and is needed as the button is technically
//a gameObject in the world and not a UI button
public class InfoButton : MonoBehaviour
{
    public Sprite hoverSprite;
    private Sprite defaultSprite;
    private SpriteRenderer sRenderer;

    public GameObject pauseMenu;
    bool isPaused = false;


    private void Awake()
    {
        sRenderer = GetComponent<SpriteRenderer>();
        defaultSprite = sRenderer.sprite;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(!isPaused)
            {
                isPaused = true;
                OpenPauseMenu();
            }
            else
            {
                isPaused = false;
                ClosePauseMenu();
            }
        }
    }

    private void OnMouseDown()
    {
        if (!isPaused)
        {
            isPaused = true;
            OpenPauseMenu();
        }
        else
        {
            isPaused = false;
            ClosePauseMenu();
        }

    }

    private void OnMouseEnter()
    {
        sRenderer.sprite = hoverSprite;
        
    }

    private void OnMouseExit()
    {
        
        sRenderer.sprite = defaultSprite;
    }

    public void OpenPauseMenu()
    {
        pauseMenu.SetActive(true);
    }

    public void ClosePauseMenu()
    {
        pauseMenu.SetActive(false);
    }
}
