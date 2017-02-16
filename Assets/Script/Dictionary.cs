using UnityEngine;
using System.Collections;

public class Dictionary : MonoBehaviour {

	public TextAsset resouce;
	string[] words;
	public string GetRandomWord(){
		return words[Random.Range(0, words.Length)];
	}

	void Awake(){
		LoadDictionary ();
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void LoadDictionary(){
		string trimedData = resouce.text.Replace ("\r", "");
		char[] dem = {'\n'};
		words = trimedData.Split(dem);
	}
}
