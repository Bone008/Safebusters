using UnityEngine;
using System.Collections;

public class SliderController : MonoBehaviour
{
    private const float BAR_EXTENT = 0.145f;

    public Transform handle;

    void Awake()
    {
        SetValue(0);
    }

    /// <summary>Sets the value the slider is displaying (between 0 and 1).</summary>
    public void SetValue(float value)
    {
        Vector3 pos = handle.localPosition;
        pos.y = Mathf.Lerp(-BAR_EXTENT, BAR_EXTENT, value);
        handle.localPosition = pos;
    }

}
