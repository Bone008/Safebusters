using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class IconSliderController : MonoBehaviour
{

    private const float HANDLE_TOP_Y = 69.0f;
    private const float HANDLE_BOTTOM_Y = -69.0f;

    public RectTransform handle;
    public Image handleIcon;

    private float value = 0;

    void Awake()
    {
        SetValue(0);
    }

    /// <summary>Sets the value the slider is displaying (between 0 and 1).</summary>
    public void SetValue(float value)
    {
        this.value = value;
        handle.anchoredPosition = new Vector2(0, HANDLE_BOTTOM_Y + (HANDLE_TOP_Y - HANDLE_BOTTOM_Y) * value);
    }

    public float GetValue()
    {
        return value;
    }

    public void SetIcon(Sprite icon, float rotation)
    {
        handleIcon.sprite = icon;
        handle.localRotation = Quaternion.Euler(0, 0, rotation);
    }

}
