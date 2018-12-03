using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public enum Command : int {Forward = 0, Backward = 1, Left = 2, Right = 3, Jump = 4, Brake = 5, 
								Special = 6, Pause = 7};

public class InputManager {



	public enum InputTypes {KeyboardMouse, OculusRift, XboxController, Mobile}

	public static readonly int numCommands = Enum.GetNames(typeof(Command)).Length;

	private static InputType<KeyCode> instance = null;

	public static InputType<KeyCode> getInput() {
		if(instance == null) {
			setInputType();
		}

		return instance;
	}

	private static void setInputType(InputTypes inputType = InputTypes.KeyboardMouse) {
		switch(inputType) {
			case InputTypes.KeyboardMouse:
				instance = new KeyboardMouseInput();
				break;

			case InputTypes.OculusRift:

			case InputTypes.XboxController:

			case InputTypes.Mobile:
				break;
		}
	}

}
