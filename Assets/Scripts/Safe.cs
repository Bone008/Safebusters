using UnityEngine;
using System.Collections;

public class Safe : MonoBehaviour {

    [Header("Children mappings")]
    public Renderer frameRenderer;
    public Transform doorAnchor;
    public Transform frontAnchor;
    public Transform frontAnchorBottomRight;
    public Transform backAnchor;

    [HideInInspector]
    public AbstractChallenge challenge;
    public AnimationCurve doorOpeningSpeed;

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

    public void SpawnChallengeObjects(GameObject frontPrefab, GameObject backPrefab)
    {
        GameObject front = (GameObject)Instantiate(frontPrefab, Vector3.zero, Quaternion.identity);
        front.transform.SetParent(frontAnchor, false);
        GameObject back = (GameObject)Instantiate(backPrefab, Vector3.zero, Quaternion.identity);
        back.transform.SetParent(backAnchor, false);

        challenge.frontGameObject = front;
        challenge.backGameObject = back;
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

    IEnumerator RotateSafeDoorSmoothly() {
        print("Called RotateSafeDoorSmoothly, current rotation: "+ doorAnchor.localRotation.eulerAngles.y);
        float passedTime = 0;
        while (doorAnchor.localRotation.eulerAngles.y > 270 || doorAnchor.localRotation.eulerAngles.y < 90) //Needed the 90 because for some reason it goes negative for a frame... -.-
        {
            print("rotating, added value is: "+ doorOpeningSpeed.Evaluate(passedTime)+" current rotation is: "+doorAnchor.localRotation.eulerAngles.y);
            doorAnchor.localRotation = Quaternion.Euler(doorAnchor.localRotation.eulerAngles.x, doorAnchor.localRotation.eulerAngles.y - doorOpeningSpeed.Evaluate(passedTime), doorAnchor.localRotation.eulerAngles.z);
            passedTime += Time.deltaTime;
            yield return 0;
        }
    }

}
