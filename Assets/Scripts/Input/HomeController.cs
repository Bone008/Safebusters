using UnityEngine;
using System.Collections;

public class HomeController : MonoBehaviour, InputIF {

	private InputState inputState = new InputState ();

	void Update(){
		inputState = collectInput ();
	}

	public InputState getInput(){
		return inputState;
	}

	public bool FarLeftPressed(){
		return Input.GetKeyDown (KeyCode.LeftArrow);
	}

	public bool FarRightPressed(){
		return Input.GetKeyDown(KeyCode.RightArrow);	
	}

	private InputState collectInput(){
		InputState inputState = new InputState ();

		inputState.HeldButtons = checkButtonsHeld();
		inputState.PressedButtons = checkButtonsPressed();

		return inputState;
	}

	GameButton checkButtonsHeld(){
		bool UpHeld, DownHeld, LeftHeld, RightHeld;

		UpHeld = Input.GetKey(KeyCode.W);
		DownHeld = Input.GetKey(KeyCode.S);			
		LeftHeld = Input.GetKey(KeyCode.A);
		RightHeld = Input.GetKey(KeyCode.D);
	
		return (UpHeld ? GameButton.Top : GameButton.None) |
			(DownHeld ? GameButton.Bottom : GameButton.None) |
			(LeftHeld ? GameButton.Left : GameButton.None) |
			(RightHeld ? GameButton.Right : GameButton.None);
	}

	// returns all gamebuttons, that were pressed in this frame
	GameButton checkButtonsPressed(){
		bool UpPressed, DownPressed, LeftPressed, RightPressed;

		UpPressed = Input.GetKeyDown(KeyCode.W);
		DownPressed = Input.GetKeyDown(KeyCode.S);			
		LeftPressed = Input.GetKeyDown(KeyCode.A);
		RightPressed = Input.GetKeyDown(KeyCode.D);

		return (UpPressed ? GameButton.Top : GameButton.None) |
			(DownPressed ? GameButton.Bottom : GameButton.None) |
			(LeftPressed ? GameButton.Left : GameButton.None) |
			(RightPressed ? GameButton.Right : GameButton.None);
	}



}
