using UnityEngine;
using System;
using System.Collections;

public class AccelerometerChallenge : AbstractChallenge {

	private const float ACCEL_SCALING = 0.75f;

	private AccelPosController accelPosIndicator;

	protected override void InitChallenge(){
		accelPosIndicator = frontGameObject.GetComponentInChildren<AccelPosController> ();
	}
	
	void Update () {
		//Debug.Log ("Accel: "+frontInputState.Acceleration.ToString());

		Vector2 movement = new Vector2(frontInputState.Acceleration.x, frontInputState.Acceleration.z);
		float width = safe.GetUsableFrontWidth ();
		float height = safe.GetUsableFrontHeight ();

		//Debug.Log ("Width: " + width + " Height: " + height);
		//TODO: normalisieren
		movement.x *= Math.Abs (width);
		movement.y *= Math.Abs (height);

		movement.x *= ACCEL_SCALING;
		movement.y *= ACCEL_SCALING;

		movement.x += width / 2;
		movement.y += height / 2;

		movement.x = Mathf.Clamp (movement.x, width, 0.0f);
		movement.y = Mathf.Clamp (movement.y, height, 0.0f);

		//Debug.Log ("Movement: " + movement.ToString ());
		accelPosIndicator.SetValue(movement);
	}
}
