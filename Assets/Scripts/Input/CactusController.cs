using UnityEngine;
using System.Collections;
using System.IO.Ports;
using System.IO;
using System.Linq;
using System.Globalization;
using System;
using System.Threading;

public class CactusController : MonoBehaviour, InputIF, OutputIF
{
    private const float ACCEL_GRAVITATION_NORM = 0.5f;


    public CactusCommsThread cactusComms;

    private InputState inputState;

    private bool farLeftPressed = false;
    private bool farRightPressed = false;

    private bool farLeftLastPressed = false;
    private bool farRightLastPressed = false;

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
        inputState = new InputState();
        inputState.Output = this;

        CactusCommsThread.InSnapshot controller = cactusComms.InputSnapshot;

        // TODO complete pressedButtons and stuff
        farLeftPressed = ((controller.heldButtons & CactusCommsThread.CactusButton.FarLeft) != 0) && !farLeftLastPressed;
        farRightPressed = ((controller.heldButtons & CactusCommsThread.CactusButton.FarRight) != 0) && !farRightLastPressed;
        farLeftLastPressed = farLeftPressed;
        farRightLastPressed = farRightPressed;

        GameButton held = GameButton.None;
        if ((controller.heldButtons & CactusCommsThread.CactusButton.Left) != 0)
            held |= GameButton.Left;
        if ((controller.heldButtons & CactusCommsThread.CactusButton.Right) != 0)
            held |= GameButton.Right;
        if ((controller.heldButtons & CactusCommsThread.CactusButton.Up) != 0)
            held |= GameButton.Top;
        if ((controller.heldButtons & CactusCommsThread.CactusButton.Down) != 0)
            held |= GameButton.Bottom;

        inputState.HeldButtons = held;

        // sliders & knobs
        for (int i = 0; i < controller.analogInputs.Length; i++)
            inputState.SetAnalogInput((GameAnalogInput)i, controller.analogInputs[i]);
        // microphone
        inputState.SetAnalogInput(GameAnalogInput.Microphone, controller.microphone);
        // acceleration
        inputState.Acceleration = controller.acceleration;
    }


    // output
    public void SetEngineIntensity(float intensity)
    {
        // TODO feed data into OutSnapshot
    }

    public void SetLEDState(int led, bool state)
    {
        // TODO feed data into OutSnapshot
    }

    void OnDestroy()
    {
        cactusComms.StopNow(TimeSpan.FromSeconds(2));
    }

}
