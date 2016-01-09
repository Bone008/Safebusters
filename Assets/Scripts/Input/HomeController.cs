using UnityEngine;
using System.Collections;

public class HomeController : MonoBehaviour, InputIF
{

    private InputState inputState = new InputState();

    public InputState GameInputState { get { return inputState; } }
    public bool FarLeftPressed { get { return Input.GetKeyDown(KeyCode.LeftArrow); } }
    public bool FarRightPressed { get { return Input.GetKeyDown(KeyCode.RightArrow); } }

	private float leftSliderValue = 0.0f;
	private float rightSliderValue = 0.0f;

	private float leftKnobValue = 0.0f;
	private float rightKnobValue = 0.0f;

	private float microphoneValue = 0.0f;

    void Update()
    {
        inputState = collectInput();
    }

	void OnGUI() {
		leftSliderValue = GUI.VerticalSlider (new Rect (20, 25, 15, 100), leftSliderValue, 1.0f, 0.0f);
		rightSliderValue = GUI.VerticalSlider (new Rect (65, 25, 15, 100), rightSliderValue, 1.0f, 0.0f);

		leftKnobValue = GUI.HorizontalSlider (new Rect (5, 5, 40, 15), leftKnobValue, 0.0f, 1.0f);
		rightKnobValue = GUI.HorizontalSlider (new Rect (50, 5, 40, 15), rightKnobValue, 0.0f, 1.0f);

		microphoneValue = GUI.VerticalSlider (new Rect (Screen.width - 20, 5, 15, 100), microphoneValue, 1.0f, 0.0f);
	}

    private InputState collectInput()
    {
        InputState inputState = new InputState();

        inputState.HeldButtons = checkButtonsHeld();
        inputState.PressedButtons = checkButtonsPressed();

		inputState.SetAnalogInput (GameAnalogInput.LeftSlider, leftSliderValue);
		inputState.SetAnalogInput (GameAnalogInput.RightSlider, rightSliderValue);

		inputState.SetAnalogInput (GameAnalogInput.LeftKnob, leftKnobValue);
		inputState.SetAnalogInput (GameAnalogInput.RightKnob, rightSliderValue);

		inputState.SetAnalogInput (GameAnalogInput.Microphone, microphoneValue);

        return inputState;
    }

    private GameButton checkButtonsHeld()
    {
        GameButton flags = GameButton.None;

        if (Input.GetKey(KeyCode.W))
            flags |= GameButton.Top;
        if (Input.GetKey(KeyCode.A))
            flags |= GameButton.Left;
        if (Input.GetKey(KeyCode.S))
            flags |= GameButton.Bottom;
        if (Input.GetKey(KeyCode.D))
            flags |= GameButton.Right;

        return flags;
    }

    // returns all gamebuttons, that were pressed in this frame
    private GameButton checkButtonsPressed()
    {
        GameButton flags = GameButton.None;

        if (Input.GetKeyDown(KeyCode.W))
            flags |= GameButton.Top;
        if (Input.GetKeyDown(KeyCode.A))
            flags |= GameButton.Left;
        if (Input.GetKeyDown(KeyCode.S))
            flags |= GameButton.Bottom;
        if (Input.GetKeyDown(KeyCode.D))
            flags |= GameButton.Right;

        return flags;
    }

}
