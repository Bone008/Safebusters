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

    protected override void InitChallenge()
    {
		// GameButtons as list
        List<GameButton> values = new List<GameButton> { GameButton.Left, GameButton.Top, GameButton.Right, GameButton.Bottom };

		// random amount of buttons to press to solve the challenge
		int numButtonsToPress = UnityEngine.Random.Range(1, values.Count + 1);

        // pick [numButtonsToPress] random GameButtons as buttonsToPress
        buttonsToPress = GameButton.None;
        for (int i = 0; i < numButtonsToPress; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, values.Count);
            buttonsToPress |= values[randomIndex];
            values.RemoveAt(randomIndex);
        }


        // colorize placeholder cubes to show buttons to press
        // when actual models for the button challenge are in, this should be replaced
		if ((buttonsToPress & GameButton.Left) != 0)
            backGameObject.transform.GetChild(0).GetComponent<UnityEngine.UI.Image>().color = Color.green;
        if ((buttonsToPress & GameButton.Top) != 0)
            backGameObject.transform.GetChild(1).GetComponent<UnityEngine.UI.Image>().color = Color.green;
        if ((buttonsToPress & GameButton.Bottom) != 0)
            backGameObject.transform.GetChild(2).GetComponent<UnityEngine.UI.Image>().color = Color.green;
        if ((buttonsToPress & GameButton.Right) != 0)
            backGameObject.transform.GetChild(3).GetComponent<UnityEngine.UI.Image>().color = Color.green;
    }

    void Update()
    {
		// TODO: remove?
        if (!hasFocusFront)
            return;

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
                    safe.SolveChallenge();
                else
                    safe.FailChallenge();
            }
        }
    }
}
