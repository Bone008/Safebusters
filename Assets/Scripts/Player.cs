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

    // index of the safe in focus
    [HideInInspector]
    public int focusedSafe = 0;

    private InputIF input;

    void Start()
    {
        if (useCactus)
        {
            this.gameObject.AddComponent<CactusController>();
            input = this.gameObject.GetComponent<CactusController>();
        }
        else
        {
            this.gameObject.AddComponent<HomeController>();
            input = this.gameObject.GetComponent<HomeController>();
        }
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
            if (input.FarRightPressed())
            {
                focusedSafe = (focusedSafe + 1) % safeCount;
            }
            if (input.FarLeftPressed())
            {
                focusedSafe = (focusedSafe + safeCount - 1) % safeCount;
            }
        } while (!level.safes[focusedSafe].IsActive);


        // smoothly move cam to focussed safe (or behind it if cheating)
        Vector3 targetPosition = level.safes[focusedSafe].transform.position + cameraOffset;

        // cheat for testing
        if(Input.GetKey(KeyCode.LeftAlt))
        {
            // move behind the safe
            targetPosition -= 2 * cameraOffset;
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else
        {
            transform.rotation = Quaternion.identity;
        }
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * cameraSpeed);


        // send input to safe
        level.safes[focusedSafe].SetInputState(isPlayer2, input.getInput());
    }

}
