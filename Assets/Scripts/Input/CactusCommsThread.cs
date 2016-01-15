using System;
using System.Threading;
using System.Linq;
using System.IO.Ports;
using System.IO;
using UnityEngine;
using System.Globalization;

public class CactusCommsThread
{
    [Flags]
    public enum CactusButton
    {
        FarLeft = (1 << 10),
        Left = (1 << 8),
        Up = (1 << 6),
        Down = (1 << 7),
        Right = (1 << 9),
        FarRight = (1 << 11)
    }


    public struct InSnapshot
    {
        public CactusButton heldButtons;
        public float[] analogInputs;
        public float microphone;
        public Vector3 acceleration;
    }
    public struct OutSnapshot
    {
        public float engineIntensity;
        public int ledFlags;
    }


    private const float ACCEL_GRAVITATION_NORM = 0.5f;

    private readonly float refreshInterval;
    private SerialPort port;

    private Thread thread = null;
    private volatile bool running = false;

    private InSnapshot __sharedInputSnapshot; // has to be accessed with locks
    private OutSnapshot __sharedOutputSnapshot; // has to be accessed with locks

    // local attributes (not accessed by other threads)
    private float lastSentIntensity = 0;
    private int lastSentLedFlags = 0;

    public bool IsConnected { get { return port != null; } }

    public InSnapshot InputSnapshot
    {
        get { lock (this) { return __sharedInputSnapshot; } }
        private set { lock (this) { __sharedInputSnapshot = value; } }
    }
    public OutSnapshot OutputSnapshot
    {
        private get { lock (this) { return __sharedOutputSnapshot; } }
        set { lock (this) { __sharedOutputSnapshot = value; } }
    }

    public CactusCommsThread(string portName, float refreshInterval)
    {
        this.refreshInterval = refreshInterval;
        try
        {
            port = new SerialPort(portName, 115200);
            port.Open();
        }
        catch (IOException e)
        {
            Debug.Log("Controller at " + portName + " not available: " + e.Message);
            port = null; // not available
        }
    }

    public void Start()
    {
        running = true;
        thread = new Thread(Run);
        thread.Start();
    }

    /// <summary>Asks the thread to stop and returns immediately.</summary>
    public void Stop()
    {
        running = false;
    }

    /// <summary>Asks the thread to stop and blocks until it terminates.</summary>
    public void StopNow(TimeSpan timeout)
    {
        Stop();
        if (thread != null)
        {
            thread.Join(timeout);
            if (thread.IsAlive) // if the thread didn't exit gracefully within the given time, kill it with fire
            {
                thread.Abort();
                Debug.LogWarning("Comm thread didn't shut down in time, had to be aborted.");
            }
        }
    }

    private void Run()
    {
        if (!IsConnected)
            return;

        while (running)
        {
            // process all inputs and make a snapshot available to other threads
            InputSnapshot = ReadInput();
            // read the desired output and write changes
            WriteOutput(OutputSnapshot);

            // rest for a bit
            Thread.Sleep(TimeSpan.FromSeconds(refreshInterval));
        }

        // reset outputs
        WriteOutput(new OutSnapshot());

        port.Close();
    }


    private InSnapshot ReadInput()
    {
        InSnapshot input = new InSnapshot();

        // read buttons
        port.Write("1");
        string response = port.ReadLine();
        input.heldButtons = (CactusButton)int.Parse(response, NumberStyles.HexNumber);

        // read analog
        port.Write("4");
        response = port.ReadLine();
        input.analogInputs = response.Split(' ')
                                     .Skip(1)
                                     .Select(str => int.Parse(str, NumberStyles.HexNumber))
                                     .Select(num => 1.0f - num / 4096.0f)
                                     .ToArray();

        // read microphone
        port.Write("s");
        response = port.ReadLine(); // format: "RMS: 0.000000"
        input.microphone = float.Parse(response.Substring(5), CultureInfo.InvariantCulture) / 32768f;

        // read accelerometer
        port.Write("a");
        response = port.ReadLine();
        float[] accelCoords = response.Split(' ').Skip(1).Select(s => Convert.ToSByte(s, 16) / 128.0f).ToArray();
        // swizzle to (-y, z, -x)
        // * z is up direction (map to y)
        // * y is right direction (map to -x)
        // * x is front direction (map to -z)
        input.acceleration = new Vector3(-accelCoords[1], accelCoords[2], -accelCoords[0]) / ACCEL_GRAVITATION_NORM;

        return input;
    }


    private void WriteOutput(OutSnapshot output)
    {
        // engine
        // only update when a notable difference from previous value
        if(Mathf.Abs(output.engineIntensity - lastSentIntensity) > 0.01f || output.engineIntensity == 0)
        {
            int value = Mathf.RoundToInt(Mathf.Clamp01(output.engineIntensity) * 1000);
            port.WriteLine("m " + value);
            port.ReadLine();

            lastSentIntensity = output.engineIntensity;
        }

        // LEDs
        output.ledFlags &= 0xf; // only use lowest 4 bits
        if(lastSentLedFlags != output.ledFlags)
        {
            for (int i = 0; i < 4; i++)
            {
                // swapped order because we hold the controller upside down
                int mask = 1 << (3 - i);

                // efficiency: don't send anything when already in correct state
                bool wasOn = (lastSentLedFlags & mask) != 0;
                bool isOn = (output.ledFlags & mask) != 0;
                if(wasOn != isOn)
                {
                    port.WriteLine("l " + i + " " + (wasOn ? "1" : "0"));
                    port.ReadLine();
                }
            }

            lastSentLedFlags = output.ledFlags;
        }
    }

}
