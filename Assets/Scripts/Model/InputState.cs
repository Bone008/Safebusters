using System;
using UnityEngine;

namespace Model
{
    public class InputState
    {

        private float[] analogInputs = new float[5];

        public GameButton PressedButtons { get; set; }
        public Vector3 Acceleration { get; set; }

        public float GetAnalogInput(GameAnalogInput type)
        {
            return analogInputs[(int)type];
        }
        public void SetAnalogInput(GameAnalogInput type, float value)
        {
            analogInputs[(int)type] = value;
        }

    }

    [Flags]
    public enum GameButton
    {
        None = 0,
        Bottom = 1,
        Top = 2,
        Right = 4,
        Left = 8
    }

    public enum GameAnalogInput
    {
        LeftKnob = 0,
        RightKnob,
        LeftSlider,
        RightSlider,
        Microphone
    }
}
