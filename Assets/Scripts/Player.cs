using UnityEngine;
using System.Collections;

/// <summary>
/// A player playing the game.
/// Each player has focus on one safe, which is where inputs are sent to.
/// </summary>
public class Player : MonoBehaviour
{

    public Level level;

    // offset of the camera to the focused safe
    public Vector3 cameraOffset;

    // speed, at which the camera changes to a new safes
    public float cameraSpeed;

    // false = player 1, true = player 2
    public bool isPlayer2 = false;

    public bool useCactus = false;
	public string port;

    // index of the safe in focus
    [HideInInspector]
    public int focusedSafe = 0;

    private Quaternion cameraInitialRotation;

    private InputIF input;

    void Start()
    {
        cameraInitialRotation = transform.rotation;

        if (useCactus)
        {
            this.gameObject.AddComponent<CactusController>();
            input = this.gameObject.GetComponent<CactusController>();
			((CactusController)input).port = port;
        }
        else
        {
            this.gameObject.AddComponent<HomeController>();
            input = this.gameObject.GetComponent<HomeController>();
        }
    }

    void Update()
    {
        // if focusedSafe isnt active anymore -> focus next active one
        while (!level.safes[focusedSafe].IsActive)
        {
            nextSafe();
        }

        // far left/right key pressed -> focus next active safe
        do
        {
            if (input.FarRightPressed)
            {
                nextSafe();
            }
            if (input.FarLeftPressed)
            {
                previousSafe();
            }
        } while (!level.safes[focusedSafe].IsActive);


        // smoothly move cam to focussed safe (or behind it if cheating)
        Vector3 targetPosition = level.safes[focusedSafe].transform.position + cameraOffset;

        // cheat for testing
        if(Input.GetKey(KeyCode.LeftAlt))
        {
            // move behind the safe
            targetPosition -= 2 * cameraOffset;
            transform.rotation = Quaternion.Euler(0, 180, 0) * cameraInitialRotation;
        }
        else
        {
            transform.rotation = cameraInitialRotation;
        }
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * cameraSpeed);


        // send input to safe
        level.safes[focusedSafe].SetInputState(isPlayer2, input.GameInputState);
    }

    private void nextSafe()
    {
        level.safes[focusedSafe].SetFocus(isPlayer2, false);
        if (!isPlayer2)
            focusedSafe = (focusedSafe + 1) % level.safes.Count;
        else
            focusedSafe = (focusedSafe + level.safes.Count - 1) % level.safes.Count;
        level.safes[focusedSafe].SetFocus(isPlayer2, true);
    }

    private void previousSafe()
    {
        level.safes[focusedSafe].SetFocus(isPlayer2, false);
        if (!isPlayer2)
            focusedSafe = (focusedSafe + level.safes.Count - 1) % level.safes.Count;
        else
            focusedSafe = (focusedSafe + 1) % level.safes.Count;
        level.safes[focusedSafe].SetFocus(isPlayer2, true);
    }
}
