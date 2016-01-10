using UnityEngine;
using System.Collections;
using System.IO.Ports;
using System.IO;
using System.Linq;
using System.Globalization;
using System;

public class CactusController : MonoBehaviour, InputIF {

	private const float ACCEL_GRAVITATION_NORM = 0.5f;

	private InputState inputState = new InputState();
	private SerialPort stream = new SerialPort("COM3", 115200); //TODO: make COM3 settable for each player

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
			inputState.SetAnalogInput ((GameAnalogInput)i, analogInputs[i] / 4096.0f);
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

		// TODO: not sure if works
		farLeftPressed = !farLeftLastPressed && ((buttonVal & buttonMasks [4]) != 0);
		farRightPressed = !farRightLastPressed && ((buttonVal & buttonMasks [5]) != 0);	
		farLeftLastPressed = farLeftPressed;
		farRightLastPressed = farRightPressed;

		// TODO: not sure if works
		if (((lastPressed & GameButton.Top) == 0) && ((buttonVal & buttonMasks [0]) != 0))
			flags |= GameButton.Top;
		if (((lastPressed & GameButton.Bottom) == 0) && ((buttonVal & buttonMasks [1]) != 0))
			flags |= GameButton.Bottom;
		if (((lastPressed & GameButton.Left) == 0) && ((buttonVal & buttonMasks [2]) != 0))
			flags |= GameButton.Left;
		if (((lastPressed & GameButton.Right) == 0) && ((buttonVal & buttonMasks [3]) != 0))
			flags |= GameButton.Right;

		lastPressed =  flags;
		
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
