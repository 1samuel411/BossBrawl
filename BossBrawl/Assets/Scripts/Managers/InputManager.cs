using Rewired;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{

    public static InputManager instance;
    public Player player1;
    public Player player2;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        player1 = ReInput.players.Players[0];
        player2 = ReInput.players.Players[1];
    }

    void Update()
    {

    }
}
