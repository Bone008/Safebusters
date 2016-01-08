using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Model.Challenges
{
    public class ButtonChallenge : AbstractChallenge
    {
        private GameButton buttonsToPress;
		private int numButtons = 2;

        public ButtonChallenge() : base(true, false)
        {
			// Pick [numButtons] random GameButtons as buttonsToPress
			List<GameButton> values = Enum.GetValues (typeof(GameButton)).Cast<GameButton>().ToList();
			buttonsToPress = GameButton.None;
			for (int i = 0; i < numButtons; i++) {
				int randomIndex = UnityEngine.Random.Range (1, values.Count);
				buttonsToPress |= values[randomIndex];
				values.RemoveAt (randomIndex);
			}
        }

        public override InputResult receiveFrontInput(InputCommand command, InputState state)
        {         
			//Debug.Log ("Buttonchallenge: " + buttonsToPress.ToString () + " <= " + state.PressedButtons.ToString ());
            
			// Right Combination -> Solved
			// Wrong Combination -> Error
			// No Input -> None
			if (state.PressedButtons == buttonsToPress) {
				return InputResult.Solved;
			} else if (state.PressedButtons == GameButton.None) {
				return InputResult.None;
			} else {
				return InputResult.Error;
			}
        }
    }
}