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
    private int focusedSafe = 0;

    private Quaternion cameraInitialRotation;

    private InputIF input;

    void Start()
    {
        cameraInitialRotation = transform.rotation;

        // when the flag is enabled, try to connect to controller, otherwise fall back to keyboard input
        System.IO.Ports.SerialPort connection;
        if (useCactus && (connection = CactusController.TryConnect(port)) != null)
        {
            input = this.gameObject.AddComponent<CactusController>();
            ((CactusController)input).stream = connection;
        }
        else
        {
            input = this.gameObject.AddComponent<HomeController>();
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
        if (Input.GetKey(KeyCode.LeftAlt))
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
