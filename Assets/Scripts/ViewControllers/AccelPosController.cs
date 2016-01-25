using UnityEngine;
using System;
using System.Collections;

public class AccelPosController : MonoBehaviour {

    private const OFFSET_Z = 0.02f;
	public Transform handle;

	public void SetValue(Vector2 pos)
	{
		handle.localPosition = new Vector3(pos.x , pos.y, OFFSET_Z);
	}
}
