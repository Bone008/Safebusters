using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;

public class SynchronousSlidersChallenge : AbstractChallenge
{
    private const float MAX_EPSILON = 0.25f;
    private const float START_EPSILON = 0.9f;
    private const float GOAL_EPSILON = 0.08f;

    private enum StartedStatus { WaitForFocus, Init, Discharge }

    private SliderController leftSliderFront;
    private SliderController rightSliderFront;
    private RectTransform statusArrowFront;
    private Text statusTextFront;

    private SliderController leftSliderBack;
    private SliderController rightSliderBack;
    private RectTransform statusArrowBack;
    private Text statusTextBack;

    private StartedStatus status = StartedStatus.WaitForFocus; // Wait until both players have focus and set their sliders to 1, then start judging

    protected override void InitChallenge()
    {
        SliderController[] sliders;
        // GUI Front
        sliders = frontGameObject.GetComponentsInChildren<SliderController>();
        leftSliderFront = sliders[0];
        rightSliderFront = sliders[1];
        statusArrowFront = frontGameObject.transform.Find("Canvas/Arrow").GetComponent<RectTransform>();
        statusTextFront = frontGameObject.transform.Find("Canvas/Status").GetComponent<Text>();

        // GUI Back
        sliders = backGameObject.GetComponentsInChildren<SliderController>();
        leftSliderBack = sliders[0];
        rightSliderBack = sliders[1];
        statusArrowBack = backGameObject.transform.Find("Canvas/Arrow").GetComponent<RectTransform>();
        statusTextBack = backGameObject.transform.Find("Canvas/Status").GetComponent<Text>();

        // set status display to proper values
        SetStatus(StartedStatus.WaitForFocus);
    }

    void Update()
    {
        // always update the slider positions when in focus (better UX)
        // Get Input
        float lsf, rsf, lsb, rsb;
        lsf = frontInputState.GetAnalogInput(GameAnalogInput.LeftSlider);
        rsf = frontInputState.GetAnalogInput(GameAnalogInput.RightSlider);
        lsb = backInputState.GetAnalogInput(GameAnalogInput.LeftSlider);
        rsb = backInputState.GetAnalogInput(GameAnalogInput.RightSlider);

        // Update GUI
        if (hasFocusFront)
        {
            leftSliderFront.SetValue(lsf);
            rightSliderFront.SetValue(rsf);
        }
        if (hasFocusBack)
        {
            leftSliderBack.SetValue(lsb);
            rightSliderBack.SetValue(rsb);
        }


        // === challenge logic ===

        if (status == StartedStatus.WaitForFocus)
        {
            // do both player have focus now?
            if (hasFocusFront && hasFocusBack)
                SetStatus(StartedStatus.Init);
            else
                return;
        }
        // did a player lose focus?
        else if (!hasFocusFront || !hasFocusBack)
        {
            SetStatus(StartedStatus.WaitForFocus);
            return;
        }


        // when initializing: check for start condition
        if (status == StartedStatus.Init)
        {
            // both sliders are up --> start discharge
            if (lsf >= START_EPSILON && rsf >= START_EPSILON && lsb >= START_EPSILON && rsb >= START_EPSILON)
                SetStatus(StartedStatus.Discharge);
        }


        // Check if solved
        else if (lsf <= GOAL_EPSILON && rsf <= GOAL_EPSILON && lsb <= GOAL_EPSILON && rsb <= GOAL_EPSILON)
        {
            safe.SolveChallenge();
        }
        // Check if failed (simple) (distance between corresponding sliders >= max epsilon)
        else if ((Math.Abs(lsf - lsb) >= MAX_EPSILON) || (Math.Abs(rsf - rsb) >= MAX_EPSILON))
        {
            SetStatus(StartedStatus.Init);
            safe.FailChallenge();
        }
        else
        {
            float failIntensity = Mathf.Max(Mathf.Abs(lsf - lsb), Mathf.Abs(rsf - rsb)) / MAX_EPSILON;

            // set LEDs to indicate how close you are to failing
            // between 1 and 4 LEDs should be on while discharging
            for (int i = 0; i < 4; i++)
            {
                frontInputState.Output.SetLEDState(i, (failIntensity > 0.27f * i));
                backInputState.Output.SetLEDState(i, (failIntensity > 0.27f * i));
            }

            // same with engine
            frontInputState.Output.SetEngineIntensity(failIntensity);
            backInputState.Output.SetEngineIntensity(failIntensity);
        }
    }

    private void SetStatus(StartedStatus status)
    {
        this.status = status;

        switch (status)
        {
            case StartedStatus.WaitForFocus:
                statusArrowFront.gameObject.SetActive(false);
                statusArrowBack.gameObject.SetActive(false);
                statusTextFront.text = statusTextBack.text = "WAIT";
                break;

            case StartedStatus.Init:
                statusArrowFront.gameObject.SetActive(true);
                statusArrowBack.gameObject.SetActive(true);
                statusArrowFront.localRotation = statusArrowBack.localRotation = Quaternion.Euler(0, 0, 90);
                statusTextFront.text = statusTextBack.text = "INIT";
                break;

            case StartedStatus.Discharge:
                statusArrowFront.gameObject.SetActive(true);
                statusArrowBack.gameObject.SetActive(true);
                statusArrowFront.localRotation = statusArrowBack.localRotation = Quaternion.Euler(0, 0, 270);
                statusTextFront.text = statusTextBack.text = "DISCHARGE";
                break;
        }
    }

}
