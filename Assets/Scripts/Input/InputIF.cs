
public interface InputIF
{
    /// <summary>Gets the state of the input relevant for ingame logic.</summary>
    InputState GameInputState { get; }

    /// <summary>Returns true when the player started pressing down the far left button in this frame.</summary>
    bool FarLeftPressed { get; }

    /// <summary>Returns true when the player started pressing down the far right button in this frame.</summary>
    bool FarRightPressed { get; }

}
