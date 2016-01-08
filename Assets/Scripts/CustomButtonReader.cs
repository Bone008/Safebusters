using UnityEngine;
using System;
using System.Collections;
using System.IO.Ports;
using Model;

public class CustomButtonReader : MonoBehaviour {

	public bool noCactus = true;

	public InputState inputState { get; set; }

    SerialPort stream = new SerialPort("COM3", 115200);
    string receivedData = "EMPTY";
	int buttonVal;
	bool connected = false;

    int[] buttonMasks = new int[]{
		64,  //Up
		128,  //Down
        256, //Left
        512, //Right
		1024, //Far left
		2048  //Far Right
    };

#region button variables used in checkButtonsHeld & checkButtonsPressed
	bool UpPressed;
	bool DownPressed;
	bool LeftPressed;
	bool RightPressed;
	public bool FarLeftPressed;
	public bool FarRightPressed;
	bool UpHeld;
	bool DownHeld;
	bool LeftHeld;
	bool RightHeld;
#endregion

	void Start () {
        if (!noCactus)
        {
            //Try opening the Serial Stream
            stream.Open();
            if (stream.IsOpen)
            {
                connected = true;
                Debug.Log("Serial Port opened.");
            }
            else
            {
                connected = false;
            }
        }
	}
	
	void Update () {
		inputState = new InputState ();

        //Read button input if connected
		if (connected) {
			stream.Write ("1");
			receivedData = stream.ReadLine ();
			buttonVal = System.Convert.ToInt32 (receivedData, 16);   //convert input hex string to actual hex int
			Debug.Log ((buttonVal - 32 - 15).ToString ());
		} else {
			buttonVal = 0;
		}

		inputState.HeldButtons = checkButtonsHeld (buttonVal);
		inputState.PressedButtons = checkButtonsPressed (buttonVal);
		//Debug.Log ("Up: "+UpPressed+" Down: "+DownPressed+" Left: "+LeftPressed+" Right: "+RightPressed+" FarLeft: "+FarLeftPressed+" FarRight: "+FarRightPressed);
	}

	// returns all gamebuttons, that are held down
	GameButton checkButtonsHeld(int buttonVal){
		UpHeld = ((buttonVal & buttonMasks [0]) != 0) || (noCactus && Input.GetKey(KeyCode.W));
		DownHeld = ((buttonVal & buttonMasks [1]) != 0) || (noCactus && Input.GetKey(KeyCode.S));			
		LeftHeld = ((buttonVal & buttonMasks [2]) != 0) || (noCactus && Input.GetKey(KeyCode.A));
		RightHeld = ((buttonVal & buttonMasks [3]) != 0) || (noCactus && Input.GetKey(KeyCode.D));

		return (UpHeld ? GameButton.Top : GameButton.None) |
			(DownHeld ? GameButton.Bottom : GameButton.None) |
			(LeftHeld ? GameButton.Left : GameButton.None) |
			(RightHeld ? GameButton.Right : GameButton.None);
	}

	// returns all gamebuttons, that were pressed in this frame
	GameButton checkButtonsPressed(int buttonVal){
		// TODO: Stop buttonspam farleft/farright
		FarLeftPressed = ((buttonVal & buttonMasks [4]) != 0) || (noCactus && Input.GetKeyDown(KeyCode.LeftArrow));
		FarRightPressed = ((buttonVal & buttonMasks [5]) != 0) || (noCactus && Input.GetKeyDown(KeyCode.RightArrow));	

		// TODO: Add same for Cactus controller
		UpPressed = noCactus && Input.GetKeyDown(KeyCode.W);
		DownPressed = noCactus && Input.GetKeyDown(KeyCode.S);			
		LeftPressed = noCactus && Input.GetKeyDown(KeyCode.A);
		RightPressed = noCactus && Input.GetKeyDown(KeyCode.D);

		return (UpPressed ? GameButton.Top : GameButton.None) |
		(DownPressed ? GameButton.Bottom : GameButton.None) |
		(LeftPressed ? GameButton.Left : GameButton.None) |
		(RightPressed ? GameButton.Right : GameButton.None);
	}
}
