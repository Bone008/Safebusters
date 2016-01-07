using Model.Challenges;
using UnityEngine;

namespace Model
{
    public class Safe
    {
        public Color DisplayColor { get; set; }
        public bool IsBackwards { get; set; }
        public IChallenge Challenge { get; set; }
        public int NumberOfSafesToActivate { get; set; }

        public bool IsOpen { get; set; }
        public bool IsActive { get; set; }

		public Safe(IChallenge challenge, Color displayColor, int numberOfSafesToActivate)
        {
			DisplayColor = displayColor;
			IsBackwards = (Random.value > 0.5f);
			Challenge = challenge;
			NumberOfSafesToActivate = numberOfSafesToActivate;

            IsOpen = false;
            IsActive = true;
        }
    }
}
