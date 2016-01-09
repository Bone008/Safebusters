using UnityEngine;
using System.Collections;

/// <summary>
/// A player playing the game.
/// Each player has focus on one safe, which is where inputs are sent to.
/// </summary>
public class Player : MonoBehaviour
{

    public Level level;
    public CustomButtonReader input;

    // offset of the camera to the focused safe
    public Vector3 cameraOffset;

    // speed, at which the camera changes to a new safes
    public float cameraSpeed;

    // false = player 1, true = player 2
    public bool isPlayer2 = false;

	// index of the safe in focus
	[HideInInspector]
	public int focusedSafe = 0;

    void Start()
    {

    }

    void Update()
    {
        // amount of safes (shorter)
        int safeCount = level.safes.Count;

        // if focusedSafe isnt active anymore -> focus next active one
        while (!level.safes[focusedSafe].IsActive)
        {
            focusedSafe = (focusedSafe + 1) % safeCount;
        }

        // far left/right key pressed -> focus next active safe
        do
        {
            if (input.FarRightPressed)
            {
                focusedSafe = (focusedSafe + 1) % safeCount;
            }
            if (input.FarLeftPressed)
            {
                focusedSafe = (focusedSafe + safeCount - 1) % safeCount;
            }
        } while (!level.safes[focusedSafe].IsActive);

        // smoothly move cam to focussed safe
        transform.position = Vector3.Lerp(transform.position, level.safes[focusedSafe].transform.position + cameraOffset, Time.deltaTime * cameraSpeed);

        // send input to safe
        level.safes[focusedSafe].SetInputState(isPlayer2, input.inputState);
    }

}
