using UnityEngine;
using System.Collections;

public class Score1 : MonoBehaviour {

	public int score;
	public TextMesh ScoreTextMesh;


	// Use this for initialization
	void Start () {
		
		score = PlayerPrefs.GetInt("GAMESCORE");
		ScoreTextMesh.text = "Score: " + score.ToString();
	}
		
	// Update is called once per frame
	void Update () {

	
	}
}
