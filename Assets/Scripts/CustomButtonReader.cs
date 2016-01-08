using UnityEngine;
using System;
using System.Collections;
using System.IO.Ports;

public class CustomButtonReader : MonoBehaviour {

    SerialPort stream = new SerialPort("COM3", 115200);
    string receivedData = "EMPTY";

    int[] buttonMasks = new int[]{
		32,  //Nichts
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

	// Use this for initialization
	void Start () {
        stream.Open(); //Open the Serial Stream.
        Debug.Log("Serial Port opened.");
	}
	
	// Update is called once per frame
	void Update () {
        //Read button input
        stream.Write("1");
        receivedData = stream.ReadLine();

        int buttonVal = System.Convert.ToInt32(receivedData, 16);   //convert input hex string to actual hex int
		Debug.Log((buttonVal-32-15).ToString());
		PressButton(buttonVal);
		Debug.Log ("Up: "+UpPressed+" Down: "+DownPressed+" Left: "+LeftPressed+" Right: "+RightPressed+" FarLeft: "+FarLeftPressed+" FarRight: "+FarRightPressed);
	}

	void PressButton(int buttonVal){
		if ((buttonVal & buttonMasks [1]) != 0) {
			UpPressed = true;
		} else {
			UpPressed = false;
		}
		if ((buttonVal & buttonMasks [2]) != 0) {
			DownPressed = true;
		} else {
			DownPressed = false;
		}
		if ((buttonVal & buttonMasks [3]) != 0) {
			LeftPressed = true;
		} else {
			LeftPressed = false;
		}
		if ((buttonVal & buttonMasks [4]) != 0) {
			RightPressed = true;
		} else {
			RightPressed = false;
		}
		if ((buttonVal & buttonMasks [5]) != 0) {
			FarLeftPressed = true;
		} else {
			FarLeftPressed = false;
		}
		if ((buttonVal & buttonMasks [6]) != 0) {
			FarRightPressed = true;
		} else {
			FarRightPressed = false;
		}
	}
}
