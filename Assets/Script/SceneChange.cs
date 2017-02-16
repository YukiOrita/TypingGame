using UnityEngine;
using System.Collections;

public class SceneChange : MonoBehaviour {

	public void toMainScene()
	{
		UnityEngine.SceneManagement.SceneManager.LoadScene("TypingGame");// ←new!
	}

	// Use this for initialization
	void Start () {

		}
	
	// Update is called once per frame
	void Update () {
	
		if (Input.GetKeyDown (KeyCode.Space)) {
			UnityEngine.SceneManagement.SceneManager.LoadScene ("TypingGame");
		}
}
}