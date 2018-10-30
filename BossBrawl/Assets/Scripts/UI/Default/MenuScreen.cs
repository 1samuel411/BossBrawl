using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuScreen : MonoBehaviour
{

    public Button[] buttons;
    public int defaultButton;
    public int curButton;
    public GameObject child;

    void Start()
    {
        
    }

    private float lastInputRead;
    public virtual void Update()
    {
        for(int i = 0; i < buttons.Length; i++)
        {
            if (i != curButton)
                buttons[i].selected = false;
            else
                buttons[i].selected = true;
        }

        if (InputManager.instance.system.GetAxis("XMovement") > 0f && buttons[curButton].right != null && Time.unscaledTime >= lastInputRead)
        {
            lastInputRead = Time.unscaledTime + 0.3f;
            // right
            SelectButton(buttons[curButton].right);
        }
        if (InputManager.instance.system.GetAxis("XMovement") < -0f && buttons[curButton].left != null && Time.unscaledTime >= lastInputRead)
        {
            lastInputRead = Time.unscaledTime + 0.3f;
            // left
            SelectButton(buttons[curButton].left);
        }
        if (InputManager.instance.system.GetAxis("YMovement") > 0f && buttons[curButton].up != null && Time.unscaledTime >= lastInputRead)
        {
            lastInputRead = Time.unscaledTime + 0.3f;
            // up
            SelectButton(buttons[curButton].up);
        }
        if (InputManager.instance.system.GetAxis("YMovement") < -0f && buttons[curButton].down != null && Time.unscaledTime >= lastInputRead)
        {
            lastInputRead = Time.unscaledTime + 0.3f;
            // down
            SelectButton(buttons[curButton].down);
        }
    }

    public void SelectButton(Button button)
    {
        for(int i = 0; i < buttons.Length; i++)
        {
            if(buttons[i] == button)
            {
                curButton = i;
                break;
            }
        }
    }

    void OnEnable()
    {
        curButton = defaultButton;
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
