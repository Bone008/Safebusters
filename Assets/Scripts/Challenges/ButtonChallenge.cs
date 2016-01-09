using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class ButtonChallenge : AbstractChallenge
{
    // time that you need to hold down the buttons to confirm your input
    private const float PRESS_TIME_TO_CONFIRM = 2;

    private GameButton buttonsToPress;
    private GameButton lastHeld = GameButton.None;
    private float heldTime = 0;
    private bool inputLocked = false;

    private LampController frontLampLeft;
    private LampController frontLampRight;
    private LampController frontLampTop;
    private LampController frontLampBottom;
    private LampController[] allFrontLamps; // store in array as well for easy access

    protected override void InitChallenge()
    {
        GameButton allButtons = GameButton.Left | GameButton.Top | GameButton.Right | GameButton.Bottom;
        // because buttons are flags, any number between 1 and allButtons represents a subset of buttons to press;
        // this also provides a uniform distribution of all possible combinations
        buttonsToPress = (GameButton)UnityEngine.Random.Range(1, (int)allButtons + 1);

        // display which buttons to press on the back
        backGameObject.GetComponentInChildren<FourButtonsController>().SetHighlightedButtons(buttonsToPress);

        // assign relevant game objects on the front
        frontLampLeft = frontGameObject.transform.Find("lamp_left").GetComponent<LampController>();
        frontLampRight = frontGameObject.transform.Find("lamp_right").GetComponent<LampController>();
        frontLampTop = frontGameObject.transform.Find("lamp_top").GetComponent<LampController>();
        frontLampBottom = frontGameObject.transform.Find("lamp_bottom").GetComponent<LampController>();
        allFrontLamps = new LampController[] { frontLampLeft, frontLampRight, frontLampTop, frontLampBottom };

        SetFrontLightColor(Color.yellow);
    }

    void Update()
    {
        // TODO: remove?
        if (!hasFocusFront)
            return;

        GameButton held = frontInputState.HeldButtons;

        if (inputLocked)
        {
            // treat as no input
            held = GameButton.None;
        }
        else
        {
            // update front LEDs
            frontLampLeft.SetTurnedOn((frontInputState.HeldButtons & GameButton.Left) != 0);
            frontLampRight.SetTurnedOn((frontInputState.HeldButtons & GameButton.Right) != 0);
            frontLampTop.SetTurnedOn((frontInputState.HeldButtons & GameButton.Top) != 0);
            frontLampBottom.SetTurnedOn((frontInputState.HeldButtons & GameButton.Bottom) != 0);
        }

        // buttons changed --> reset timer
        if (frontInputState.HeldButtons != lastHeld)
        {
            lastHeld = frontInputState.HeldButtons;
            heldTime = 0;
        }

        if (lastHeld != GameButton.None)
        {
            // player is holding down buttons --> increase timer
            heldTime += Time.deltaTime;

            // timer reached threshold --> evaluate
            if (heldTime >= PRESS_TIME_TO_CONFIRM)
            {
                heldTime = 0;
                if (lastHeld == buttonsToPress)
                {
                    SetFrontLightColor(Color.green);
                    safe.SolveChallenge();
                }
                else
                {
                    StartCoroutine(PlayFailEffect());
                    safe.FailChallenge();
                }
            }
        }
    }

    private IEnumerator PlayFailEffect()
    {
        SetFrontLightColor(Color.red);

        inputLocked = true;
        yield return new WaitForSeconds(0.6f);
        inputLocked = false;

        // reset back to regular state
        foreach (var lamp in allFrontLamps)
            lamp.SetTurnedOn(false);
        SetFrontLightColor(Color.yellow);
    }

    private void SetFrontLightColor(Color color)
    {
        foreach (var lamp in allFrontLamps)
            lamp.SetLightColor(color);
    }

}
