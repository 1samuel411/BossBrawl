using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class Pause : MenuScreen
{

    private Animator animator;
    private Player inputPlayer;
    public bool paused;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        curButton = defaultButton;
    }

    void Start()
    {

    }

    void Update()
    {
        animator.SetBool("Open", paused);
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

        if(paused && curButton != null)
        {
            curButton.selected = true;
        }
    }

    public void PauseToggle()
    {
        curButton = defaultButton;
        paused = !paused;
        Time.timeScale = paused ? 0 : 1;
    }
}
