using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;

public class SynchronousSlidersChallenge : AbstractChallenge
{
    private const float MAX_EPSILON = 0.1f;
    private const float START_EPSILON = 0.9f;

    private SliderController leftSliderFront;
    private SliderController rightSliderFront;
    private RectTransform statusArrowFront;
    private Text statusTextFront;

    private SliderController leftSliderBack;
    private SliderController rightSliderBack;
    private RectTransform statusArrowBack;
    private Text statusTextBack;

    private bool hasStarted = false; // Wait, until both players set their sliders to 1, then start judging

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
    }

    void Update()
    {
        // Sync-Challenge -> Both focus required
        if (!hasFocusFront || !hasFocusBack)
        {
            SetStarted(false);
            return;
        }

        // Get Input
        float lsf, rsf, lsb, rsb;
        lsf = frontInputState.GetAnalogInput(GameAnalogInput.LeftSlider);
        rsf = frontInputState.GetAnalogInput(GameAnalogInput.RightSlider);
        lsb = backInputState.GetAnalogInput(GameAnalogInput.LeftSlider);
        rsb = backInputState.GetAnalogInput(GameAnalogInput.RightSlider);

        // Update GUI
        leftSliderFront.SetValue(lsf);
        rightSliderFront.SetValue(rsf);
        leftSliderBack.SetValue(lsb);
        rightSliderBack.SetValue(rsb);

        // Check if started
        if(lsf >= START_EPSILON && rsf >= START_EPSILON && lsb >= START_EPSILON && rsb >= START_EPSILON)
        {
            SetStarted(true);
        }

        // Abort here if not started
        if (!hasStarted)
            return;

        // Check if solved
        if(lsf <= MAX_EPSILON && rsf <= MAX_EPSILON && lsb <= MAX_EPSILON && rsb <= MAX_EPSILON)
        {
            safe.SolveChallenge();
        }
        // Check if failed (simple) (distance between corresponding sliders >= max epsilon)
        else if((Math.Abs(lsf - lsb) >= MAX_EPSILON) || (Math.Abs(rsf - rsb) >= MAX_EPSILON))
        {
            SetStarted(false);
            safe.FailChallenge();
        }
    }

    public void SetStarted(bool flag)
    {
        hasStarted = flag;
        if(flag)
        {
            statusArrowFront.localRotation = statusArrowBack.localRotation = Quaternion.Euler(0, 0, 270);
            statusTextFront.text = statusTextBack.text = "DISCHARGE";
        }
        else
        {
            statusArrowFront.localRotation = statusArrowBack.localRotation = Quaternion.Euler(0, 0, 90);
            statusTextFront.text = statusTextBack.text = "INIT";
        }
    }

}
