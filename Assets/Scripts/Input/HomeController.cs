using UnityEngine;
using System.Collections;

public class HomeController : MonoBehaviour, InputIF, OutputIF
{

    private InputState inputState;

    public bool isPlayer2 = false;

    public InputState GameInputState { get { return inputState; } }
    public bool FarLeftPressed { get { return (isPlayer2)? Input.GetKeyDown(KeyCode.Comma) : Input.GetKeyDown(KeyCode.Y); } }
    public bool FarRightPressed { get { return (isPlayer2)? Input.GetKeyDown(KeyCode.Period) : Input.GetKeyDown(KeyCode.X); } }


	private float leftSliderValue = 0.0f;
	private float rightSliderValue = 0.0f;

	private float leftKnobValue = 0.0f;
	private float rightKnobValue = 0.0f;

	private float microphoneValue = 0.0f;

	private Vector3 accelerometerValue = Vector3.zero;

    void Awake()
    {
        // initialize to empty input state for first frame
        inputState = new InputState { Output = this };
    }

    void Update()
    {
        inputState = collectInput();
    }

	void OnGUI() {
        int offsetHorizontal = 0;
        
        if (isPlayer2)
        {
            offsetHorizontal = Screen.width / 2;
        }

		// top-left corner: knobs and slider (cactus layout)
		leftSliderValue = GUI.VerticalSlider (new Rect (offsetHorizontal + 20, 25, 15, 100), leftSliderValue, 1.0f, 0.0f);
		rightSliderValue = GUI.VerticalSlider (new Rect (offsetHorizontal + 65, 25, 15, 100), rightSliderValue, 1.0f, 0.0f);

		leftKnobValue = GUI.HorizontalSlider (new Rect (offsetHorizontal + 5, 5, 40, 15), leftKnobValue, 0.0f, 1.0f);
		rightKnobValue = GUI.HorizontalSlider (new Rect (offsetHorizontal + 50, 5, 40, 15), rightKnobValue, 0.0f, 1.0f);

		// top-right corner: microphone
		microphoneValue = GUI.VerticalSlider (new Rect (Screen.width / 2 + offsetHorizontal - 20, 5, 15, 100), microphoneValue, 1.0f, 0.0f);

		// bottom-left corner: accelerometer
		accelerometerValue.x = GUI.VerticalSlider (new Rect (offsetHorizontal + 5, Screen.height - 105, 15, 100), accelerometerValue.x, 1.0f, -1.0f);
		accelerometerValue.y = GUI.VerticalSlider (new Rect (offsetHorizontal + 25, Screen.height - 105, 15, 100), accelerometerValue.y, 1.0f, -1.0f);
		accelerometerValue.z = GUI.VerticalSlider (new Rect (offsetHorizontal + 45, Screen.height - 105, 15, 100), accelerometerValue.z, 1.0f, -1.0f);
	}

    private InputState collectInput()
    {
        InputState inputState = new InputState();
        inputState.Output = this;

        inputState.HeldButtons = checkButtonsHeld();
        inputState.PressedButtons = checkButtonsPressed();

		inputState.SetAnalogInput (GameAnalogInput.LeftSlider, leftSliderValue);
		inputState.SetAnalogInput (GameAnalogInput.RightSlider, rightSliderValue);

		inputState.SetAnalogInput (GameAnalogInput.LeftKnob, leftKnobValue);
		inputState.SetAnalogInput (GameAnalogInput.RightKnob, rightKnobValue);

		inputState.SetAnalogInput (GameAnalogInput.Microphone, microphoneValue);

		inputState.Acceleration = accelerometerValue;

        return inputState;
    }

    private GameButton checkButtonsHeld()
    {
        GameButton flags = GameButton.None;

        if (isPlayer2)
        {
            if (Input.GetKey(KeyCode.I))
                flags |= GameButton.Top;
            if (Input.GetKey(KeyCode.J))
                flags |= GameButton.Left;
            if (Input.GetKey(KeyCode.K))
                flags |= GameButton.Bottom;
            if (Input.GetKey(KeyCode.L))
                flags |= GameButton.Right;
        }
        else
        {
            if (Input.GetKey(KeyCode.W))
                flags |= GameButton.Top;
            if (Input.GetKey(KeyCode.A))
                flags |= GameButton.Left;
            if (Input.GetKey(KeyCode.S))
                flags |= GameButton.Bottom;
            if (Input.GetKey(KeyCode.D))
                flags |= GameButton.Right;
        }        

        return flags;
    }

    // returns all gamebuttons, that were pressed in this frame
    private GameButton checkButtonsPressed()
    {
        GameButton flags = GameButton.None;

        if (isPlayer2)
        {
            if (Input.GetKeyDown(KeyCode.I))
                flags |= GameButton.Top;
            if (Input.GetKeyDown(KeyCode.J))
                flags |= GameButton.Left;
            if (Input.GetKeyDown(KeyCode.K))
                flags |= GameButton.Bottom;
            if (Input.GetKeyDown(KeyCode.L))
                flags |= GameButton.Right;
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.W))
                flags |= GameButton.Top;
            if (Input.GetKeyDown(KeyCode.A))
                flags |= GameButton.Left;
            if (Input.GetKeyDown(KeyCode.S))
                flags |= GameButton.Bottom;
            if (Input.GetKeyDown(KeyCode.D))
                flags |= GameButton.Right;
        }        

        return flags;
    }

    public void SetEngineIntensity(float intensity)
    {
        print("Engine: " + intensity);
        // nop
    }

    public void SetLEDState(int led, bool state)
    {
        // nop
        print("LED #" + led + ": " + state);
    }

}
