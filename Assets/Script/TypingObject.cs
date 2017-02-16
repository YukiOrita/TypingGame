using UnityEngine;
using System.Collections;

public class TypingObject : MonoBehaviour {

	public TextMesh InputStringTextMesh;
	public TextMesh AlphabetTextMesh;
	public TextMesh ScoreTextMesh;
	TypingSystem ts;
	public Dictionary dictionary;
	public Score score;

	// Use this for initialization
	void Start () {

		ts = new TypingSystem ();
		ts.SetInputString (dictionary.GetRandomWord());
		UpdateText ();
	}
		
	// Update is called once per frame
	void Update (){

		string[] keys = {"a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "-",};
		foreach (string key in keys) {
			if(Input.GetKeyDown(key)){
				if (ts.InputKey (key) == 1) {
					UpdateText ();
				}
				break;
	}
		}
		if (ts.isEnded ()) {
			ts.SetInputString (dictionary.GetRandomWord ());
			score.AddScore (10);
			UpdateText ();
		}
	}

	void UpdateText () {

		InputStringTextMesh.text =  "<color=red>" + ts.GetInputedString() + "</color>" + ts.GetRestString();
		AlphabetTextMesh.text = "<color=red>" + ts.GetInputedKey() + "</color>" + ts.GetRestKey();
	}

}

