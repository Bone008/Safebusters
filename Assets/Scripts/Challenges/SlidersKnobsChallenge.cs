using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections;
using System;
using System.Collections.Generic;

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


        IconSliderController[] iconDisplaysOnBack = backGameObject.GetComponentsInChildren<IconSliderController>();
        List<Sprite> sprites = GameObject.FindWithTag("GameController").GetComponent<Level>().iconSprites.ToList();
        List<float> spriteRotations = GameObject.FindWithTag("GameController").GetComponent<Level>().iconSpriteRotations.ToList();

        if (iconDisplaysOnBack.Length > sprites.Count)
        {
            Debug.LogError("You need to configure at least enough icons (" + iconDisplaysOnBack.Length + ") to fill all columns on the back of sliders&knobs!");
            return;
        }

        foreach (var icon in iconDisplaysOnBack)
        {
            // set each column to a random icon and position
            int i = UnityEngine.Random.Range(0, sprites.Count);
            icon.SetIcon(sprites[i], spriteRotations[i]);
            icon.SetValue(UnityEngine.Random.value);

            sprites.RemoveAt(i);
            spriteRotations.RemoveAt(i);
        }

        // pick 4 random icons from the back and use their values for the challenge
        var selectedIcons = iconDisplaysOnBack.OrderBy(_ => UnityEngine.Random.value).Take(4).ToList();
        leftSliderGoal = selectedIcons[0].GetValue();
        rightSliderGoal = selectedIcons[1].GetValue();
        leftKnobGoal = selectedIcons[2].GetValue();
        rightKnobGoal = selectedIcons[3].GetValue();

        // update the icons on the front to display the chosen icons
        Transform frontCanvas = frontGameObject.GetComponentInChildren<Canvas>().transform;
        for(int i=0; i<4; i++)
        {
            frontCanvas.GetChild(i).localRotation = selectedIcons[i].handle.localRotation;
            frontCanvas.GetChild(i).GetChild(0).GetComponent<Image>().sprite = selectedIcons[i].handleIcon.sprite;
        }
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
        if ((Math.Abs(ls - leftSliderGoal) <= EPSILON)
            && (Math.Abs(rs - rightSliderGoal) <= EPSILON)
            && (Math.Abs(lk - leftKnobGoal) <= EPSILON)
            && (Math.Abs(rk - rightKnobGoal) <= EPSILON))
        {
            safe.SolveChallenge();
        }

    }

}
