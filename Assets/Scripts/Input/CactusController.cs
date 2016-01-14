using UnityEngine;
using System.Collections;
using System.IO.Ports;
using System.IO;
using System.Linq;
using System.Globalization;
using System;

public class CactusController : MonoBehaviour, InputIF, OutputIF
{

    // called by the player script to determine availability of cactus controller
    // before instantiatin the component
    public static SerialPort TryConnect(string port)
    {
        try
        {
            SerialPort stream = new SerialPort(port, 115200);
            stream.Open();
            return stream;
        }
        catch (IOException e)
        {
            Debug.Log("Controller at " + port + " not available: " + e.Message);
            return null;
        }
    }


    public SerialPort stream;

    private const float ACCEL_GRAVITATION_NORM = 0.5f;

    private InputState inputState;

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

    private bool[] lastKnownLEDStates = new bool[4];

    public InputState GameInputState { get { return inputState; } }
    public bool FarLeftPressed { get { return farLeftPressed; } }
    public bool FarRightPressed { get { return farRightPressed; } }


    void Awake()
    {
        // initialize to empty input state for first frame
        inputState = new InputState { Output = this };
    }

    void Update()
    {
        if (stream == null)
            throw new InvalidOperationException("controller not initialized");
        if (!stream.IsOpen) // no controller connected
            return;

        inputState = collectInput();
    }


    private InputState collectInput()
    {
        InputState inputState = new InputState();
        inputState.Output = this;

        // read button input if connected
        int buttonVal;
        stream.Write("1");
		stream.BaseStream.Flush();
        string response = stream.ReadLine();
        buttonVal = System.Convert.ToInt32(response, 16);   //convert input hex string to actual hex int

        inputState.HeldButtons = checkButtonsHeld(buttonVal);
        inputState.PressedButtons = checkButtonsPressed(buttonVal);

        // read analog input
        stream.Write("4");
		stream.BaseStream.Flush();
        response = stream.ReadLine();
        int[] analogInputs = response.Split(' ').Skip(1).Select(s => Convert.ToInt32(s, 16)).ToArray();
        for (int i = 0; i < 4; i++)
        {
            inputState.SetAnalogInput((GameAnalogInput)i, 1 - (analogInputs[i] / 4096.0f));
        }

        // read microphone
        inputState.SetAnalogInput(GameAnalogInput.Microphone, getMicrophone());

        // read accelerometer
        inputState.Acceleration = getAcceleration();

        return inputState;
    }

    private GameButton checkButtonsHeld(int buttonVal)
    {
        GameButton flags = GameButton.None;

        if ((buttonVal & buttonMasks[0]) != 0)
            flags |= GameButton.Top;
        if ((buttonVal & buttonMasks[1]) != 0)
            flags |= GameButton.Bottom;
        if ((buttonVal & buttonMasks[2]) != 0)
            flags |= GameButton.Left;
        if ((buttonVal & buttonMasks[3]) != 0)
            flags |= GameButton.Right;

        return flags;
    }

    private GameButton checkButtonsPressed(int buttonVal)
    {
        GameButton flags = GameButton.None;

        // Far left
        farLeftPressed = ((buttonVal & buttonMasks[4]) != 0);
        if (farLeftPressed)
        {
            if (farLeftLastPressed)
            {
                farLeftPressed = false;
            }
            farLeftLastPressed = true;
        }
        else if (!farLeftPressed)
        {
            farLeftLastPressed = false;
        }

        // Far right
        farRightPressed = ((buttonVal & buttonMasks[5]) != 0);
        if (farRightPressed)
        {
            if (farRightLastPressed)
            {
                farRightPressed = false;
            }
            farRightLastPressed = true;
        }
        else if (!farRightPressed)
        {
            farRightLastPressed = false;
        }

        bool topPressed, bottomPressed, leftPressed, rightPressed;
        topPressed = (buttonVal & buttonMasks[0]) != 0;
        if (topPressed)
        {
            if (topLastPressed)
            {
                topPressed = false;
            }
            topLastPressed = true;
        }
        else if (!topPressed)
        {
            topLastPressed = false;
        }
        bottomPressed = (buttonVal & buttonMasks[1]) != 0;
        if (bottomPressed)
        {
            if (bottomLastPressed)
            {
                bottomPressed = false;
            }
            bottomLastPressed = true;
        }
        else if (!bottomPressed)
        {
            bottomLastPressed = false;
        }
        leftPressed = (buttonVal & buttonMasks[2]) != 0;
        if (leftPressed)
        {
            if (leftLastPressed)
            {
                leftPressed = false;
            }
            leftLastPressed = true;
        }
        else if (!leftPressed)
        {
            leftLastPressed = false;
        }
        rightPressed = (buttonVal & buttonMasks[3]) != 0;
        if (rightPressed)
        {
            if (rightLastPressed)
            {
                rightPressed = false;
            }
            rightLastPressed = true;
        }
        else if (!rightPressed)
        {
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

    private float getMicrophone()
    {
        stream.Write("s");
		stream.BaseStream.Flush();
        string response = stream.ReadLine(); // format: "RMS: 0.000000"
        return float.Parse(response.Substring(5), CultureInfo.InvariantCulture) / 32768f;
    }

    private Vector3 getAcceleration()
    {
        stream.Write("a");
		stream.BaseStream.Flush();
        string response = stream.ReadLine();
        float[] accelCoords = response.Split(' ').Skip(1).Select(s => Convert.ToSByte(s, 16) / 128.0f).ToArray();
        // swizzle to (-y, z, -x)
        // * z is up direction (map to y)
        // * y is right direction (map to -x)
        // * x is front direction (map to -z)
        return new Vector3(-accelCoords[1], accelCoords[2], -accelCoords[0]) / ACCEL_GRAVITATION_NORM;
    }


    // output
    public void SetEngineIntensity(float intensity)
    {
        if (stream != null && stream.IsOpen)
        {
            int value = Mathf.RoundToInt(Mathf.Clamp01(intensity) * 1000);
            stream.WriteLine("m " + value);
            string response = stream.ReadLine();

            Debug.Assert(response == "Done.", "Controller didn't respond with Done, but with '" + response + "'.");
        }
    }

    public void SetLEDState(int led, bool state)
    {
        if (led < 0 || led > 3)
            throw new ArgumentException("led");

        if (stream != null && stream.IsOpen)
        {
            // swap order because we hold the controller upside down
            led = 3 - led;
            // efficiency: don't send anything when LED already on
            if (lastKnownLEDStates[led] == state)
                return;

            stream.WriteLine("l " + led + " " + (state ? "1" : "0"));
            string response = stream.ReadLine();

            Debug.Assert(response == "Done.", "Controller didn't respond with Done, but with '" + response + "'.");

            lastKnownLEDStates[led] = state;
        }
    }

    void OnDestroy()
    {
        if (stream != null)
        {
            if (stream.IsOpen)
            {
                SetEngineIntensity(0);
                SetLEDState(0, false);
                SetLEDState(1, false);
                SetLEDState(2, false);
                SetLEDState(3, false);
                stream.Close();
            }
            stream = null;
        }
    }

}
