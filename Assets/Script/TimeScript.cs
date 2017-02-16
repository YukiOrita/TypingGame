using UnityEngine;
using System.Collections;

public class TimeScript : MonoBehaviour {

	public float countTime = 60;
	public TextMesh timerTextMesh;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

		countTime -= Time.deltaTime; //スタートしてからの秒数を格納
		timerTextMesh.text = (countTime.ToString());

		if (countTime < 0) countTime = 0;
		GetComponent<TextMesh> ().text = ((int)countTime).ToString ();
	
	}
}