using UnityEngine;
using System.Collections;

public class Score : MonoBehaviour {

	public int score;
	public TextMesh ScoreTextMesh;


	// Use this for initialization
	void Start () {
		
		score = 0;
	}

	public void AddScore (int addScore) {
		score += addScore;
	}


	
	// Update is called once per frame
	void Update () {

		ScoreTextMesh.text = "score : " + score;
		DontDestroyOnLoad (gameObject);
	
	}
}
