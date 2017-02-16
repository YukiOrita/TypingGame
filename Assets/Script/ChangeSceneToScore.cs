using UnityEngine;
using System.Collections;

public class ChangeSceneToScore : MonoBehaviour {
	public Score scoreInsctance;

	// Use this for initialization
	void Start () {
	
		//DelayMethodを3.5秒後に呼び出す
		Invoke("DelayMethod", 10f);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void DelayMethod()
	{

		PlayerPrefs.SetInt("GAMESCORE", scoreInsctance.score); 
		UnityEngine.SceneManagement.SceneManager.LoadScene ("Score");
	}
}
