using UnityEngine;
using System;
using System.Collections.Generic;

namespace Model.Challenges
{
    public class ButtonChallenge : AbstractChallenge
    {
        private GameButton buttonsToPress;

        public ButtonChallenge()
            : base(true, false)
        {
            buttonsToPress = GameButton.Left | GameButton.Bottom;
        }

        public override InputResult receiveFrontInput(InputCommand command, InputState state)
        {
            Debug.Log("received input");
            
            if (state.PressedButtons == buttonsToPress)
                return InputResult.Solved;

            return InputResult.None;
        }
    }
}