using UnityEngine;
using System.Collections;

public class Safe : MonoBehaviour
{

    public AnimationCurve doorOpeningCurve;
    public float doorOpeningDuration;

    [Header("Children mappings")]
    public Renderer frameRenderer;
    public Transform doorAnchor;
    public Transform frontAnchor;
    public Transform frontAnchorBottomRight;
    public Transform backAnchor;
    public GameObject backDecorated;
    public GameObject backUndecorated;

    [HideInInspector]
    public AbstractChallenge challenge;

    private bool backwards = false;
    private float maxTimer;
    private int numberOfSafesToActivate;

    private bool active = true;
    private float remainingTime;

    public bool IsActive { get { return active; } }

    void Start()
    {
    }

    void Update()
    {
    }


    public void SetInputState(bool isPlayer2, InputState state)
    {
        if (isPlayer2 == backwards)
            challenge.SetFrontInput(state);
        else
            challenge.SetBackInput(state);
    }

    public void SolveChallenge()
    {
        Debug.Log("challenge solved!");
        // open door
        //doorAnchor.localRotation = Quaternion.Euler(0, -90, 0);
        StartCoroutine(RotateSafeDoorSmoothly());
        // TODO process game logic (activate next safes, etc)
        SetActive(false);
        challenge.enabled = false;
    }
    public void FailChallenge()
    {
        Debug.Log("lost a life!");
    }

    public void SpawnChallengeObjects(GameObject frontPrefab, GameObject backPrefab, bool decoratedBack)
    {
        GameObject front = (GameObject)Instantiate(frontPrefab, Vector3.zero, Quaternion.identity);
        front.transform.SetParent(frontAnchor, false);
        GameObject back = (GameObject)Instantiate(backPrefab, Vector3.zero, Quaternion.identity);
        back.transform.SetParent(backAnchor, false);

        challenge.frontGameObject = front;
        challenge.backGameObject = back;

        backDecorated.SetActive(decoratedBack);
        backUndecorated.SetActive(!decoratedBack);
    }

    public void SetBackwards(bool flag)
    {
        backwards = flag;
        // note that the model is facing backwards by default, so it actually has to be rotated when NOT backwards
        transform.localRotation = (backwards ? Quaternion.identity : Quaternion.Euler(0, 180, 0));
    }

    public void SetDisplayColor(Color color)
    {
        frameRenderer.material.color = color;
    }

    public void SetNumberOfSafesToActivate(int num)
    {
        numberOfSafesToActivate = num;
    }

    public void SetActive(bool flag)
    {
        active = flag;
    }

    public void SetMaxTimer(float time)
    {
        maxTimer = time;
        remainingTime = time;
        // TODO set visual timer value
    }

    public Bounds GetFrameBounds()
    {
        return frameRenderer.bounds;
    }

    /// <summary>Returns the width of the area on the safe usable by challenges on the front side. Note that this returns a negative value.</summary>
    public float GetUsableFrontWidth()
    {
        return frontAnchorBottomRight.localPosition.x - frontAnchor.localPosition.x;
    }

    /// <summary>Returns the height of the area on the safe usable by challenges on the front side. Note that this returns a negative value.</summary>
    public float GetUsableFrontHeight()
    {
        return frontAnchorBottomRight.localPosition.y - frontAnchor.localPosition.y;
    }

    IEnumerator RotateSafeDoorSmoothly()
    {
        float passedTime = 0;
        // animate for [doorOpeningDuration] seconds
        while (passedTime < doorOpeningDuration)
        {
            // apply curve to [passedTime]
            doorAnchor.localRotation = Quaternion.Euler(0, -90 * doorOpeningCurve.Evaluate(passedTime / doorOpeningDuration), 0);
            passedTime += Time.deltaTime;
            yield return null;
        }

        // final step --> fully open
        doorAnchor.localRotation = Quaternion.Euler(0, -90, 0);
    }

}
