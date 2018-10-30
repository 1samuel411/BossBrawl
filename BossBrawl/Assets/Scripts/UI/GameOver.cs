using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOver : MenuScreen
{

    public static GameOver instance;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {

    }

    public void ReturnToMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
        Time.timeScale = 1;
    }

    public void Death()
    {
        StartCoroutine(Died());
    }

    IEnumerator Died()
    {
        yield return new WaitForSeconds(2);
        child.SetActive(true);
        Time.timeScale = 0;
    }
}
