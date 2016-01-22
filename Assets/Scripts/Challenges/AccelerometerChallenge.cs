using UnityEngine;
using System;
using System.Collections;

public class AccelerometerChallenge : AbstractChallenge {

	private const float ACCEL_SCALING_X = 0.75f;

	private AccelPosController accelPosIndicator;

	protected override void InitChallenge(){
		accelPosIndicator = frontGameObject.GetComponentInChildren<AccelPosController> ();
	}
	
	void Update () {
		//Debug.Log ("Accel: "+frontInputState.Acceleration.ToString());
		Vector3 accel = frontInputState.Acceleration.normalized;
		Vector2 movement = new Vector2(accel.x, accel.z);

		float width = safe.GetUsableFrontWidth ();
		float height = safe.GetUsableFrontHeight ();

		movement.x *= Math.Abs (width) * ACCEL_SCALING_X;
		movement.y *= Math.Abs (height);

		movement.x += width / 2;
		movement.y += height / 2;

		movement.x = Mathf.Clamp (movement.x, width + 0.05f, -0.05f);
		movement.y = Mathf.Clamp (movement.y, height + 0.05f, -0.05f);

		accelPosIndicator.SetValue(movement);
	}
}
