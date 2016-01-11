using UnityEngine;
using System;
using System.Collections;

public class SynchronousSlidersChallenge : AbstractChallenge
{
    private const float MAX_EPSILON = 0.1f;
    private const float START_EPSILON = 0.9f;

    private SliderController leftSliderFront;
    private SliderController rightSliderFront;

    private SliderController leftSliderBack;
    private SliderController rightSliderBack;

    private bool hasStarted = false; // Wait, until both players set their sliders to 1, then start judging

    protected override void InitChallenge()
    {
        SliderController[] sliders;
        // GUI Front
        sliders = frontGameObject.GetComponentsInChildren<SliderController>();
        leftSliderFront = sliders[0];
        rightSliderFront = sliders[1];

        // GUI Back
        sliders = backGameObject.GetComponentsInChildren<SliderController>();
        leftSliderBack = sliders[0];
        rightSliderBack = sliders[1];
    }

    void Update()
    {
        // Sync-Challenge -> Both focus required
        if (!hasFocusFront || !hasFocusBack)
        {
            hasStarted = false;
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
            hasStarted = true;
            // TODO: do stuff to show the players that the challenge has started
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
        if((Math.Abs(lsf - lsb) >= MAX_EPSILON) || (Math.Abs(rsf - rsb) >= MAX_EPSILON))
        {
            safe.FailChallenge();
        }
    }
}
