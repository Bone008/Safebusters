using UnityEngine;
using System.Collections;

public class AccelPosController : MonoBehaviour {

	public Transform handle;

	public void SetValue(Vector2 pos)
	{
		handle.localPosition = new Vector3(pos.x, pos.y, 0);
	}
}
