using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

public class InterconnectedDialChallenge : AbstractChallenge {

    Dictionary<GameObject,List<GameObject>> dials = new Dictionary<GameObject, List<GameObject>>();  //used to store simple reference and to store which dial rotates which ones
    List<GameObject> mainDialList = new List<GameObject>(); //So we can easily switch between dials with index value

    int currentlySelectedDial = 0;
    float errorRange = 10.0f;

    protected override void InitChallenge()
    {

        foreach (Transform child in frontGameObject.transform.GetChild(0))
        {
            if (child.tag == "rotatingDial")
            {
                dials.Add(child.gameObject, new List<GameObject>());
                mainDialList.Add(child.gameObject);
            }
        }

        //Assign each dial one of the others
        foreach (KeyValuePair<GameObject,List<GameObject>> entry in dials)
        {
            int assignedDialAmount = UnityEngine.Random.Range(0, dials.Count);
            foreach (GameObject dial in GetRandomKeyFromDic(dials).Take(assignedDialAmount))
            {
                if(dial != entry.Key)    entry.Value.Add(dial);
            }
        }

        //Rotate dials randomly to create a new puzzle
        foreach (KeyValuePair<GameObject, List<GameObject>> entry in dials) {
            float amountToRotate = UnityEngine.Random.Range(0,360);
            entry.Key.transform.Rotate(0, 0, amountToRotate);
            foreach (GameObject dial in entry.Value) {
                dial.transform.Rotate(0, 0, amountToRotate);
            }
        }
    }

    //Copied from here: http://stackoverflow.com/questions/1028136/random-entry-from-dictionary (slightly modified)
    public IEnumerable<TKey> GetRandomKeyFromDic<TKey, TValue>(IDictionary<TKey, TValue> dict)
    {
        System.Random rand = new System.Random();
        List<TKey> keys = Enumerable.ToList(dict.Keys);
        int size = dict.Count;
        while (true)
        {
            yield return keys[rand.Next(size)];
        }
    }

    void Update() {
        if (!hasFocusFront)
            return;

        GameButton pressed = frontInputState.PressedButtons;    //We need this so we can switch between dials
        float knobPosition = frontInputState.GetAnalogInput(0);    //We need this so we can rotate the dials (obviously)

        float knobRotation = (knobPosition - 0.5f);
        // if the knob is close to the center, treat as neutral input
        if (Mathf.Abs(knobRotation) < 0.05f)
            knobRotation = 0;
        else
            knobRotation *= 90 * Time.deltaTime;


        if (pressed == GameButton.Left) {
            currentlySelectedDial = (currentlySelectedDial + mainDialList.Count - 1) % mainDialList.Count; //+ mDL.count ,so we dont go into negative
            //print(currentlySelectedDial);
        }
        if (pressed == GameButton.Right)
        {
            currentlySelectedDial = (currentlySelectedDial + 1) % mainDialList.Count;
            //print(currentlySelectedDial);
        }
        //Color currently selected dial and rotate it and its connected dials too
        foreach (GameObject go in dials.Keys) {
            if (go == mainDialList[currentlySelectedDial])
            {
                go.GetComponent<Renderer>().material.color = Color.red;
                go.transform.Rotate(0, 0, knobRotation);
                foreach (GameObject dial in dials[go]) {
                    dial.transform.Rotate(0, 0, knobRotation);
					dial.GetComponent<Renderer>().material.color = Color.magenta;
                }
            }
            else if(!dials[mainDialList[currentlySelectedDial]].Contains(go)){
                go.GetComponent<Renderer>().material.color = Color.white;
            }
        }

        //Check if dials are in correct position, if yes we won!
        int dialsInCorrectPos = 0;
        foreach (GameObject dial in mainDialList) {
			Color originalColor = dial.GetComponent<Renderer>().material.color;
			if (dial.transform.localEulerAngles.z < errorRange || dial.transform.localEulerAngles.z > 360-errorRange) {
                dialsInCorrectPos++;
				dial.GetComponent<Renderer>().material.color = Color.green;
                //dial.transform.localRotation = Quaternion.Euler(0,0,0);
			}else{
				dial.GetComponent<Renderer>().material.color = originalColor;
			}
        }

		//print (dialsInCorrectPos);
        if (dialsInCorrectPos == 4) {
			foreach (GameObject dial in mainDialList) {
				dial.transform.localRotation = Quaternion.Euler(0,0,0);
			}
            safe.SolveChallenge();
        }
    }
}
