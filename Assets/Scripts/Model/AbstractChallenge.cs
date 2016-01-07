
namespace Model
{
    public abstract class AbstractChallenge : IChallenge
    {
        private bool hasFront;
        private bool hasBack;

        public bool HasFrontInput { get { return hasFront; } }
        public bool HasBackInput { get { return hasBack; } }

        protected AbstractChallenge(bool hasFront, bool hasBack)
        {
            this.hasFront = hasFront;
            this.hasBack = hasBack;
        }

        // no nothing by default
        // override this in subclasses to handle the challenge logic
        public virtual InputResult receiveFrontInput(InputCommand command, InputState state)
        {
            return InputResult.None;
        }

        public virtual InputResult receiveBackInput(InputCommand command, InputState state)
        {
            return InputResult.None;
        }
    }

}
