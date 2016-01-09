using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class ButtonSequenceChallenge : AbstractChallenge
{
	// time that you need to hold down the buttons to confirm your input
	private const float PRESS_TIME_TO_CONFIRM = 2;
	private const int LONGEST_SEQUENCE_TO_PRESS = 6;
	private List<GameButton> buttonsToPress;
	private GameButton lastHeld = GameButton.None;
	private float heldTime = 0;

	protected override void InitChallenge()
	{
		// GameButtons as list
		List<GameButton> values = Enum.GetValues(typeof(GameButton)).Cast<GameButton>().ToList();

		// random amount of buttons (minimum 2) to press to solve the challenge
		int numButtonsToPress = UnityEngine.Random.Range(2, LONGEST_SEQUENCE_TO_PRESS + 1);

		// pick [numButtonsToPress] random GameButtons as buttonsToPress
		buttonsToPress = new List<GameButton> ();
		for (int i = 0; i < numButtonsToPress; i++)
		{
			int randomIndex = UnityEngine.Random.Range(1, values.Count);
			buttonsToPress.Add(values[randomIndex]);
		}

		updateView ();
	}

	void Update()
	{
		// TODO: remove?
		if (!hasFocusFront)
			return;

		if (buttonsToPress.Count == 0)
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
				// correct buttons held and only 1 button held --> correct button
				if (lastHeld == buttonsToPress[0] && frontInputState.getButtonHeldCount () == 1) {
					// remove this button from list
					buttonsToPress.RemoveAt (0);

					// no buttons left -> challenge solved, else: update view
					if (buttonsToPress.Count == 0) {
						safe.SolveChallenge ();
					} else {
						updateView ();
					}
				} else {
					safe.FailChallenge ();
				}
			}
		}

	}

	void updateView(){
		//TODO: simplify and replace magenta with normal color
		// colorize placeholder cubes to show buttons to press
		// when actual models for the button challenge are in, this should be replaced
		if ((buttonsToPress[0] & GameButton.Left) != 0)
			backGameObject.transform.GetChild(0).GetComponent<Renderer>().material.color = Color.green;
		else
			backGameObject.transform.GetChild(0).GetComponent<Renderer>().material.color = Color.magenta;
		if ((buttonsToPress[0] & GameButton.Top) != 0)
			backGameObject.transform.GetChild(1).GetComponent<Renderer>().material.color = Color.green;
		else
			backGameObject.transform.GetChild(1).GetComponent<Renderer>().material.color = Color.magenta;
		if ((buttonsToPress[0] & GameButton.Bottom) != 0)
			backGameObject.transform.GetChild(2).GetComponent<Renderer>().material.color = Color.green;
		else
			backGameObject.transform.GetChild(2).GetComponent<Renderer>().material.color = Color.magenta;
		if ((buttonsToPress[0] & GameButton.Right) != 0)
			backGameObject.transform.GetChild(3).GetComponent<Renderer>().material.color = Color.green;
		else
			backGameObject.transform.GetChild(3).GetComponent<Renderer>().material.color = Color.magenta;
		
	}
}
