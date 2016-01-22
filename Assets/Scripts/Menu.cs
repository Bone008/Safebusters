using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using System.IO.Ports;

public class Menu : MonoBehaviour {

	public Dropdown player1PortSelection;
	public Dropdown player2PortSelection;

	public Text player1StatusText;
	public Text player2StatusText;

	void Start()
	{
		// Set ports initially on keyboard for both players
		PlayerPrefs.SetString("Player1Port", "Keyboard");
		PlayerPrefs.SetString("Player2Port", "Keyboard");

		// Add active ports to both dropdowns
		foreach (string port in System.IO.Ports.SerialPort.GetPortNames ()) {
			Dropdown.OptionData portname = new Dropdown.OptionData(port);
			player1PortSelection.options.Add(portname);
			player2PortSelection.options.Add(portname);
		}
	}

	private bool testPort(string portName){
		// tests if cactus controller on portName can be reached
		try {
			SerialPort port = new SerialPort(portName, 115200);
			port.Open();
			
			if(port.IsOpen){
				port.Close();
				return true;
			}
		}catch(IOException e){}

		return false;
	}

	public void ConnectPlayer1()
	{
		// update status depending on selected port
		string portName = player1PortSelection.captionText.text;
		if (portName == "Keyboard") {
			PlayerPrefs.SetString("Player1Port", "Keyboard");
			player1StatusText.text = "Keyboard";
			player1StatusText.color = Color.cyan;
		} else {
			if(testPort (portName)){
				PlayerPrefs.SetString("Player1Port", portName);
			   	player1StatusText.text = "Cactus Connected";
				player1StatusText.color = Color.green;
			}else{
				PlayerPrefs.SetString("Player1Port", "Keyboard");
				player1StatusText.text = "Port Failure";
				player1StatusText.color = Color.red;
			}
		}
	}

	public void ConnectPlayer2()
	{
		// update status depending on selected port
		string portName = player2PortSelection.captionText.text;
		if (portName == "Keyboard") {
			PlayerPrefs.SetString("Player2Port", "Keyboard");
			player2StatusText.text = "Keyboard";
			player2StatusText.color = Color.cyan;
		} else {
			if(testPort (portName)){
				PlayerPrefs.SetString("Player2Port", portName);
				player2StatusText.text = "Cactus Connected";
				player2StatusText.color = Color.green;
			}else{
				PlayerPrefs.SetString("Player2Port", "Keyboard");
				player2StatusText.text = "Port Failure";
				player2StatusText.color = Color.red;
			}				
		}
	}

	public void StartGame()
    {
        Application.LoadLevel("Main");
    }

    public void ExitGame()
    {
		Application.Quit();
    }
}
