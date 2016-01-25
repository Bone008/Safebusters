using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class Safe : MonoBehaviour
{

    public AnimationCurve doorOpeningCurve;
    public float doorOpeningDuration;
    public AnimationCurve shutterOpenCurve;
    public float shutterOpenDuration;

    [Header("Children mappings")]
    public Renderer frameRenderer;
    public Transform doorAnchor;
    public Transform frontAnchor;
    public Transform frontAnchorBottomRight;
    public Transform backAnchor;
    public GameObject backDecorated;
    public GameObject backUndecorated;
    public Transform shutter;
    public GameObject protectivePanel;
    [Header("Timer controls")]
    public Text timerDigit1;
    public Text timerDigit2;
    public Text timerDot;

    public AudioClip wonSound;
    public AudioClip lostSound;

    [HideInInspector]
    public AbstractChallenge challenge;

    private bool backwards = false;
    private float maxTimer;
    private int numberOfSafesToActivate;

    private bool shutterClosed = false;

    private Color displayColor;
    private bool active = true;
    private float remainingTime;
    private bool open = false;
    Level lvl;

    public bool IsActive { get { return active; } }
    public bool IsOpen { get { return open; } }

    void Start()
    {
        lvl = GameObject.FindGameObjectWithTag("GameController").GetComponent<Level>(); 
        // initially show the shutter when we are started in inactive state
        if (!active)
        {
            print(name + " is initially inactive");
            shutter.gameObject.SetActive(true);
            protectivePanel.SetActive(true);
            shutterClosed = true;
        }
    }

    void Update()
    {
        if (active)
        {
            // count down timer
            remainingTime -= Time.deltaTime;

            if (remainingTime < 0)
                // TODO lose game; for now just reset to max value (easier testing when you're not constantly losing ...)
                //remainingTime = maxTimer;
                FailChallenge();
            UpdateTimerText();

            //Only for debugging!
#if UNITY_EDITOR
            if (Input.GetKeyUp(KeyCode.L)) FailChallenge();
            if (Input.GetKeyUp(KeyCode.W)) SolveChallenge();
#endif
        }
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
        GetComponent<AudioSource>().PlayOneShot(wonSound);
        // open door
        //doorAnchor.localRotation = Quaternion.Euler(0, -90, 0);
        StartCoroutine(AnimateRotateSafeDoor());
        
        open = true;
        //SetActive(false); <-- this is now done at the end of the coroutine
        challenge.enabled = false;

        lvl.safesOpened++;
        //Activating new safes of same coloring
        if (lvl.safesOpened < lvl.safes.Count) { 
            lvl.ActivateNewSafes(displayColor, numberOfSafesToActivate);
        }
        else {
            lvl.showEndGUI(true);
        }
    }
    public void FailChallenge()
    {
        GetComponent<AudioSource>().PlayOneShot(lostSound);
        if (lvl.currentLiveCount > 0)
        { //We still have tries left
            StartCoroutine(ActivateNeuroToxin());
            lvl.fails++;
            lvl.currentLiveCount--;
        }
        else {
            lvl.showEndGUI(false);
        }
        remainingTime = maxTimer;
    }

    public void SpawnChallengeObjects(GameObject frontPrefab, GameObject backPrefab, bool decoratedBack)
    {
        if (frontPrefab != null)
        {
            GameObject front = (GameObject)Instantiate(frontPrefab, Vector3.zero, Quaternion.identity);
            front.transform.SetParent(frontAnchor, false);
            challenge.frontGameObject = front;
        }
        if (backPrefab != null)
        {
            GameObject back = (GameObject)Instantiate(backPrefab, Vector3.zero, Quaternion.identity);
            back.transform.SetParent(backAnchor, false);
            challenge.backGameObject = back;
        }

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
        displayColor = color;
        frameRenderer.material.color = color;
    }

    public void SetNumberOfSafesToActivate(int num)
    {
        numberOfSafesToActivate = num;
    }

    public void SetActive(bool flag)
    {
        active = flag;
        UpdateTimerText();

        // when activating and the shutter is still closed
        if (flag && shutterClosed)
        {
            shutterClosed = false;
            StartCoroutine(AnimateOpenShutter());
        }
    }

    public void SetFocus(bool isPlayer2, bool flag)
    {
        if (isPlayer2 == backwards)
            challenge.SetFrontFocus(flag);
        else
            challenge.SetBackFocus(flag);
    }

    public void SetMaxTimer(float time)
    {
        maxTimer = time;
        remainingTime = time;
        UpdateTimerText();
    }

    private void UpdateTimerText()
    {
        if (active)
        {
            if (remainingTime >= 10)
            {
                // show whole seconds as a two digit number
                timerDigit1.text = "" + Mathf.FloorToInt(remainingTime / 10);
                timerDigit2.text = "" + Mathf.FloorToInt(remainingTime % 10);
                timerDot.enabled = false;
            }
            else
            {
                // show seconds and tenths of a second
                timerDigit1.text = "" + Mathf.FloorToInt(remainingTime);
                timerDigit2.text = "" + Mathf.FloorToInt(remainingTime * 10 % 10);
                timerDot.enabled = true;
                timerDot.text = ".";
            }
        }
        else
        {
            timerDigit1.text = "";
            timerDigit2.text = "";
            timerDot.enabled = true;
            timerDot.text = "OFF";
        }
    }

    public Bounds GetFrameBounds()
    {
        return frameRenderer.bounds;
    }

    public Color GetDisplayColor()
    {
        return displayColor;
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

    private IEnumerator AnimateRotateSafeDoor()
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

        // set to inactive
        SetActive(false);
    }

    private IEnumerator ActivateNeuroToxin() {
        float passedTime = 0;
        bool finishedActivating = false;
        UnityStandardAssets.ImageEffects.Fisheye fish1 = lvl.Player1Cam.GetComponent<UnityStandardAssets.ImageEffects.Fisheye>();
        UnityStandardAssets.ImageEffects.Fisheye fish2 = lvl.Player2Cam.GetComponent<UnityStandardAssets.ImageEffects.Fisheye>();
        UnityStandardAssets.ImageEffects.Blur blur1 = lvl.Player1Cam.GetComponent<UnityStandardAssets.ImageEffects.Blur>();
        UnityStandardAssets.ImageEffects.Blur blur2 = lvl.Player2Cam.GetComponent<UnityStandardAssets.ImageEffects.Blur>();

        lvl.neuroToxinParticleSystem[0].SetActive(true);
        lvl.neuroToxinParticleSystem[1].SetActive(true);
        fish1.enabled = true;
        fish2.enabled = true;
        blur1.enabled = true;
        blur2.enabled = true;
        float translationSpeed = 0.2f;
        Color pColor = lvl.neuroToxinParticleSystem[0].GetComponent<ParticleSystem>().startColor;
        lvl.neuroToxinParticleSystem[0].GetComponent<ParticleSystem>().Play();
        lvl.neuroToxinParticleSystem[1].GetComponent<ParticleSystem>().Play();

        while(passedTime < lvl.generationOptions.punishmentDuration && !finishedActivating){
            blur1.iterations = 1;
            blur2.iterations = 1;

            if (fish1.strengthX < 0.3) {    //Since we literally raising the same level in both fisheyes, we are testing for just one of them
                fish1.strengthX += Time.deltaTime*translationSpeed;
                fish1.strengthY += Time.deltaTime * translationSpeed;
                fish2.strengthX += Time.deltaTime * translationSpeed;
                fish2.strengthY += Time.deltaTime * translationSpeed;
            }
            if(lvl.neuroToxinParticleSystem[0].GetComponent<ParticleSystem>().startColor.a < 1){
                pColor.a += Time.deltaTime*translationSpeed;
                lvl.neuroToxinParticleSystem[0].GetComponent<ParticleSystem>().startColor = pColor;
                lvl.neuroToxinParticleSystem[1].GetComponent<ParticleSystem>().startColor = pColor;
            }
            passedTime += Time.deltaTime;
            yield return null;
        }
        finishedActivating = true;
        while(passedTime > 0 && finishedActivating){

            blur1.iterations = 0;
            blur2.iterations = 0;

            if (fish1.strengthX > 0.0)
            {    //Since we literally raising the same level in both fisheyes, we are testing for just one of them
                fish1.strengthX -= Time.deltaTime * translationSpeed;
                fish1.strengthY -= Time.deltaTime * translationSpeed;
                fish2.strengthX -= Time.deltaTime * translationSpeed;
                fish2.strengthY -= Time.deltaTime * translationSpeed;
                yield return null;
            }
            if (lvl.neuroToxinParticleSystem[0].GetComponent<ParticleSystem>().startColor.a > 0)
            {
                pColor.a -= Time.deltaTime*translationSpeed;
                lvl.neuroToxinParticleSystem[0].GetComponent<ParticleSystem>().startColor = pColor;
                lvl.neuroToxinParticleSystem[1].GetComponent<ParticleSystem>().startColor = pColor;
            }
            passedTime -= Time.deltaTime;
        }
        fish1.enabled = false;
        fish2.enabled = false;
        blur1.enabled = false;
        blur2.enabled = false;
        lvl.neuroToxinParticleSystem[0].GetComponent<ParticleSystem>().Stop();
        lvl.neuroToxinParticleSystem[0].SetActive(false);
        lvl.neuroToxinParticleSystem[1].GetComponent<ParticleSystem>().Stop();
        lvl.neuroToxinParticleSystem[1].SetActive(false);
    }

    private IEnumerator AnimateOpenShutter()
    {
        Material mat = shutter.GetComponentInChildren<Renderer>().material;
        
        float passedTime = 0;
        if (protectivePanel != null) protectivePanel.GetComponent<Animator>().SetBool("Open", true);
        while(passedTime < shutterOpenDuration)
        {
            float factor = 1 - shutterOpenCurve.Evaluate(passedTime / shutterOpenDuration);

            Vector3 s = shutter.localScale;
            s.y = factor;
            shutter.localScale = s;

            Vector2 texScale = mat.mainTextureScale;
            texScale.y = factor;
            mat.mainTextureScale = texScale;
            /*Color col = protectivePanel.GetComponent<Renderer>().material.color;
            col = new Color(col.r,col.b,col.g,factor);
            print(col);*/
            passedTime += Time.deltaTime;
            yield return null;
        }
        protectivePanel.SetActive(false);
        shutter.gameObject.SetActive(false);
    }

}
