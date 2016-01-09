using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FourButtonsController : MonoBehaviour
{
    public Image left;
    public Image right;
    public Image top;
    public Image bottom;

    public Color highlightColor = Color.green;

    public void SetHighlightedButtons(GameButton buttonFlags)
    {
        left.color = ((buttonFlags & GameButton.Left) != 0 ? highlightColor : Color.white);
        right.color = ((buttonFlags & GameButton.Right) != 0 ? highlightColor : Color.white);
        top.color = ((buttonFlags & GameButton.Top) != 0 ? highlightColor : Color.white);
        bottom.color = ((buttonFlags & GameButton.Bottom) != 0 ? highlightColor : Color.white);
    }
}
