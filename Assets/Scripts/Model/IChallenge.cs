
namespace Model
{
    public interface IChallenge
    {
        bool HasFrontInput { get; }
        bool HasBackInput { get; }

        InputResult receiveFrontInput(InputCommand input);
        InputResult receiveBackInput(InputCommand input);
    }

    public enum InputResult
    {
        Solved, Error, None
    }

    // TODO
    public class InputCommand { }
}
