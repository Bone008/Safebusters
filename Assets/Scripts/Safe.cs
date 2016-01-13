using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class Safe : MonoBehaviour
{

    public AnimationCurve doorOpeningCurve;
    public float doorOpeningDuration;

    [Header("Children mappings")]
    public Renderer frameRenderer;
    public Transform doorAnchor;
    public Transform frontAnchor;
    public Transform frontAnchorBottomRight;
    public Transform backAnchor;
    public GameObject backDecorated;
    public GameObject backUndecorated;
    [Header("Timer controls")]
    public Text timerDigit1;
    public Text timerDigit2;
    public Text timerDot;

    [HideInInspector]
    public AbstractChallenge challenge;

    private bool backwards = false;
    private float maxTimer;
    private int numberOfSafesToActivate;

    private Color displayColor;
    private bool active = true;
    private float remainingTime;
    private bool open = false;

    public bool IsActive { get { return active; } }
    public bool IsOpen { get { return open; } }

    void Start()
    {
    }

    void Update()
    {
        if (active)
        {
            // count down timer
            remainingTime -= Time.deltaTime;

            if (remainingTime < 0)
                // TODO lose game; for now just reset to max value (easier testing when you're not constantly losing ...)
                remainingTime = maxTimer;

            UpdateTimerText();
        }
        else {
            timerDigit1.text = "";
            timerDigit2.text = "";
            timerDot.enabled = true;
            timerDot.text = "OFF";
        }
    }


    public void SetInputState(bool isPlayer2, InputState state)
    {
        if (isPlayer2 == backwards)
            challenge.SetFrontInput(state);
        else
            challenge.SetBackInput(state);
    }

    public void SolveChallenge()
    {
        Debug.Log("challenge solved!");
        // open door
        //doorAnchor.localRotation = Quaternion.Euler(0, -90, 0);
        StartCoroutine(RotateSafeDoorSmoothly());
        
        open = true;
        SetActive(false);
        challenge.enabled = false;

        //Activating new safes of same coloring
        Level lvl = GameObject.FindGameObjectWithTag("GameController").GetComponent<Level>();   //didn't want to add another variable at the top, so I'm using a tag
        lvl.ActivateNewSafes(displayColor, numberOfSafesToActivate);
    }
    public void FailChallenge()
    {
        Debug.Log("lost a life!");
    }

    public void SpawnChallengeObjects(GameObject frontPrefab, GameObject backPrefab, bool decoratedBack)
    {
        if (frontPrefab != null)
        {
            GameObject front = (GameObject)Instantiate(frontPrefab, Vector3.zero, Quaternion.identity);
            front.transform.SetParent(frontAnchor, false);
            challenge.frontGameObject = front;
        }
        if (backPrefab != null)
        {
            GameObject back = (GameObject)Instantiate(backPrefab, Vector3.zero, Quaternion.identity);
            back.transform.SetParent(backAnchor, false);
            challenge.backGameObject = back;
        }

        backDecorated.SetActive(decoratedBack);
        backUndecorated.SetActive(!decoratedBack);
    }

    public void SetBackwards(bool flag)
    {
        backwards = flag;
        // note that the model is facing backwards by default, so it actually has to be rotated when NOT backwards
        transform.localRotation = (backwards ? Quaternion.identity : Quaternion.Euler(0, 180, 0));
    }

    public void SetDisplayColor(Color color)
    {
        displayColor = color;
        frameRenderer.material.color = color;
    }

    public void SetNumberOfSafesToActivate(int num)
    {
        numberOfSafesToActivate = num;
    }

    public void SetActive(bool flag)
    {
        active = flag;
    }

    public void SetFocus(bool isPlayer2, bool flag)
    {
        if (isPlayer2 == backwards)
            challenge.SetFrontFocus(flag);
        else
            challenge.SetBackFocus(flag);
    }

    public void SetMaxTimer(float time)
    {
        maxTimer = time;
        remainingTime = time;
        UpdateTimerText();
    }

    private void UpdateTimerText()
    {
        if (remainingTime >= 10)
        {
            // show whole seconds as a two digit number
            timerDigit1.text = "" + Mathf.FloorToInt(remainingTime / 10);
            timerDigit2.text = "" + Mathf.FloorToInt(remainingTime % 10);
            timerDot.enabled = false;
        }
        else
        {
            // show seconds and tenths of a second
            timerDigit1.text = "" + Mathf.FloorToInt(remainingTime);
            timerDigit2.text = "" + Mathf.FloorToInt(remainingTime * 10 % 10);
            timerDot.enabled = true;
        }
    }

    public Bounds GetFrameBounds()
    {
        return frameRenderer.bounds;
    }

    public Color GetDisplayColor()
    {
        return displayColor;
    }

    /// <summary>Returns the width of the area on the safe usable by challenges on the front side. Note that this returns a negative value.</summary>
    public float GetUsableFrontWidth()
    {
        return frontAnchorBottomRight.localPosition.x - frontAnchor.localPosition.x;
    }

    /// <summary>Returns the height of the area on the safe usable by challenges on the front side. Note that this returns a negative value.</summary>
    public float GetUsableFrontHeight()
    {
        return frontAnchorBottomRight.localPosition.y - frontAnchor.localPosition.y;
    }

    private IEnumerator RotateSafeDoorSmoothly()
    {
        float passedTime = 0;
        // animate for [doorOpeningDuration] seconds
        while (passedTime < doorOpeningDuration)
        {
            // apply curve to [passedTime]
            doorAnchor.localRotation = Quaternion.Euler(0, -90 * doorOpeningCurve.Evaluate(passedTime / doorOpeningDuration), 0);
            passedTime += Time.deltaTime;
            yield return null;
        }

        // final step --> fully open
        doorAnchor.localRotation = Quaternion.Euler(0, -90, 0);
    }

}
