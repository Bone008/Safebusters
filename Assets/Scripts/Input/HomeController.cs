using UnityEngine;
using System.Collections;

public class HomeController : MonoBehaviour, InputIF
{

    private InputState inputState = new InputState();

    public InputState GameInputState { get { return inputState; } }
    public bool FarLeftPressed { get { return Input.GetKeyDown(KeyCode.LeftArrow); } }
    public bool FarRightPressed { get { return Input.GetKeyDown(KeyCode.RightArrow); } }

    void Update()
    {
        inputState = collectInput();
    }

    private InputState collectInput()
    {
        InputState inputState = new InputState();

        inputState.HeldButtons = checkButtonsHeld();
        inputState.PressedButtons = checkButtonsPressed();

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
