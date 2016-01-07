using UnityEngine;
using System;
using System.Collections.Generic;

namespace Model.Challenges
{
    public class ButtonChallenge : IChallenge
    {
        public bool HasFrontInput { get { return true; } }

        public bool HasBackInput { get { return false; } }

        public InputResult receiveFrontInput(InputCommand input)
        {
            // TODO
            Debug.Log("received input");
            return InputResult.None;
        }

        public InputResult receiveBackInput(InputCommand input)
        {
            throw new NotImplementedException();
        }
    }
}