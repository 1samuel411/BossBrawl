using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuScreen : MonoBehaviour
{

    public Button defaultButton;
    public Button curButton;
    public GameObject child;

    void Start()
    {
        curButton = defaultButton;
        curButton.selected = true;
    }

    void Update()
    {

    }

    public void Quit()
    {
        Application.Quit();
    }

    public void LoadGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
    }
}
