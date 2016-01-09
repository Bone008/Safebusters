using UnityEngine;
using System.Collections;
using System.IO.Ports;

public class CactusController : MonoBehaviour, InputIF {

	private InputState inputState = new InputState();
	private SerialPort stream = new SerialPort("COM3", 115200);

	private int[] buttonMasks = new int[]{
		64,  	//Up
		128,  	//Down
		256,	//Left
		512, 	//Right
		1024, 	//Far left
		2048  	//Far Right
	};

	private bool farLeftPressed = false;
	private bool farRightPressed = false;

	private GameButton lastPressed = GameButton.None;
	private bool farLeftLastPressed = false;
	private bool farRightLastPressed = false;


    public InputState GameInputState { get { return inputState; } }
    public bool FarLeftPressed { get { return farLeftPressed; } }
    public bool FarRightPressed { get { return farRightPressed; } }


	void Start(){
		stream.Open();
	}

	void Update(){
		inputState = collectInput ();
	}


	private InputState collectInput(){
		InputState inputState = new InputState ();

		//Read button input if connected
		int buttonVal;
		if (stream.IsOpen) {
			stream.Write ("1");
			buttonVal = System.Convert.ToInt32 (stream.ReadLine (), 16);   //convert input hex string to actual hex int

			inputState.HeldButtons = checkButtonsHeld (buttonVal);
			inputState.PressedButtons = checkButtonsPressed (buttonVal);
		}

		return inputState;
	}

	private GameButton checkButtonsHeld (int buttonVal){
		bool UpHeld, DownHeld, LeftHeld, RightHeld;

		UpHeld = ((buttonVal & buttonMasks [0]) != 0);
		DownHeld = ((buttonVal & buttonMasks [1]) != 0);			
		LeftHeld = ((buttonVal & buttonMasks [2]) != 0);
		RightHeld = ((buttonVal & buttonMasks [3]) != 0);

		return (UpHeld ? GameButton.Top : GameButton.None) |
			(DownHeld ? GameButton.Bottom : GameButton.None) |
			(LeftHeld ? GameButton.Left : GameButton.None) |
			(RightHeld ? GameButton.Right : GameButton.None);
	}

	private GameButton checkButtonsPressed (int buttonVal){
		bool UpPressed, DownPressed, LeftPressed, RightPressed;

		// TODO: not sure if works
		farLeftPressed = !farLeftLastPressed && ((buttonVal & buttonMasks [4]) != 0);
		farRightPressed = !farRightLastPressed && ((buttonVal & buttonMasks [5]) != 0);	
		farLeftLastPressed = farLeftPressed;
		farRightLastPressed = farRightPressed;

		// TODO: not sure if works
		UpPressed = ((lastPressed & GameButton.Top) == 0) && ((buttonVal & buttonMasks [0]) != 0);
		DownPressed = ((lastPressed & GameButton.Bottom) == 0) && ((buttonVal & buttonMasks [1]) != 0);	
		LeftPressed = ((lastPressed & GameButton.Left) == 0) && ((buttonVal & buttonMasks [2]) != 0);
		RightPressed = ((lastPressed & GameButton.Right) == 0) && ((buttonVal & buttonMasks [3]) != 0);

		lastPressed =  (UpPressed ? GameButton.Top : GameButton.None) |
			(DownPressed ? GameButton.Bottom : GameButton.None) |
			(LeftPressed ? GameButton.Left : GameButton.None) |
			(RightPressed ? GameButton.Right : GameButton.None);
		
		return lastPressed;
	}

}
