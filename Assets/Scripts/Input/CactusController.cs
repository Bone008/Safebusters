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
    private CactusCommsThread.OutSnapshot outState = new CactusCommsThread.OutSnapshot();

    private InputState inputState;
    private bool farLeftPressed = false;
    private bool farRightPressed = false;

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

        // pressed buttons
        farLeftPressed = farRightPressed = false;
        GameButton pressed = GameButton.None;

        CactusCommsThread.CactusButton? pressedButton;
        while((pressedButton = cactusComms.PollButtonPress()) != null)
        {
            switch(pressedButton.Value)
            {
                case CactusCommsThread.CactusButton.FarLeft: farLeftPressed = true; break;
                case CactusCommsThread.CactusButton.FarRight: farRightPressed = true; break;
                case CactusCommsThread.CactusButton.Left: pressed |= GameButton.Left; break;
                case CactusCommsThread.CactusButton.Right: pressed |= GameButton.Right; break;
                case CactusCommsThread.CactusButton.Up: pressed |= GameButton.Top; break;
                case CactusCommsThread.CactusButton.Down: pressed |= GameButton.Bottom; break;
            }
        }

        inputState.PressedButtons = pressed;

        CactusCommsThread.InSnapshot controller = cactusComms.InputSnapshot;

        // held buttons
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

    void LateUpdate()
    {
        // commit output to the comms thread
        // this is done in LateUpdate to make sure all challenges have been updated this frame so the state is fresh
        cactusComms.OutputSnapshot = outState;
    }


    // output
    public void SetEngineIntensity(float intensity)
    {
        if (intensity < 0 || intensity > 1)
            throw new ArgumentOutOfRangeException("intensity", "intensity has to be between 0.0 and 1.0");

        outState.engineIntensity = intensity;
    }

    public void SetLEDState(int led, bool state)
    {
        if (led < 0 || led > 3)
            throw new ArgumentOutOfRangeException("led", "led index has to be between 0 and 3");

        // set or clear appropiate bit
        if (state)
            outState.ledFlags |= (1 << led);
        else
            outState.ledFlags &= ~(1 << led);
    }

    void OnDestroy()
    {
        cactusComms.StopNow(TimeSpan.FromSeconds(2));
    }

}
