using System;
using System.IO;
using System.IO.Ports;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour {

	public Dropdown player1PortSelection;
	public Dropdown player2PortSelection;

	public Text player1StatusText;
	public Text player2StatusText;
    public Text errorText;

    public Toggle showHelpToggle;

	void Start()
	{
        showHelpToggle.isOn = (PlayerPrefs.GetInt("EnableTutorial", 1) > 0 ? true : false);
        showHelpToggle.onValueChanged.AddListener(flag => PlayerPrefs.SetInt("EnableTutorial", (flag ? 1 : 0)));


		// Add active ports to both dropdowns
		foreach (string port in SerialPort.GetPortNames()) {
			Dropdown.OptionData portname = new Dropdown.OptionData(port);
			player1PortSelection.options.Add(portname);
			player2PortSelection.options.Add(portname);

            // if port was previously selected, select it now
            if (PlayerPrefs.GetString("Player1Port", "Keyboard") == port)
                player1PortSelection.value = player1PortSelection.options.Count - 1;
            if (PlayerPrefs.GetString("Player2Port", "Keyboard") == port)
                player2PortSelection.value = player2PortSelection.options.Count - 1;
        }

        player1PortSelection.onValueChanged.AddListener(_ => TestPlayer1());
        player2PortSelection.onValueChanged.AddListener(_ => TestPlayer2());

        // initially test selected ports
        TestPlayer1();
        TestPlayer2();
	}

    private bool testPort(string portName)
    {
        // tests if cactus controller on portName can be reached
        try
        {
            SerialPort port = new SerialPort(portName, 115200);
            port.WriteTimeout = 500;
            port.ReadTimeout = 500;
            port.Open();

            // turn on an LED and check response to see if we are indeed connected to a cactus controller
            port.WriteLine("l 3 1");
            string response = port.ReadLine();
            if (response != "Done.")
                throw new IOException("Invalid response: " + response);

            // leave the LED on for a bit so the controller can be visibly identified;
            // yes, we are freezing the main thread here, but it only happens briefly and on a seldom UI action
            System.Threading.Thread.Sleep(150);

            port.WriteLine("l 3 0");
            port.ReadLine();
            return true;
        }
        catch (IOException e)
        {
            Debug.LogWarning(errorText.text = "Error connecting to port " + portName + ": " + e.Message);
        }
        catch (TimeoutException e)
        {
            Debug.LogWarning(errorText.text = "Controller on port " + portName + " caused a timeout.");
        }

        return false;
    }

	public void TestPlayer1()
    {
        errorText.text = "";

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

	public void TestPlayer2()
    {
        errorText.text = "";

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
