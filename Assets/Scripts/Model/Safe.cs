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
            DisplayColor = (Random.value > 0.5 ? new Color(0.5f, 0.7f, 0.6f) : new Color(0.7f, 0.7f, 0.5f));
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
