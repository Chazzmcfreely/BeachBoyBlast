using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

	public Ball ball;

	public int player1Score;

	public int player2Score;

	public Text player1ScoreText;

	public Text player2ScoreText;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		player1ScoreText.text = "" + player1Score;

		player2ScoreText.text = "" + player2Score;

		if (player1Score == 6) {
			SceneManager.LoadScene ("End Scene");
		}

		if (player2Score == 6) {
			SceneManager.LoadScene ("End Scene");
		}
	}
}
