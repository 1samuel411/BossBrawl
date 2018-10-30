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
        
        CheckInput(InputManager.instance.system);
    }

    void CheckInput(Player player)
    {
        if(player.GetButtonDown("Select"))
        {
            Debug.Log("pressed");
            pressEvent.Invoke();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Mouse enter");
        menuScreen.SelectButton(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("Mouse exit");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("pressed");
        pressEvent.Invoke();
    }

}
