using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class KeyboardMouseInput : InputType<KeyCode> {
	private KeyCode[] keyCodes; //maps commands to their assigned keys

	private const string commandTag = "key"; //suffix added to playerprefs keys

	public KeyCode[] DefaultKeys {
		get { return defaultKeys; }
	}
	private readonly KeyCode[] defaultKeys; //holds numeric ASCII values of the default keys assigned to each command

	public KeyboardMouseInput() {
		int numCommands = InputManager.numCommands;
		keyCodes = new KeyCode[numCommands];

		//assign default key values
		defaultKeys = new KeyCode[numCommands];
		defaultKeys[(int) Command.Forward] = KeyCode.W;
		defaultKeys[(int) Command.Backward] = KeyCode.S;
		defaultKeys[(int) Command.Left] = KeyCode.A;
		defaultKeys[(int) Command.Right] = KeyCode.D;
		defaultKeys[(int) Command.Jump] = KeyCode.Mouse0;
		defaultKeys[(int) Command.Brake] = KeyCode.Mouse1;
		defaultKeys[(int) Command.Special] = KeyCode.Space;
		defaultKeys[(int) Command.Pause] = KeyCode.Escape;
		
		//assign key commands using saved settings
		for(int i = 0; i < numCommands; i++) {
			string command = ((Command) i).ToString();
			keyCodes[i] = (KeyCode) PlayerPrefs.GetInt(command + commandTag, (int) defaultKeys[i]);
		}
	}

	//should only be called within Update()
	public bool buttonDown(Command command) {
		KeyCode assignedKey = keyCodes[(int) command];

		if(command == Command.Jump || command == Command.Pause) { //prevent repeated jumping/pausing
			return Input.GetKeyDown(assignedKey);

		} else {
			return Input.GetKey(assignedKey);
		}		
	}

	//the string representation of the command is used as the key for PlayerPrefs
	public void setButton(Command command, KeyCode newKey) {
		keyCodes[(int) command] = newKey;
		PlayerPrefs.SetInt(command.ToString() + commandTag, (int) newKey);
	}

	//returns the current key that activates command
	public KeyCode getMapping(Command command) {
		return keyCodes[(int) command];
	}

	public float xAxisMovement() { return Input.GetAxis("Mouse X"); }
	public float yAxisMovement() { return -Input.GetAxis("Mouse Y"); }
}