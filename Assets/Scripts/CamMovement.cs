using UnityEngine;
using System.Collections;

public class CamMovement : MonoBehaviour {

	public GameObject level;
	public float zOffset;
	public float speed;

	int focussedSafe = 0;

	void Update () {	
		// amount of safes == childcount of level gameobject	
		int safeCount = level.transform.childCount;

		// if focussedSafe isnt active anymore -> focus next active one
		while (!level.GetComponent<LevelComponent> ().level.Safes [focussedSafe].IsActive) {
			focussedSafe = (focussedSafe + 1) % safeCount;
		}

		// TODO: replace by cactus-controller buttons
		if (Input.GetButtonDown ("Horizontal")) {
			// focus next active safe
			do {
				if (Input.GetAxisRaw ("Horizontal") > 0) {
					focussedSafe = (focussedSafe + 1) % safeCount;
				} else {
					focussedSafe = (focussedSafe + safeCount - 1) % safeCount;
				}
			} while(!level.GetComponent<LevelComponent> ().level.Safes [focussedSafe].IsActive);
		}

		// smoothly move cam to focussed safe
		transform.position = Vector3.Lerp (
			transform.position, 
			level.transform.GetChild (focussedSafe).transform.position + new Vector3(0, 0, zOffset), 
			Time.deltaTime * speed
		);
	}
}
