using UnityEngine;
using System.Collections;
using Model;

public class LevelComponent : MonoBehaviour {

	public Level level;
	public GameObject safePrefab;

	void Start () {
		// init level
		level = new Level ();

		// instantiate safes from prefab
		int i = 0;
		float safeWidth = safePrefab.GetComponent<Renderer> ().bounds.size.x;
		float safeHeight = safePrefab.GetComponent<Renderer> ().bounds.size.y;
		Vector3 offset;
		Quaternion rotation;

		foreach(var tmp_safe in level.Safes){
			// offset from prefab's original position
            offset = new Vector3(safeWidth * (i % level.SafesPerRow), -safeHeight * (int)(i / level.SafesPerRow), 0);

			// rotation: safe backwards -> turn around
			if(tmp_safe.IsBackwards){
				rotation = Quaternion.Euler(0, 180, 0);
			}else{
				rotation = Quaternion.identity;
			}

			// instantiate new safe with offset and rotation, set level as parent
			var go = Instantiate(safePrefab, safePrefab.transform.position + offset, rotation) as GameObject;
			go.transform.parent = transform;

            go.GetComponent<Renderer>().material.color = tmp_safe.DisplayColor;

			i++;
		}
	}
	
	void Update () {
	
	}
}
