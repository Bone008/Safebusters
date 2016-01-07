using UnityEngine;
using System.Collections;

public class CamMovement : MonoBehaviour {

	public LevelComponent level;
	public float zOffset;
	public float speed;

	int focussedSafe = 0;

	void Update () {
		// amount of safes (shorter)
		int safeCount = level.safeGameObjects.Count;

		// if focussedSafe isnt active anymore -> focus next active one
		while (!level.level.Safes [focussedSafe].IsActive) {
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
			} while(!level.level.Safes [focussedSafe].IsActive);
		}

		// smoothly move cam to focussed safe
		transform.position = Vector3.Lerp (transform.position, level.safeGameObjects[focussedSafe].transform.position + new Vector3(0, 0, zOffset), Time.deltaTime * speed);
	}
}
