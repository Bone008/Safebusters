using UnityEngine;
using System.Collections;
using System.IO.Ports;
using System.IO;
using System.Linq;
using System.Globalization;
using System;

public class CactusController : MonoBehaviour, InputIF {

	public string port;

	private const float ACCEL_GRAVITATION_NORM = 0.5f;

	private InputState inputState = new InputState();
	private SerialPort stream; //TODO: make COM3 settable for each player

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

	private bool farLeftLastPressed = false;
	private bool farRightLastPressed = false;
	private bool leftLastPressed = false;
	private bool rightLastPressed = false;
	private bool topLastPressed = false;
	private bool bottomLastPressed = false;

    public InputState GameInputState { get { return inputState; } }
    public bool FarLeftPressed { get { return farLeftPressed; } }
    public bool FarRightPressed { get { return farRightPressed; } }


	void Start(){
		stream = new SerialPort(port, 115200);
		try { stream.Open(); }
		catch (IOException e)
		{
			Debug.Log("I/O error while trying to connect to controller: " + e.Message);
		}
	}

	void Update(){
		if (stream == null)
			throw new InvalidOperationException("controller not initialized");
		if (!stream.IsOpen) // no controller connected
			return;
		
		inputState = collectInput ();
	}


	private InputState collectInput(){
		InputState inputState = new InputState ();

		// read button input if connected
		int buttonVal;
		stream.Write ("1");
		string response = stream.ReadLine ();
		buttonVal = System.Convert.ToInt32 (response, 16);   //convert input hex string to actual hex int

		inputState.HeldButtons = checkButtonsHeld (buttonVal);
		inputState.PressedButtons = checkButtonsPressed (buttonVal);

		// read analog input
		stream.Write("4");
		response = stream.ReadLine();
		int[] analogInputs = response.Split(' ').Skip(1).Select(s => Convert.ToInt32(s, 16)).ToArray();
		for (int i = 0; i < 4; i++) {
			inputState.SetAnalogInput ((GameAnalogInput)i, 1 - (analogInputs[i] / 4096.0f));
		}

		// read microphone
		inputState.SetAnalogInput (GameAnalogInput.Microphone, getMicrophone());

		// read accelerometer
		inputState.Acceleration = getAcceleration();

		return inputState;
	}

	private GameButton checkButtonsHeld (int buttonVal){
		GameButton flags = GameButton.None;

		if ((buttonVal & buttonMasks [0]) != 0)
			flags |= GameButton.Top;
		if ((buttonVal & buttonMasks [1]) != 0)
			flags |= GameButton.Bottom;
		if ((buttonVal & buttonMasks [2]) != 0)
			flags |= GameButton.Left;
		if ((buttonVal & buttonMasks [3]) != 0)
			flags |= GameButton.Right;

		return flags;
	}

	private GameButton checkButtonsPressed (int buttonVal){
		GameButton flags = GameButton.None;

		// Far left
		farLeftPressed = ((buttonVal & buttonMasks [4]) != 0);
		if (farLeftPressed) {
			if (farLeftLastPressed) {
				farLeftPressed = false;
			}
			farLeftLastPressed = true;
		}else if (!farLeftPressed) {
			farLeftLastPressed = false;
		}

		// Far right
		farRightPressed = ((buttonVal & buttonMasks [5]) != 0);	
		if (farRightPressed) {
			if (farRightLastPressed) {
				farRightPressed = false;
			}
			farRightLastPressed = true;
		} else if (!farRightPressed) {
			farRightLastPressed = false;
		}

		bool topPressed, bottomPressed, leftPressed, rightPressed;
		topPressed = (buttonVal & buttonMasks [0]) != 0;
		if (topPressed) {
			if (topLastPressed) {
				topPressed = false;
			}
			topLastPressed = true;
		} else if (!topPressed) {
			topLastPressed = false;
		}
		bottomPressed = (buttonVal & buttonMasks [1]) != 0;
		if (bottomPressed) {
			if (bottomLastPressed) {
				bottomPressed = false;
			}
			bottomLastPressed = true;
		} else if (!bottomPressed) {
			bottomLastPressed = false;
		}
		leftPressed = (buttonVal & buttonMasks [2]) != 0;
		if (leftPressed) {
			if (leftLastPressed) {
				leftPressed = false;
			}
			leftLastPressed = true;
		} else if (!leftPressed) {
			leftLastPressed = false;
		}
		rightPressed = (buttonVal & buttonMasks [3]) != 0;
		if (rightPressed) {
			if (rightLastPressed) {
				rightPressed = false;
			}
			rightLastPressed = true;
		} else if (!rightPressed) {
			rightLastPressed = false;
		}
		// TODO: not sure if works
		if (topPressed)
			flags |= GameButton.Top;
		if (bottomPressed)
			flags |= GameButton.Bottom;
		if (leftPressed)
			flags |= GameButton.Left;
		if (rightPressed)
			flags |= GameButton.Right;

		return flags;
	}

	private float getMicrophone() {
		stream.Write("s");
		string response = stream.ReadLine(); // format: "RMS: 0.000000"
		return float.Parse(response.Substring(5), CultureInfo.InvariantCulture) / 32768f;
	}

	private Vector3 getAcceleration() {
		stream.Write("a");
		string response = stream.ReadLine();
		float[] accelCoords = response.Split(' ').Skip(1).Select(s => Convert.ToSByte(s, 16) / 128.0f).ToArray();
		// swizzle to (-y, z, -x)
		// * z is up direction (map to y)
		// * y is right direction (map to -x)
		// * x is front direction (map to -z)
		return new Vector3(-accelCoords[1], accelCoords[2], -accelCoords[0]) / ACCEL_GRAVITATION_NORM;
	}

	void OnDestroy()
	{
		if (stream != null){
			if (stream.IsOpen){
				stream.Close();
			}
			stream = null;
		}
	}

}
