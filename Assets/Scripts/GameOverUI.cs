using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }

    public void Replay()
    {
        SceneManager.LoadScene("HighLowGame");
    }

    public void Done()
    {
        Application.Quit();
    }
}
