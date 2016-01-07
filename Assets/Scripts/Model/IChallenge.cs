
namespace Model
{
    public interface IChallenge
    {
        bool HasFrontInput { get; }
        bool HasBackInput { get; }

        InputResult receiveFrontInput(InputCommand command, InputState state);
        InputResult receiveBackInput(InputCommand command, InputState state);
    }

    public enum InputResult
    {
        Solved, Error, None
    }

    // TODO make this meaningful or remove it
    public class InputCommand { }

}
