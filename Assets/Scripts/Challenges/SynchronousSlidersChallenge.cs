using UnityEngine;
using System;
using System.Collections;

public class SynchronousSlidersChallenge : AbstractChallenge
{

    private const float MAX_EPSILON = 0.1f;

    private SliderController leftSliderFront;
    private SliderController rightSliderFront;

    private SliderController leftSliderBack;
    private SliderController rightSliderBack;

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
            return;

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
               
        // Check if failed (simple)
        if((Math.Abs(lsf - lsb) >= MAX_EPSILON) || (Math.Abs(rsf - rsb) >= MAX_EPSILON))
        {
            safe.FailChallenge();
        }
    }
}
