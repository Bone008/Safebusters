using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class ButtonSequenceChallenge : AbstractChallenge
{
    private const int LONGEST_SEQUENCE_TO_PRESS = 5;

    private List<GameButton> buttonsToPress;

    private int nextButtonIndex = 0;
    private List<LampController> lamps;

    protected override void InitChallenge()
    {
        // GameButtons as list
        List<GameButton> values = Enum.GetValues(typeof(GameButton)).Cast<GameButton>().ToList();

        // random amount of buttons (minimum 2) to press to solve the challenge
        int numButtonsToPress = UnityEngine.Random.Range(2, LONGEST_SEQUENCE_TO_PRESS + 1);

        // pick [numButtonsToPress] random GameButtons as buttonsToPress
        buttonsToPress = new List<GameButton>();
        for (int i = 0; i < numButtonsToPress; i++)
        {
            int randomIndex = UnityEngine.Random.Range(1, values.Count);
            buttonsToPress.Add(values[randomIndex]);
        }

        lamps = new List<LampController>(LONGEST_SEQUENCE_TO_PRESS);
        frontGameObject.GetComponentsInChildren<LampController>(lamps);

        for (int i = buttonsToPress.Count; i < lamps.Count; i++)
            lamps[i].gameObject.SetActive(false);

        UpdateBackView();
    }

    void Update()
    {
        if (!hasFocusFront)
            return;

        GameButton pressed = frontInputState.PressedButtons;
        if (pressed == buttonsToPress[nextButtonIndex])
        {
            if (IsInvoking("ResetLamps"))
            {
                CancelInvoke("ResetLamps");
                ResetLamps();
            }

            lamps[nextButtonIndex].SetLightColor(Color.green);
            lamps[nextButtonIndex].SetTurnedOn(true);
            nextButtonIndex++;

            // no buttons left -> challenge solved, else: update view
            if (nextButtonIndex == buttonsToPress.Count)
                safe.SolveChallenge();
            else
                UpdateBackView();
        }
        else if (pressed != GameButton.None)
        {
            safe.FailChallenge();

            // reset to beginning
            for (int i = 0; i < nextButtonIndex; i++)
                lamps[i].SetLightColor(Color.red);
            Invoke("ResetLamps", 0.5f);

            nextButtonIndex = 0;
            UpdateBackView();
        }
    }

    private void ResetLamps()
    {
        for (int i = 0; i < lamps.Count; i++)
            lamps[i].SetTurnedOn(false);
    }

    private void UpdateBackView()
    {
        //TODO: simplify and replace magenta with normal color
        // colorize placeholder cubes to show buttons to press
        // when actual models for the button challenge are in, this should be replaced
        if ((buttonsToPress[nextButtonIndex] & GameButton.Left) != 0)
            backGameObject.transform.GetChild(0).GetComponent<UnityEngine.UI.Image>().color = Color.green;
        else
            backGameObject.transform.GetChild(0).GetComponent<UnityEngine.UI.Image>().color = Color.magenta;

        if ((buttonsToPress[nextButtonIndex] & GameButton.Top) != 0)
            backGameObject.transform.GetChild(1).GetComponent<UnityEngine.UI.Image>().color = Color.green;
        else
            backGameObject.transform.GetChild(1).GetComponent<UnityEngine.UI.Image>().color = Color.magenta;

        if ((buttonsToPress[nextButtonIndex] & GameButton.Bottom) != 0)
            backGameObject.transform.GetChild(2).GetComponent<UnityEngine.UI.Image>().color = Color.green;
        else
            backGameObject.transform.GetChild(2).GetComponent<UnityEngine.UI.Image>().color = Color.magenta;

        if ((buttonsToPress[nextButtonIndex] & GameButton.Right) != 0)
            backGameObject.transform.GetChild(3).GetComponent<UnityEngine.UI.Image>().color = Color.green;
        else
            backGameObject.transform.GetChild(3).GetComponent<UnityEngine.UI.Image>().color = Color.magenta;

    }
}
