using UnityEngine;
using System.Collections;

public class KnobController : MonoBehaviour
{

    public Transform thingThatTurns;
    public float minAngle;
    public float maxAngle;

    void Start()
    {
        SetValue(0);
    }

    /// <summary>Sets the value the knob is displaying (between 0 and 1).</summary>
    public void SetValue(float value)
    {
        thingThatTurns.localEulerAngles = new Vector3(0, 0, Mathf.Lerp(minAngle, maxAngle, value));
    }

}
