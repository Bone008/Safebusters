
public interface OutputIF
{

    /// <summary>Sets the intensity of the engine.</summary>
    /// <param name="intensity">between 0 and 1</param>
    void SetEngineIntensity(float intensity);

    /// <summary>Turns an LED on or off. 0=left-most led, 3=right-most led.</summary>
    void SetLEDState(int led, bool state);

}
