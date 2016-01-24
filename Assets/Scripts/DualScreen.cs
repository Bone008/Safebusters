using UnityEngine;
using System.Collections;

public class DualScreen : MonoBehaviour
{

    public Camera cam1;
    public Camera cam2;

    void Start()
    {
        if (Display.displays.Length >= 2)
        {
            Debug.Log("Enabling dual screen mode.");

            Display.displays[1].Activate();
            cam1.rect = new Rect(0, 0, 1, 1);
            cam1.targetDisplay = 0;
            cam2.rect = new Rect(0, 0, 1, 1);
            cam2.targetDisplay = 1;
        }
    }

}
