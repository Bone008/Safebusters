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

        public Safe()
        {
            DisplayColor = Color.cyan;
            IsBackwards = false;

            if (Random.value > 0.3)
                Challenge = new ButtonChallenge();
            else
                Challenge = new PlaceholderChallenge();

            NumberOfSafesToActivate = 1;
            IsOpen = false;
            IsActive = true;
        }
    }
}
