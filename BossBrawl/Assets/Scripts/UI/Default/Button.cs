using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Button : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{

    public Button down, up, left, right;
    public MenuScreen menuScreen;

    private Animator animator;
    public bool selected;

    public Player player;

    public UnityEvent pressEvent;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        animator.SetBool("Selected", selected);
        if (!selected)
            return;

        if (player == null)
        {
            CheckInput(InputManager.instance.player1);
            CheckInput(InputManager.instance.player2);
        }
        else
        {
            CheckInput(player);
        }
    }

    void CheckInput(Player player)
    {
        if (player.GetAxis("XMovement") > 0.5f && right != null)
        {
            // right
            selected = false;
            menuScreen.curButton = right;
        }
        if (player.GetAxis("XMovement") < -0.5f && left != null)
        {
            // left
            selected = false;
            menuScreen.curButton = left;
        }
        if (player.GetAxis("YMovement") > 0.5f && up != null)
        {
            // up
            selected = false;
            menuScreen.curButton = up;
        }
        if (player.GetAxis("YMovement") < -0.5f && down != null)
        {
            // down
            selected = false;
            menuScreen.curButton = down;
        }

        if(player.GetButtonDown("Select"))
        {
            Debug.Log("pressed");
            pressEvent.Invoke();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Mouse enter");
        selected = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("Mouse exit");
        //selected = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("pressed");
        pressEvent.Invoke();
    }

}
