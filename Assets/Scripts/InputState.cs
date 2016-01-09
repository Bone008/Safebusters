using System;
using UnityEngine;

public class InputState
{
	public GameButton HeldButtons { get; set; }
    public GameButton PressedButtons { get; set; }
    public Vector3 Acceleration { get; set; }

	private float[] analogInputs = new float[5];

    public float GetAnalogInput(GameAnalogInput type)
    {
        return analogInputs[(int)type];
    }
    public void SetAnalogInput(GameAnalogInput type, float value)
    {
        analogInputs[(int)type] = value;
    }

	public int getButtonHeldCount(){
		int count = 0;
		int value = (int) HeldButtons;
		while (value != 0) {
			value = value & (value - 1); //internet magic
			count++;
		}
		return count;
	}

}

[Flags]
public enum GameButton
{
    None = 0,
    Top = 1,
    Bottom = 2,
    Left = 4,
    Right = 8
}

public enum GameAnalogInput
{
    LeftKnob = 0,
    RightKnob = 1,
    LeftSlider = 2,
    RightSlider = 3,
    Microphone = 4
}
