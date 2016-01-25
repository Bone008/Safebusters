using UnityEngine;
using System;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

public class AccelerometerChallenge : AbstractChallenge {

	private const float ACCEL_SCALING_X = 0.75f;
    
    private AccelPosController accelPosIndicator;

    private int solutionX;
    private int solutionY;

	protected override void InitChallenge(){
        // find cube to show accelerometer position
		accelPosIndicator = frontGameObject.GetComponentInChildren<AccelPosController> ();

        // get sprites
        List<Sprite> sprites = GameObject.FindWithTag("GameController").GetComponent<Level>().iconSprites.ToList();
        List<float> spriteRotations = GameObject.FindWithTag("GameController").GetComponent<Level>().iconSpriteRotations.ToList();

        // find a right solution
        int solution = UnityEngine.Random.Range(0, 5);

        // set random sprites
        for(int i = 0; i < 6; i++)
        {
            // get Image
            Transform icon = frontGameObject.transform.GetChild(1).GetChild(i);

            // set random sprite
            int r = UnityEngine.Random.Range(0, sprites.Count);
            icon.GetComponent<Image>().sprite = sprites[r];
            icon.localRotation = Quaternion.Euler(0, 0, spriteRotations[r]);

            // set solution
            if(i == solution)
            {
                solutionX = i % 3;
                solutionY = (int)(i / 3);
                Transform backside = backGameObject.transform.GetChild(0).GetChild(0);
                backside.GetComponent<Image>().sprite = sprites[r];
                backside.localRotation = Quaternion.Euler(0, 0, spriteRotations[r]);
            }

            // remove sprite from list -> no double sprites
            sprites.RemoveAt(r);
            spriteRotations.RemoveAt(r);
        }

    }
	
	void Update () {
		// set cube position from accelerometer values
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

        // see if solved
        int positionX = (int)(3 * (movement.x + 0.05f) / (width + 0.1f)); // 0, 1, or 2. 0 = left, 1 = middle, 2 = right
        int positionY = (int)(2 * (movement.y + 0.05f) / (height + 0.1f)); // 0 or 1. 0 = top, 1 = bottom

        if (frontInputState.PressedButtons != GameButton.None)
        {
            if (positionX == solutionX && positionY == solutionY)
            {
                safe.SolveChallenge();
            }
            else
            {
                safe.FailChallenge();
            }
        }
        
    }
}
