using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Model;

public class LevelComponent : MonoBehaviour {

    //Shown in Inspector
	public Level level;
	public GameObject safePrefab;
    [SerializeField]
    [Tooltip("The final scale of a safe instance")]
    float safeSize = 1;

    //Hidden in Inspector
	[HideInInspector]
	public List<GameObject> safeGameObjects;

	void Start () {
		// init level
		level = new Level ();

		// instantiate safes from prefab
		int i = 0;
		float safeWidth = safePrefab.GetComponent<Renderer> ().bounds.size.x*(safeSize);
        float safeHeight = safePrefab.GetComponent<Renderer> ().bounds.size.y * (safeSize);
		Vector3 offset;
		Quaternion rotation;
		safeGameObjects = new List<GameObject> ();

		foreach(var tmp_safe in level.Safes){

            //Instantiate safe at origo
            var go = Instantiate(safePrefab, Vector3.zero, Quaternion.identity) as GameObject;

            //Scale safe
            go.transform.localScale *= safeSize;

            // offset from prefab's original position
            offset = new Vector3(safeWidth * (i % level.SafesPerRow), -safeHeight * (int)(i / level.SafesPerRow), 0);

			// rotation: safe backwards -> turn around
			if(tmp_safe.IsBackwards){
				rotation = Quaternion.Euler(0, 180, 0);
			}else{
				rotation = Quaternion.identity;
			}
            go.transform.position = offset;
            go.transform.rotation = rotation;
			go.transform.parent = transform;
            go.GetComponent<Renderer>().material.color = tmp_safe.DisplayColor;

			// add camera waypoint
			safeGameObjects.Add(go);

			i++;
		}
	}
	
	void Update () {
	
	}
}
