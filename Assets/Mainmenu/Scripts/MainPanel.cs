using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainPanel : MonoBehaviour {

	public NetworkManager nm;
	public string gameScene;
	public Canvas canvas;

	public void Update(){
		if (nm.numPlayers == 2) {
			changeLevel ();
		}

	}
	public void OnClickHost() {
		nm.StartHost ();
	}

	public void OnClickJoin() {
		nm.StartClient();
		changeLevel ();
	}

	void changeLevel(){
		nm.ServerChangeScene ("game");
		canvas.enabled = false;
		this.enabled = false;
	}

}
