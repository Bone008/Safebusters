using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Model;

public class LevelComponent : MonoBehaviour {

    //Shown in Inspector
	
	public GameObject safePrefab;
    [SerializeField]
    [Tooltip("The final scale of a safe instance")]
    public float safeSize = 1;

    //Hidden in Inspector
	[HideInInspector]
	public List<GameObject> safeGameObjects;
    [HideInInspector]
    public Level level;
	[HideInInspector]
	public int focussedSafe = 0;

    void Start () {
		// init level
		level = new Level();

		// instantiate safes from prefab
		int i = 0;
		float safeWidth = safePrefab.GetComponent<Renderer> ().bounds.size.x * safeSize;
        float safeHeight = safePrefab.GetComponent<Renderer> ().bounds.size.y * safeSize;
		Vector3 offset;
		Quaternion rotation;
		safeGameObjects = new List<GameObject> ();

        int rows = Mathf.CeilToInt(level.SAFE_COUNT / (float)level.SafesPerRow);
        Vector3 baseOffset = new Vector3((-level.SafesPerRow + 1) * safeWidth / 2, rows * safeHeight - safeHeight / 2, 0);

		for(int j = 0; j < level.SAFE_COUNT; j++) {

            //Instantiate safe
            var go = Instantiate(safePrefab, Vector3.zero, Quaternion.identity) as GameObject;
            go.transform.SetParent(transform);

            //Scale safe
            go.transform.localScale *= safeSize;

            // offset from top left position
            offset = new Vector3(safeWidth * (i % level.SafesPerRow), -safeHeight * (int)(i / level.SafesPerRow), 0);

            Safe tmp_safe = level.CreateSafeScriptOnGO(go);

			// rotation: safe backwards -> turn around
			if(tmp_safe.IsBackwards){
				rotation = Quaternion.Euler(0, 180, 0);
			}else{
				rotation = Quaternion.identity;
			}

            go.transform.localPosition = baseOffset + offset;
            go.transform.localRotation = rotation;
            go.GetComponent<Renderer>().material.color = tmp_safe.DisplayColor;
            go.name = "Safe " + i;
			// add camera waypoint
			safeGameObjects.Add(go);
			i++;
		}
	}
	
	void Update () {
		CustomButtonReader input = GetComponent<CustomButtonReader> ();
		Safe fs = level.Safes [focussedSafe];
		if (fs.IsActive) {
			InputResult result = fs.Challenge.receiveFrontInput (null, input.inputState);
			if (result == InputResult.Solved) {
				Debug.Log ("solved!");
				fs.IsActive = false;
			} else if (result == InputResult.Error) {
				Debug.Log ("lost life!");
			}
		}
	}
}
