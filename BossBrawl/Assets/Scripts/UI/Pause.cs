using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using UnityEngine.SceneManagement;

public class Pause : MenuScreen
{

    private Animator animator;
    private Player inputPlayer;
    public bool paused;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        curButton = defaultButton;
    }

    void Start()
    {

    }

    public void ReturnToMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
        PauseToggle();
    }

    public override void Update()
    {
        base.Update();
        animator.SetBool("Open", paused);

        if(!paused)
        {
            for(int i = 0; i < buttons.Length; i++)
            {
                buttons[i].selected = false;
            }
        }

        if (InputManager.instance.player1.GetButtonDown("Pause"))
        {
            inputPlayer = InputManager.instance.player1;
            PauseToggle();
        }
        else if (InputManager.instance.player2.GetButtonDown("Pause"))
        {
            inputPlayer = InputManager.instance.player2;
            PauseToggle();
        }
    }

    public void PauseToggle()
    {
        curButton = defaultButton;
        paused = !paused;
        Time.timeScale = paused ? 0 : 1;
    }
}
