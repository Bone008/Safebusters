using UnityEngine;
using System.Collections;

public class Safe : MonoBehaviour {

    [Header("Children mappings")]
    public Renderer frameRenderer;
    public Transform doorAnchor;
    public Transform frontAnchor;
    public Transform backAnchor;

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
            challenge.SetBackInput(state);
        else
            challenge.SetFrontInput(state);
    }

    public void SolveChallenge()
    {
        Debug.Log("challenge solved!");
        // open door
        doorAnchor.localRotation = Quaternion.Euler(0, -90, 0);

        // TODO process game logic (activate next safes, etc)
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
        transform.localRotation = (backwards ? Quaternion.Euler(0, 180, 0) : Quaternion.identity);
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

}
