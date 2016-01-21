using UnityEngine;
using System.Collections;

public class SliderController : MonoBehaviour
{

    public Renderer barRenderer;
    public Transform handle;

    void Awake()
    {
        SetValue(0);
    }

    /// <summary>Sets the value the slider is displaying (between 0 and 1).</summary>
    public void SetValue(float value)
    {
        float extents = barRenderer.bounds.extents.y;

        Vector3 pos = handle.localPosition;
        pos.y = Mathf.Lerp(-extents, extents, value);
        handle.localPosition = pos;
    }

}
