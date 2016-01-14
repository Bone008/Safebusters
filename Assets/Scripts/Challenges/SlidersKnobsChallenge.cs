using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class SlidersKnobsChallenge : AbstractChallenge
{
    private const float EPSILON = 0.1f; // max difference between actual value and goal to solve challenge

    private SliderController leftSlider;
    private SliderController rightSlider;
    private KnobController leftKnob;
    private KnobController rightKnob;

    private float leftSliderGoal;
    private float rightSliderGoal;
    private float leftKnobGoal;
    private float rightKnobGoal;

    protected override void InitChallenge()
    {
        // GUI
        var sliders = frontGameObject.GetComponentsInChildren<SliderController>();
        leftSlider = sliders[0];
        rightSlider = sliders[1];

        var knobs = frontGameObject.GetComponentsInChildren<KnobController>();
        leftKnob = knobs[0];
        rightKnob = knobs[1];

        // Challenge
        leftSliderGoal = UnityEngine.Random.value;
        rightSliderGoal = UnityEngine.Random.value;
        leftKnobGoal = UnityEngine.Random.value;
        rightKnobGoal = UnityEngine.Random.value;

        // for testing: just show the values on the back
        // TODO make this more interesting
        Text backText = backGameObject.GetComponentInChildren<Text>();
        backText.text = 
            "LS: " + (leftSliderGoal * 100).ToString("0") + "%\n" +
            "RS: " + (rightSliderGoal * 100).ToString("0") + "%\n" +
            "LK: " + (leftKnobGoal * 100).ToString("0") + "%\n" + 
            "RK: " + (rightKnobGoal * 100).ToString("0") + "%\n";
    }

    void Update()
    {
        if (!hasFocusFront)
            return;

        float ls, rs, lk, rk;
        ls = frontInputState.GetAnalogInput(GameAnalogInput.LeftSlider);
        rs = frontInputState.GetAnalogInput(GameAnalogInput.RightSlider);
        lk = frontInputState.GetAnalogInput(GameAnalogInput.LeftKnob);
        rk = frontInputState.GetAnalogInput(GameAnalogInput.RightKnob);

        // Update GUI
        leftSlider.SetValue(ls);
        rightSlider.SetValue(rs);
        leftKnob.SetValue(lk);
        rightKnob.SetValue(rk);

        // Check if goal is reached (simple)
        if((Math.Abs(ls - leftSliderGoal) <= EPSILON)
            && (Math.Abs(rs - rightSliderGoal) <= EPSILON)
            && (Math.Abs(lk - leftKnobGoal) <= EPSILON)
            && (Math.Abs(rk - rightKnobGoal) <= EPSILON))
        {
            safe.SolveChallenge();
        }

    }

}
