using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerManager : MonoBehaviour {

	//[Syncvar]
	public int playerLives = 3;
	public string playerName = "bob";
	public Color tankColor;

	public Text livesText;

	GameManager gameManager;

	void Awake() {
		DontDestroyOnLoad(gameObject);
	}

	public void SetupPlayerLives() {

		if (livesText == null) {
			Text[] texts = FindObjectsOfType<Text>();
			foreach(Text t in texts) {
				if(t.name == "LivesText") {
					livesText = t;
					break;
				}
			}
		}

		livesText.text = playerLives.ToString();

		gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
		gameManager.CmdNewPlayerToList(playerName);
	}

	public void RemovePlayerLife() {

		playerLives--;

		if (playerLives > 0) {
			livesText.text = playerLives.ToString();
		}
		else {
			livesText.text = "0";
			Debug.Log("dead");
		}
	}

	public int ReturnPlayerLives() {
		return playerLives;
	}
}
