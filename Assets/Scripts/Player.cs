using UnityEngine;
using System.Collections;
using System.Linq;

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

		// if port is set in main menu: overwrite editor settings
		string _port = PlayerPrefs.GetString ("Player" + ((isPlayer2) ? "2" : "1") + "Port", "");
		if (_port != "") {
			useCactus = (_port != "Keyboard");
			port = _port;
		}

		// when the flag is enabled, try to connect to controller, otherwise fall back to keyboard input
        CactusCommsThread cactusThread;
        if (useCactus && (cactusThread = new CactusCommsThread(port, Time.fixedDeltaTime)).IsConnected)
        {
            cactusThread.Start();

            input = this.gameObject.AddComponent<CactusController>();
            ((CactusController)input).cactusComms = cactusThread;
        }
        else
        {
            input = this.gameObject.AddComponent<HomeController>();
            ((HomeController)input).isPlayer2 = isPlayer2;
        }

        // fix initially focused safe for player 2
        if (isPlayer2)
            focusedSafe = level.generationOptions.safesPerRow - 1;

        level.safes[focusedSafe].SetFocus(isPlayer2, true);
    }

    void Update()
    {
        Vector3 targetPosition;

        // as long as there are active safes left
        if (level.safes.Any(s => s.IsActive))
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
            targetPosition = level.safes[focusedSafe].transform.position + cameraOffset;

            // send input to safe
            level.safes[focusedSafe].SetInputState(isPlayer2, input.GameInputState);
        }
        else
        {
            // no active safes left --> zoom out
            float centerY = level.safes[level.safes.Count / 2].transform.position.y;
            targetPosition = new Vector3(0, centerY, 0) + 2 * cameraOffset;
        }

#if UNITY_EDITORs
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
#endif

        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * cameraSpeed);


    }

    private void nextSafe()
    {
        level.safes[focusedSafe].SetFocus(isPlayer2, false);
        
        if (!isPlayer2)
            focusedSafe = (focusedSafe + 1) % level.safes.Count;
        else
        {
            // check if we need to jump to next row
            if (focusedSafe % level.generationOptions.safesPerRow == 0)
                focusedSafe += 2 * level.generationOptions.safesPerRow - 1;
            // decrease otherwise
            else
                focusedSafe -= 1;

            // stay in bounds
            focusedSafe = (focusedSafe + level.safes.Count) % level.safes.Count;
        }

        level.safes[focusedSafe].SetFocus(isPlayer2, true);
    }

    private void previousSafe()
    {
        level.safes[focusedSafe].SetFocus(isPlayer2, false);

        if (!isPlayer2)
            focusedSafe = (focusedSafe - 1 + level.safes.Count) % level.safes.Count;
        else
        {
            // check if we need to jump to previous row
            if (focusedSafe % level.generationOptions.safesPerRow == level.generationOptions.safesPerRow - 1)
                focusedSafe -= (2 * level.generationOptions.safesPerRow - 1);
            // increase otherwise
            else
                focusedSafe += 1;

            // stay in bounds
            focusedSafe = (focusedSafe + level.safes.Count) % level.safes.Count;
        }

        level.safes[focusedSafe].SetFocus(isPlayer2, true);
    }
}
