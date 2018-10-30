using Rewired;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
 
public class InputManager : MonoBehaviour
{

    public static InputManager instance;
    public Player player1;
    public Player player2;
    public Player system;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        player1 = ReInput.players.Players[0];
        player2 = ReInput.players.Players[1];
        system = ReInput.players.SystemPlayer;
    }

    void Update()
    {
		for(int i = 0; i < ReInput.controllers.GetControllers(ControllerType.Joystick).Length; i++)
			if(!system.controllers.ContainsController(ReInput.controllers.GetControllers(ControllerType.Joystick)[i]))
				system.controllers.AddController(ReInput.controllers.GetControllers(ControllerType.Joystick)[i], false);
	}
}
