using System;
using System.Threading;
using System.Linq;
using System.IO.Ports;
using System.IO;
using UnityEngine;
using System.Globalization;
using System.Collections.Generic;

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
    private const int SMOOTHING_SAMPLES = 5;

    private readonly float refreshInterval;
    private SerialPort port;

    private Thread thread = null;
    private volatile bool running = false;

    private LinkedList<CactusButton> __buttonPresses = new LinkedList<CactusButton>(); // has to be accessed with locks
    private InSnapshot __sharedInputSnapshot; // has to be accessed with locks
    private OutSnapshot __sharedOutputSnapshot; // has to be accessed with locks

    // local attributes (not accessed by other threads)
    private CactusButton lastHeldButtons = 0;
    private float lastSentIntensity = 0;
    private int lastSentLedFlags = 0;
    private Queue<float>[] analogValueHistory;

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

        // push default snapshots at the beginning so they are never null
        InputSnapshot = new InSnapshot { analogInputs = new float[4] };
        OutputSnapshot = new OutSnapshot();

        analogValueHistory = new Queue<float>[4];
        for (int i = 0; i < 4; i++)
        {
            analogValueHistory[i] = new Queue<float>(SMOOTHING_SAMPLES);
            for (int c = 0; c < SMOOTHING_SAMPLES; c++) analogValueHistory[i].Enqueue(0);
        }
    }

    public CactusButton? PollButtonPress()
    {
        lock (__buttonPresses)
        {
            if (__buttonPresses.Count == 0)
                return null;

            CactusButton btn = __buttonPresses.First.Value;
            __buttonPresses.RemoveFirst();
            return btn;
        }
    }

    private void PushButtonPress(CactusButton button)
    {
        lock (__buttonPresses)
        {
            __buttonPresses.AddLast(button);
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
        // held
        input.heldButtons = (CactusButton)int.Parse(response, NumberStyles.HexNumber);

        // pressed ^= now held but not during the previous sample
        CactusButton newPresses = input.heldButtons & ~lastHeldButtons;
        foreach (CactusButton btn in Enum.GetValues(typeof(CactusButton)))
            if ((newPresses & btn) != 0) PushButtonPress(btn);

        lastHeldButtons = input.heldButtons;


        // read analog
        port.Write("4");
        response = port.ReadLine();
        input.analogInputs = response.Split(' ')
                                     .Skip(1)
                                     .Select(str => int.Parse(str, NumberStyles.HexNumber))
                                     .Select(num => 1.0f - num / 4096.0f)
                                     .ToArray();
        SmoothAnalog(input.analogInputs);

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


    private void SmoothAnalog(float[] values)
    {
        for(int i=0; i<values.Length; i++)
        {
            // push new sample on history and remove oldest sample
            Queue<float> history = analogValueHistory[i];
            history.Dequeue();
            history.Enqueue(values[i]);

            // smoothed value is average of last samples
            values[i] = history.Average();
        }
    }


    private void WriteOutput(OutSnapshot output)
    {
        // engine
        // only update when a notable difference from previous value
        if (Mathf.Abs(output.engineIntensity - lastSentIntensity) > 0.01f || output.engineIntensity == 0)
        {
            int value = Mathf.RoundToInt(Mathf.Clamp01(output.engineIntensity) * 1000);
            port.WriteLine("m " + value);
            port.ReadLine();

            lastSentIntensity = output.engineIntensity;
        }

        // LEDs
        output.ledFlags &= 0xf; // only use lowest 4 bits
        if (lastSentLedFlags != output.ledFlags)
        {
            for (int i = 0; i < 4; i++)
            {
                // swapped order because we hold the controller upside down
                int mask = 1 << (3 - i);

                // efficiency: don't send anything when already in correct state
                bool wasOn = (lastSentLedFlags & mask) != 0;
                bool isOn = (output.ledFlags & mask) != 0;
                if (wasOn != isOn)
                {
                    port.WriteLine("l " + i + " " + (isOn ? "1" : "0"));
                    port.ReadLine();
                }
            }

            lastSentLedFlags = output.ledFlags;
        }
    }

}
