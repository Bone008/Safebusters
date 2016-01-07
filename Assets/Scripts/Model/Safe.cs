
using Model.Challenges;
namespace Model
{
    public class Safe
    {
        public bool IsOpen { get; set; }
        public bool IsActive { get; set; }
        //private ColorGroup colorGroup = ColorGroup.Default;
        public bool IsBackwards { get; set; }
        public IChallenge Challenge { get; set; }
        public int NumberOfSafesToActivate { get; set; }

        public Safe()
        {
            IsOpen = false;
            IsActive = true;
            IsBackwards = false;
            Challenge = new ButtonChallenge();
            NumberOfSafesToActivate = 1;
        }
    }
}
