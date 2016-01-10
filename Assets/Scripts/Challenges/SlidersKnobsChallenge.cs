using UnityEngine;
using System.Collections;

public class SlidersKnobsChallenge : AbstractChallenge
{

    private SliderController leftSlider;
    private SliderController rightSlider;
    private KnobController leftKnob;
    private KnobController rightKnob;

    protected override void InitChallenge()
    {
        var sliders = frontGameObject.GetComponentsInChildren<SliderController>();
        leftSlider = sliders[0];
        rightSlider = sliders[1];

        var knobs = frontGameObject.GetComponentsInChildren<KnobController>();
        leftKnob = knobs[0];
        rightKnob = knobs[1];
    }

    void Update()
    {
        if (!hasFocusFront)
            return;

        leftSlider.SetValue(frontInputState.GetAnalogInput(GameAnalogInput.LeftSlider));
        rightSlider.SetValue(frontInputState.GetAnalogInput(GameAnalogInput.RightSlider));
        leftKnob.SetValue(frontInputState.GetAnalogInput(GameAnalogInput.LeftKnob));
        rightKnob.SetValue(frontInputState.GetAnalogInput(GameAnalogInput.RightKnob));
    }

}
