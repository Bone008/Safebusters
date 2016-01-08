using UnityEngine;
using System;
using System.Collections;
using System.IO.Ports;

public class CustomButtonReader : MonoBehaviour {

	public bool noCactus = true;

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

#region publicly available button repots
	public bool UpPressed;
	public bool DownPressed;
	public bool LeftPressed;
	public bool RightPressed;
	public bool FarLeftPressed;
	public bool FarRightPressed;
#endregion

	void Start () {
		//Try opening the Serial Stream
        stream.Open(); 
		if (stream.IsOpen) {
			connected = true;
			Debug.Log ("Serial Port opened.");
		} else {
			connected = false;
		}
	}
	
	void Update () {
        //Read button input if connected
		if (connected) {
			stream.Write ("1");
			receivedData = stream.ReadLine ();
			buttonVal = System.Convert.ToInt32 (receivedData, 16);   //convert input hex string to actual hex int
			Debug.Log ((buttonVal - 32 - 15).ToString ());
		} else {
			buttonVal = 0;
		}
		checkButtonsPressed (buttonVal);
		Debug.Log ("Up: "+UpPressed+" Down: "+DownPressed+" Left: "+LeftPressed+" Right: "+RightPressed+" FarLeft: "+FarLeftPressed+" FarRight: "+FarRightPressed);
	}

	void checkButtonsPressed(int buttonVal){
		UpPressed = ((buttonVal & buttonMasks [0]) != 0) || (noCactus && Input.GetKeyDown(KeyCode.A));
		DownPressed = ((buttonVal & buttonMasks [1]) != 0) || (noCactus && Input.GetKeyDown(KeyCode.S));			
		LeftPressed = ((buttonVal & buttonMasks [2]) != 0) || (noCactus && Input.GetKeyDown(KeyCode.A));
		RightPressed = ((buttonVal & buttonMasks [3]) != 0) || (noCactus && Input.GetKeyDown(KeyCode.D));
		FarLeftPressed = ((buttonVal & buttonMasks [4]) != 0) || (noCactus && Input.GetKeyDown(KeyCode.LeftArrow));
		FarRightPressed = ((buttonVal & buttonMasks [5]) != 0) || (noCactus && Input.GetKeyDown(KeyCode.RightArrow));			
	}
}
