using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class TypingSystem {

	//入力する文字列のオリジナル
	string InputStringOrigin;
	//入力する文字列
	string InputString;
	//入力する文字列のオリジナル
	string RestStringOrigin;
	//残りの文字列
	string RestString;
	//入力済みのキー
	string InputedKeys;

	//次に入力すべき文字パターン
	string next;

	//直前に入力成功したのが[ん]の２個目のnである
	bool last_n_secondary;

	//パターンチップのリスト
	List<PatternTip> patternTips = new List<PatternTip> ();


	//入力する文字列(ひらがなorカタカナ)をセットし初期化する.
	public void SetInputString(string s){
		InputStringOrigin = InputString = RestStringOrigin = s;
		RestString = StringToHiragana (s);
		InputedKeys = "";
		last_n_secondary = false;
		patternTips.Clear ();
		UpdatePatternTips ();
	}
	//入力されたキーを引数に，判定を行う．
	//入力に成功すれば1，失敗すれば0を返す．
	public int InputKey(string input){

		List<int> removeList = new List<int> ();

		bool inputSucceeded = false;

		if (input.Length > 1)
			input = input [0].ToString ();

		if(input == "n" && InputedKeys.Length > 0 && last_n_secondary == false){

			if(InputedKeys[InputedKeys.Length - 1] == 'n' ){
				bool canAddN = true;
				for (int i =patternTips.Count -1; i>=0; i--) {
					char f = patternTips[i].alphabet[0];
					if( f != 'n' && f != 'y' && f != 'a' && f != 'i' && f != 'u' && 
					   f != 'e' && f != 'o'){

					}else{
						canAddN = false;
					}
				}
				if(canAddN){
					InputedKeys += "n";
					last_n_secondary = true;
					return 1;
				}
			}
		}

		for (int i =patternTips.Count -1; i>=0; i--) {
			patternTips [i].isPoped = false;
		}

		for (int i =patternTips.Count -1; i>=0; i--) {
			if (patternTips [i].alphabet.Length == 0)
				continue;
			if (patternTips [i].alphabet [0].ToString () == input) {
				patternTips [i].Pop ();
				inputSucceeded = true;
			}
		}

		if (inputSucceeded) {
			for (int i = patternTips.Count - 1; i>=0; i--) {
				if (patternTips [i].isPoped == false) {
					patternTips.Remove (patternTips [i]);
				}
			}
			for (int i = patternTips.Count - 1; i>=0; i--) {
				if (patternTips [i].alphabet == "") {
					PatternTip tip = patternTips [i];
					patternTips.Clear ();
					if (tip.remainder == "") {
						RestString = RestString.Substring (next.Length);
						RestStringOrigin = RestStringOrigin.Substring (next.Length);
						UpdatePatternTips ();
					} else {
						RestString = RestString.Substring (next.Length - tip.remainder.Length);
						RestStringOrigin = RestStringOrigin.Substring (next.Length - tip.remainder.Length);
						next = tip.remainder;
						GetPatternTips (patternTips, next);
					
					}
				}
			}
			InputedKeys += input;
		}

	
		input = input.Substring(1);

		if(inputSucceeded){
			last_n_secondary = false;
			return 1;
		}else return 0;
	}
	//入力するすべての文字列を返す．
	public string GetInputString(bool isOrigin = true, bool isKatakana = false){
		string returnText;
		if (isOrigin)
			returnText = InputStringOrigin;
		else
			returnText = InputString;

		if (isKatakana && isOrigin == false)
			return StringToKatakana (returnText);
		else 
			return returnText;
	}
	//残りの文字列を返す．
	public string GetRestString(bool isOrigin = true, bool isKatakana = false){
		string returnText;
		if (isOrigin)
			returnText = RestStringOrigin;
		else
			returnText = RestString;

		if (isKatakana && isOrigin == false)
			return StringToKatakana (returnText);
		else 
			return returnText;
	}

	//残りの入力すべきキーを候補を一つ返す．
	public string GetRestKey(){
		string restStr = "";
		string restKeys = "";
		if (patternTips.Count > 0) {
			restStr = RestString.Substring(next.Length - patternTips[0].remainder.Length);
			restKeys += patternTips [0].alphabet;
		}
		while (restStr.Length > 0) {
			string head = GetNextPattern(restStr);
			if(head == "")break;
			List<PatternTip> tips = new List<PatternTip>(); 
			GetPatternTips(tips, head);
			restKeys += tips [0].alphabet;
			restStr = restStr.Substring (head.Length - tips [0].remainder.Length);
		}
		return restKeys;
	}
	//すでに入力済みの文字列を返す．
	public string GetInputedString(bool isOrigin = true, bool isKatakana = false){
		int length = InputString.Length - RestString.Length;
		if (isOrigin) {
			return InputStringOrigin.Substring (0, length);
		} else {
			if (isKatakana) {
				return StringToKatakana(InputString.Substring (0, length));
			} else {
				return StringToHiragana(InputString.Substring (0, length));
			}
		}
	}
	//すでに入力成功済みのキーを返す．
	public string GetInputedKey(){
		return InputedKeys;
	}
	//すべての文字列の判定が終了したかどうかを返す．
	public bool isEnded(){
		if (next == "") {
			return true;
		}
		else
			return false;
	}

	void UpdatePatternTips(){
		next = GetNextPattern (RestString);
		GetPatternTips (patternTips, next);
	}

	string GetNextPattern(string s){
		if (s == "") {
			return "";
		}
		char head = s [0];
		if (head == 'ん') {
			if (s.Length > 1) {
				char n = s [1];
				if (n == 'あ' || n == 'い' || n == 'う' || n == 'え' || n == 'お' || n == 'や' || n == 'ゆ' || n == 'よ' ||
					n == 'な' || n == 'に' || n == 'ぬ' || n == 'ね' || n == 'の' || n == 'ん') {
					return head + GetNextPattern (s.Substring (1));
				} else
					return s.Substring(0, 1);
			} else
				return ".";
		} else if (head == 'し' || head == 'ち' || head == 'き' || head == 'ぎ' || head == 'ぢ' || head == 'じ' ||
				head == 'に' || head == 'ひ' || head == 'び' || head == 'ぴ' || head == 'み' || head == 'り' ||
				head == 'て' || head == 'で') {
			if (s.Length > 1) {
				char n = s [1];
				if (n == 'ゃ' || n == 'ぃ' || n == 'ゅ' || n == 'ょ' || n == 'ぇ') {
					return s.Substring (0, 2);
				} else
					return head.ToString();
			} else
				return head.ToString();
		} else if (head == 'ふ') {
			if (s.Length > 1) {
				char n = s [1];
				if (n == 'ぁ' || n == 'ぃ' || n == 'ぇ' || n == 'ぉ' || n == 'ゃ' || n == 'ゅ' || n == 'ょ') {
					return s.Substring (0, 2);
				} else
					return head.ToString();
			} else
				return head.ToString();
		} else if (head == 'っ') {
			if (s.Length > 1) {
				char n = s [1];
				if (n == 'あ' || n == 'い' || n == 'う' || n == 'え' || n == 'お' || n == 'ん' || 
				    n == 'な' || n == 'に' || n == 'ぬ' || n == 'ね' || n == 'の') {
					return head.ToString();
				} else
					return head.ToString() + GetNextPattern (s.Substring (1));
			}
		} else if (head == 'い') {
			if(s.Length > 1){
				if( s[1] == 'ぇ'){
					return s.Substring(0, 2);
				}
			}return head.ToString();
		}else if (head == 'う' || head == 'ゔ' || head == 'ヴ') {
			if(s.Length > 1){
				char n = s [1];
				if( s[1] == 'ぁ' || s[1] == 'ぃ' || s[1] == 'ぇ' || s[1] == 'ぉ'){
					return s.Substring(0, 2);
				}
			}return head.ToString();
		}else {
			return head.ToString();
		}
		return "";
	}
	void GetPatternTips(List<PatternTip> tips, string s){
		if (s.Length <= 0)
			return;
		switch (s) {
		case "あ":
			tips.Add(new PatternTip("a"));
			break;
		case "い":
			tips.Add(new PatternTip("i"));
			tips.Add(new PatternTip("yi"));
			break;
		case "う":
			tips.Add(new PatternTip("u"));
			break;
		case "え":
			tips.Add(new PatternTip("e"));
			break;
		case "お":
			tips.Add(new PatternTip("o"));
			break;
		case "いぇ":
			tips.Add(new PatternTip("ye"));
			tips.Add(new PatternTip("i", "ぇ"));
			tips.Add(new PatternTip("yi", "ぇ"));
			break;
		case "うぁ":
			tips.Add(new PatternTip("wha"));
			tips.Add(new PatternTip("u", "ぁ"));
			break;
		case "うぃ":
			tips.Add(new PatternTip("wi"));
			tips.Add(new PatternTip("whi"));
			tips.Add(new PatternTip("u", "ぃ"));
			break;
		case "うぇ":
			tips.Add(new PatternTip("we"));
			tips.Add(new PatternTip("whe"));
			tips.Add(new PatternTip("u", "ぇ"));
			break;
		case "うぉ":
			tips.Add(new PatternTip("who"));
			tips.Add(new PatternTip("u", "ぉ"));
			break;
		case "か":
			tips.Add(new PatternTip("ka"));
			tips.Add(new PatternTip("ca"));
			break;
		case "き":
			tips.Add(new PatternTip("ki"));
			break;
		case "く":
			tips.Add(new PatternTip("ku"));
			tips.Add(new PatternTip("cu"));
			tips.Add(new PatternTip("qu"));
			break;
		case "け":
			tips.Add(new PatternTip("ke"));
			break;
		case "こ":
			tips.Add(new PatternTip("ko"));
			tips.Add(new PatternTip("co"));
			break;
		case "が":
			tips.Add(new PatternTip("ga"));
			break;
		case "ぎ":
			tips.Add(new PatternTip("gi"));
			break;
		case "ぐ":
			tips.Add(new PatternTip("gu"));
			break;
		case "げ":
			tips.Add(new PatternTip("ge"));
			break;
		case "ご":
			tips.Add(new PatternTip("go"));
			break;
		case "さ":
			tips.Add(new PatternTip("sa"));
			break;
		case "し":
			tips.Add(new PatternTip("si"));
			tips.Add(new PatternTip("shi"));
			tips.Add(new PatternTip("ci"));
			break;
		case "す":
			tips.Add(new PatternTip("su"));
			break;
		case "せ":
			tips.Add(new PatternTip("se"));
			tips.Add(new PatternTip("ce"));
			break;
		case "そ":
			tips.Add(new PatternTip("so"));
			break;
		case "ざ":
			tips.Add(new PatternTip("za"));
			break;
		case "じ":
			tips.Add(new PatternTip("zi"));
			tips.Add(new PatternTip("ji"));
			break;
		case "ず":
			tips.Add(new PatternTip("zu"));
			break;
		case "ぜ":
			tips.Add(new PatternTip("ze"));
			break;
		case "ぞ":
			tips.Add(new PatternTip("zo"));
			break;
		case "た":
			tips.Add(new PatternTip("ta"));
			break;
		case "ち":
			tips.Add(new PatternTip("ti"));
			tips.Add(new PatternTip("chi"));
			break;
		case "つ":
			tips.Add(new PatternTip("tu"));
			tips.Add(new PatternTip("tsu"));
			break;
		case "て":
			tips.Add(new PatternTip("te"));
			break;
		case "と":
			tips.Add(new PatternTip("to"));
			break;
		case "だ":
			tips.Add(new PatternTip("da"));
			break;
		case "ぢ":
			tips.Add(new PatternTip("di"));
			break;
		case "づ":
			tips.Add(new PatternTip("du"));
			break;
		case "で":
			tips.Add(new PatternTip("de"));
			break;
		case "ど":
			tips.Add(new PatternTip("do"));
			break;
		case "な":
			tips.Add(new PatternTip("na"));
			break;
		case "に":
			tips.Add(new PatternTip("ni"));
			break;
		case "ぬ":
			tips.Add(new PatternTip("nu"));
			break;
		case "ね":
			tips.Add(new PatternTip("ne"));
			break;
		case "の":
			tips.Add(new PatternTip("no"));
			break;
		case "は":
			tips.Add(new PatternTip("ha"));
			break;
		case "ひ":
			tips.Add(new PatternTip("hi"));
			break;
		case "ふ":
			tips.Add(new PatternTip("hu"));
			tips.Add(new PatternTip("fu"));
			break;
		case "へ":
			tips.Add(new PatternTip("he"));
			break;
		case "ほ":
			tips.Add(new PatternTip("ho"));
			break;
		case "ば":
			tips.Add(new PatternTip("ba"));
			break;
		case "び":
			tips.Add(new PatternTip("bi"));
			break;
		case "ぶ":
			tips.Add(new PatternTip("bu"));
			break;
		case "べ":
			tips.Add(new PatternTip("be"));
			break;
		case "ぼ":
			tips.Add(new PatternTip("bo"));
			break;
		case "ぱ":
			tips.Add(new PatternTip("pa"));
			break;
		case "ぴ":
			tips.Add(new PatternTip("pi"));
			break;
		case "ぷ":
			tips.Add(new PatternTip("pu"));
			break;
		case "ぺ":
			tips.Add(new PatternTip("pe"));
			break;
		case "ぽ":
			tips.Add(new PatternTip("po"));
			break;
		case "ま":
			tips.Add(new PatternTip("ma"));
			break;
		case "み":
			tips.Add(new PatternTip("mi"));
			break;
		case "む":
			tips.Add(new PatternTip("mu"));
			break;
		case "め":
			tips.Add(new PatternTip("me"));
			break;
		case "も":
			tips.Add(new PatternTip("mo"));
			break;
		case "や":
			tips.Add(new PatternTip("ya"));
			break;
		case "ゆ":
			tips.Add(new PatternTip("yu"));
			break;
		case "よ":
			tips.Add(new PatternTip("yo"));
			break;
		case "ら":
			tips.Add(new PatternTip("ra"));
			break;
		case "り":
			tips.Add(new PatternTip("ri"));
			break;
		case "る":
			tips.Add(new PatternTip("ru"));
			break;
		case "れ":
			tips.Add(new PatternTip("re"));
			break;
		case "ろ":
			tips.Add(new PatternTip("ro"));
			break;
		case "わ":
			tips.Add(new PatternTip("wa"));
			break;
		case "を":
			tips.Add(new PatternTip("wo"));
			break;
		case "ん":
			tips.Add(new PatternTip("n"));
			break;
		case ".":
			tips.Add(new PatternTip("nn"));
			break;
		case "ぁ":
			tips.Add(new PatternTip("xa"));
			tips.Add(new PatternTip("la"));
			break;
		case "ぃ":
			tips.Add(new PatternTip("xi"));
			tips.Add(new PatternTip("li"));
			break;
		case "ぅ":
			tips.Add(new PatternTip("xu"));
			tips.Add(new PatternTip("lu"));
			break;
		case "ぇ":
			tips.Add(new PatternTip("xe"));
			tips.Add(new PatternTip("le"));
			break;
		case "ぉ":
			tips.Add(new PatternTip("xo"));
			tips.Add(new PatternTip("lo"));
			break;
		case "ゃ":
			tips.Add(new PatternTip("xya"));
			tips.Add(new PatternTip("lya"));
			break;
		case "ゅ":
			tips.Add(new PatternTip("xyu"));
			tips.Add(new PatternTip("lyu"));
			break;
		case "ょ":
			tips.Add(new PatternTip("xyo"));
			tips.Add(new PatternTip("lyo"));
			break;
		case "ゎ":
			tips.Add(new PatternTip("xwa"));
			tips.Add(new PatternTip("lwa"));
			break;
		case "っ":
			tips.Add(new PatternTip("xtu"));
			tips.Add(new PatternTip("ltu"));
			tips.Add(new PatternTip("xtsu"));
			tips.Add(new PatternTip("ltsu"));
			break;
		case "しゃ":
			tips.Add(new PatternTip("sya"));
			tips.Add(new PatternTip("sha"));
			tips.Add(new PatternTip("si", "ゃ"));
			tips.Add(new PatternTip("shi", "ゃ"));
			tips.Add(new PatternTip("ci", "ゃ"));
			break;
		case "しぃ":
			tips.Add(new PatternTip("syi"));
			tips.Add(new PatternTip("shi"));
			tips.Add(new PatternTip("si", "ぃ"));
			tips.Add(new PatternTip("shi", "ぃ"));
			tips.Add(new PatternTip("ci", "ぃ"));
			break;
		case "しゅ":
			tips.Add(new PatternTip("syu"));
			tips.Add(new PatternTip("shu"));
			tips.Add(new PatternTip("si", "ゅ"));
			tips.Add(new PatternTip("shi", "ゅ"));
			tips.Add(new PatternTip("ci", "ゅ"));
			break;
		case "しぇ":
			tips.Add(new PatternTip("sye"));
			tips.Add(new PatternTip("she"));
			tips.Add(new PatternTip("si", "ぇ"));
			tips.Add(new PatternTip("shi", "ぇ"));
			tips.Add(new PatternTip("ci", "ぇ"));
			break;
		case "しょ":
			tips.Add(new PatternTip("syo"));
			tips.Add(new PatternTip("sho"));
			tips.Add(new PatternTip("si", "ょ"));
			tips.Add(new PatternTip("shi", "ょ"));
			tips.Add(new PatternTip("ci", "ょ"));
			break;
		case "じゃ":
			tips.Add(new PatternTip("ja"));
			tips.Add(new PatternTip("jya"));
			tips.Add(new PatternTip("zya"));
			tips.Add(new PatternTip("ji", "ゃ"));
			tips.Add(new PatternTip("zi", "ゃ"));
			break;
		case "じぃ":
			tips.Add(new PatternTip("jyi"));
			tips.Add(new PatternTip("zyi"));
			tips.Add(new PatternTip("ji", "ぃ"));
			tips.Add(new PatternTip("zi", "ぃ"));
			break;
		case "じゅ":
			tips.Add(new PatternTip("ju"));
			tips.Add(new PatternTip("jyu"));
			tips.Add(new PatternTip("zyu"));
			tips.Add(new PatternTip("ji", "ゅ"));
			tips.Add(new PatternTip("zi", "ゅ"));
			break;
		case "じぇ":
			tips.Add(new PatternTip("je"));
			tips.Add(new PatternTip("jye"));
			tips.Add(new PatternTip("zye"));
			tips.Add(new PatternTip("ji", "ぇ"));
			tips.Add(new PatternTip("zi", "ぇ"));
			break;
		case "じょ":
			tips.Add(new PatternTip("jo"));
			tips.Add(new PatternTip("jyo"));
			tips.Add(new PatternTip("zyo"));
			tips.Add(new PatternTip("ji", "ぉ"));
			tips.Add(new PatternTip("zi", "ぉ"));
			break;
		case "きゃ":
			tips.Add(new PatternTip("kya"));
			tips.Add(new PatternTip("ki", "ゃ"));
			break;
		case "きぃ":
			tips.Add(new PatternTip("kyi"));
			tips.Add(new PatternTip("ki", "ぃ"));
			break;
		case "きゅ":
			tips.Add(new PatternTip("kyu"));
			tips.Add(new PatternTip("ki", "ゅ"));
			break;
		case "きぇ":
			tips.Add(new PatternTip("kye"));
			tips.Add(new PatternTip("ki", "ぇ"));
			break;
		case "きょ":
			tips.Add(new PatternTip("kyo"));
			tips.Add(new PatternTip("ki", "ょ"));
			break;
		case "ぎゃ":
			tips.Add(new PatternTip("gya"));
			tips.Add(new PatternTip("gi", "ゃ"));
			break;
		case "ぎぃ":
			tips.Add(new PatternTip("gyi"));
			tips.Add(new PatternTip("gi", "ぃ"));
			break;
		case "ぎゅ":
			tips.Add(new PatternTip("gyu"));
			tips.Add(new PatternTip("gi", "ゅ"));
			break;
		case "ぎぇ":
			tips.Add(new PatternTip("gye"));
			tips.Add(new PatternTip("gi", "ぇ"));
			break;
		case "ぎょ":
			tips.Add(new PatternTip("gyo"));
			tips.Add(new PatternTip("gi", "ょ"));
			break;
		case "ちゃ":
			tips.Add(new PatternTip("tya"));
			tips.Add(new PatternTip("cha"));
			tips.Add(new PatternTip("cya"));
			tips.Add(new PatternTip("ti", "ゃ"));
			tips.Add(new PatternTip("chi", "ゃ"));
			break;
		case "ちぃ":
			tips.Add(new PatternTip("tyi"));
			tips.Add(new PatternTip("cyi"));
			tips.Add(new PatternTip("ti", "ぃ"));
			tips.Add(new PatternTip("chi", "ぃ"));
			break;
		case "ちゅ":
			tips.Add(new PatternTip("tyu"));
			tips.Add(new PatternTip("chu"));
			tips.Add(new PatternTip("cyu"));
			tips.Add(new PatternTip("ti", "ゅ"));
			tips.Add(new PatternTip("chi", "ゅ"));
			break;
		case "ちぇ":
			tips.Add(new PatternTip("tye"));
			tips.Add(new PatternTip("che"));
			tips.Add(new PatternTip("cye"));
			tips.Add(new PatternTip("ti", "ぇ"));
			tips.Add(new PatternTip("chi", "ぇ"));
			break;
		case "ちょ":
			tips.Add(new PatternTip("tyo"));
			tips.Add(new PatternTip("cho"));
			tips.Add(new PatternTip("cyo"));
			tips.Add(new PatternTip("ti", "ょ"));
			tips.Add(new PatternTip("chi", "ょ"));
			break;
		case "ぢゃ":
			tips.Add(new PatternTip("dya"));
			tips.Add(new PatternTip("di", "ゃ"));
			break;
		case "ぢぃ":
			tips.Add(new PatternTip("dyi"));
			tips.Add(new PatternTip("di", "ぃ"));
			break;
		case "ぢゅ":
			tips.Add(new PatternTip("dyu"));
			tips.Add(new PatternTip("di", "ゅ"));
			break;
		case "ぢぇ":
			tips.Add(new PatternTip("dye"));
			tips.Add(new PatternTip("di", "ぇ"));
			break;
		case "ぢょ":
			tips.Add(new PatternTip("dyo"));
			tips.Add(new PatternTip("di", "ょ"));
			break;
		case "てゃ":
			tips.Add(new PatternTip("tha"));
			tips.Add(new PatternTip("te", "ゃ"));
			break;
		case "てぃ":
			tips.Add(new PatternTip("thi"));
			tips.Add(new PatternTip("te", "ぃ"));
			break;
		case "てゅ":
			tips.Add(new PatternTip("thu"));
			tips.Add(new PatternTip("te", "ゅ"));
			break;
		case "てぇ":
			tips.Add(new PatternTip("the"));
			tips.Add(new PatternTip("te", "ぇ"));
			break;
		case "てょ":
			tips.Add(new PatternTip("tho"));
			tips.Add(new PatternTip("te", "ょ"));
			break;
		case "でゃ":
			tips.Add(new PatternTip("dha"));
			tips.Add(new PatternTip("de", "ゃ"));
			break;
		case "でぃ":
			tips.Add(new PatternTip("dhi"));
			tips.Add(new PatternTip("de", "ぃ"));
			break;
		case "でゅ":
			tips.Add(new PatternTip("dhu"));
			tips.Add(new PatternTip("de", "ゅ"));
			break;
		case "でぇ":
			tips.Add(new PatternTip("dhe"));
			tips.Add(new PatternTip("de", "ぇ"));
			break;
		case "でょ":
			tips.Add(new PatternTip("dho"));
			tips.Add(new PatternTip("de", "ょ"));
			break;
		case "にゃ":
			tips.Add(new PatternTip("nya"));
			tips.Add(new PatternTip("ni", "ゃ"));
			break;
		case "にぃ":
			tips.Add(new PatternTip("nyi"));
			tips.Add(new PatternTip("ni", "ぃ"));
			break;
		case "にゅ":
			tips.Add(new PatternTip("nyu"));
			tips.Add(new PatternTip("ni", "ゅ"));
			break;
		case "にぇ":
			tips.Add(new PatternTip("nye"));
			tips.Add(new PatternTip("ni", "ぇ"));
			break;
		case "にょ":
			tips.Add(new PatternTip("nyo"));
			tips.Add(new PatternTip("ni", "ょ"));
			break;
		case "ひゃ":
			tips.Add(new PatternTip("hya"));
			tips.Add(new PatternTip("hi", "ゃ"));
			break;
		case "ひぃ":
			tips.Add(new PatternTip("hyi"));
			tips.Add(new PatternTip("hi", "ぃ"));
			break;
		case "ひゅ":
			tips.Add(new PatternTip("hyu"));
			tips.Add(new PatternTip("hi", "ゅ"));
			break;
		case "ひぇ":
			tips.Add(new PatternTip("hye"));
			tips.Add(new PatternTip("hi", "ぇ"));
			break;
		case "ひょ":
			tips.Add(new PatternTip("hyo"));
			tips.Add(new PatternTip("hi", "ょ"));
			break;
		case "びゃ":
			tips.Add(new PatternTip("bya"));
			tips.Add(new PatternTip("bi", "ゃ"));
			break;
		case "びぃ":
			tips.Add(new PatternTip("byi"));
			tips.Add(new PatternTip("bi", "ぃ"));
			break;
		case "びゅ":
			tips.Add(new PatternTip("byu"));
			tips.Add(new PatternTip("bi", "ゅ"));
			break;
		case "びぇ":
			tips.Add(new PatternTip("bye"));
			tips.Add(new PatternTip("bi", "ぇ"));
			break;
		case "びょ":
			tips.Add(new PatternTip("byo"));
			tips.Add(new PatternTip("bi", "ょ"));
			break;
		case "ぴゃ":
			tips.Add(new PatternTip("pya"));
			tips.Add(new PatternTip("pi", "ゃ"));
			break;
		case "ぴぃ":
			tips.Add(new PatternTip("pyi"));
			tips.Add(new PatternTip("pi", "ぃ"));
			break;
		case "ぴゅ":
			tips.Add(new PatternTip("pyu"));
			tips.Add(new PatternTip("pi", "ゅ"));
			break;
		case "ぴぇ":
			tips.Add(new PatternTip("pye"));
			tips.Add(new PatternTip("pi", "ぇ"));
			break;
		case "ぴょ":
			tips.Add(new PatternTip("pyo"));
			tips.Add(new PatternTip("pi", "ょ"));
			break;
		case "ふぁ":
			tips.Add(new PatternTip("fa"));
			tips.Add(new PatternTip("fu", "ぁ"));
			break;
		case "ふぃ":
			tips.Add(new PatternTip("fi"));
			tips.Add(new PatternTip("fu", "ぃ"));
			break;
		case "ふぇ":
			tips.Add(new PatternTip("fe"));
			tips.Add(new PatternTip("fu", "ぇ"));
			break;
		case "ふぉ":
			tips.Add(new PatternTip("fo"));
			tips.Add(new PatternTip("fu", "ぉ"));
			break;
		case "ふゃ":
			tips.Add(new PatternTip("fya"));
			tips.Add(new PatternTip("fu", "ゃ"));
			break;
		case "ふゅ":
			tips.Add(new PatternTip("fyu"));
			tips.Add(new PatternTip("fu", "ゅ"));
			break;
		case "ふょ":
			tips.Add(new PatternTip("fyo"));
			tips.Add(new PatternTip("fu", "ょ"));
			break;
		case "みゃ":
			tips.Add(new PatternTip("mya"));
			tips.Add(new PatternTip("mi", "ゃ"));
			break;
		case "みぃ":
			tips.Add(new PatternTip("myi"));
			tips.Add(new PatternTip("mi", "ぃ"));
			break;
		case "みゅ":
			tips.Add(new PatternTip("myu"));
			tips.Add(new PatternTip("mi", "ゅ"));
			break;
		case "みぇ":
			tips.Add(new PatternTip("mye"));
			tips.Add(new PatternTip("mi", "ぇ"));
			break;
		case "みょ":
			tips.Add(new PatternTip("myo"));
			tips.Add(new PatternTip("mi", "ょ"));
			break;
		case "りゃ":
			tips.Add(new PatternTip("rya"));
			tips.Add(new PatternTip("ri", "ゃ"));
			break;
		case "りぃ":
			tips.Add(new PatternTip("ryi"));
			tips.Add(new PatternTip("ri", "ぃ"));
			break;
		case "りゅ":
			tips.Add(new PatternTip("ryu"));
			tips.Add(new PatternTip("ri", "ゅ"));
			break;
		case "りぇ":
			tips.Add(new PatternTip("rye"));
			tips.Add(new PatternTip("ri", "ぇ"));
			break;
		case "りょ":
			tips.Add(new PatternTip("ryo"));
			tips.Add(new PatternTip("ri", "ょ"));
			break;
		case "ゔ":
		case "ヴ":
			tips.Add (new PatternTip ("vu"));
			break;
		case "ヴぁ":
		case "ゔぁ":
			tips.Add (new PatternTip ("va"));
			tips.Add (new PatternTip ("vu", "ぁ"));
			break;
		case "ヴぃ":
		case "ゔぃ":
			tips.Add (new PatternTip ("vi"));
			tips.Add (new PatternTip ("vu", "ぃ"));
			break;
		case "ヴぇ":
		case "ゔぇ":
			tips.Add (new PatternTip ("ve"));
			tips.Add (new PatternTip ("vu", "ぇ"));
			break;
		case "ヴぉ":
		case "ゔぉ":
			tips.Add (new PatternTip ("vo"));
			tips.Add (new PatternTip ("vu", "ぉ"));
			break;
		case "ー":
			tips.Add(new PatternTip("-"));
			break;
		case "！":
			tips.Add (new PatternTip ("!"));
			break;
		case "一":
			tips.Add (new PatternTip ("ichi"));
			tips.Add (new PatternTip ("iti"));
			tips.Add (new PatternTip ("hito"));
			break;
		case "二":
			tips.Add (new PatternTip ("huta"));
			tips.Add (new PatternTip ("futa"));
			tips.Add (new PatternTip ("ni"));
			break;
		case "三":
			tips.Add (new PatternTip ("sann"));
			tips.Add (new PatternTip ("san"));
			break;
		case "四":
			tips.Add (new PatternTip ("yo"));
			break;
		case "五":
			tips.Add (new PatternTip ("go"));
			break;

		case "ア":
			tips.Add (new PatternTip ("a"));
			break;
		case "イ":
			tips.Add (new PatternTip ("i"));
			break;
		case "オ":
			tips.Add (new PatternTip ("o"));
			break;
		case "ギ":
			tips.Add (new PatternTip ("gi"));
			break;
		case "キ":
			tips.Add (new PatternTip ("ki"));
			break;
		case "グ":
			tips.Add (new PatternTip ("gu"));
			break;
		case "ク":
			tips.Add (new PatternTip ("ku"));
			break;
		case "ケ":
			tips.Add (new PatternTip ("ke"));
			break;
		case "ゲ":
			tips.Add (new PatternTip ("ge"));
			break;
		case "ゴ":
			tips.Add (new PatternTip ("go"));
			break;
		case "コ":
			tips.Add (new PatternTip ("ko"));
			break;
		case "サ":
			tips.Add (new PatternTip ("sa"));
			break;
		case "タ":
			tips.Add (new PatternTip ("ta"));
			break;
		case "ダ":
			tips.Add (new PatternTip ("da"));
			break;
		case "ヅ":
			tips.Add (new PatternTip ("du"));
			break;
		case "デ":
			tips.Add (new PatternTip ("de"));
			break;
		case "ト":
			tips.Add (new PatternTip ("to"));
			break;
		case "ニュ":
			tips.Add (new PatternTip ("nyu"));
			tips.Add (new PatternTip ("nilyu"));
			break;
		case "ノ":
			tips.Add (new PatternTip ("no"));
			break;
		case "バ":
			tips.Add (new PatternTip ("ba"));
			break;
		case "パ":
			tips.Add (new PatternTip ("pa"));
			break;
		case "ヒ":
			tips.Add (new PatternTip ("hi"));
			break;
		case "フ":
			tips.Add (new PatternTip ("hu"));
			tips.Add (new PatternTip ("fu"));
			break;
		case "ブ":
			tips.Add (new PatternTip ("bu"));
			break;
		case "ベ":
			tips.Add (new PatternTip ("be"));
			break;
		case "ポ":
			tips.Add (new PatternTip ("po"));
			break;
		case "マ":
			tips.Add (new PatternTip ("ma"));
			break;
		case "ミ":
			tips.Add (new PatternTip ("mi"));
			break;
		case "ム":
			tips.Add (new PatternTip ("mu"));
			break;
		case "メ":
			tips.Add (new PatternTip ("me"));
			break;
		case "ラ":
			tips.Add (new PatternTip ("ra"));
			break;
		case "リ":
			tips.Add (new PatternTip ("ri"));
			break;
		case "ル":
			tips.Add (new PatternTip ("ru"));
			break;
		case "レ":
			tips.Add (new PatternTip ("re"));
			break;
		case "ロ":
			tips.Add (new PatternTip ("ro"));
			break;
		case "ン":
			tips.Add (new PatternTip ("nn"));
			break;
		
		
		
		//　　　　　　　　漢字

		//　　　　　　　　あ行
		
		case "会":
			tips.Add (new PatternTip ("a"));
			break;
		case "諦":
			tips.Add (new PatternTip ("akira"));
			break;
		case "朝":
			tips.Add (new PatternTip ("asa"));
			break;
		case "雨":
			tips.Add (new PatternTip ("ame"));
			break;
		case "生":
			tips.Add (new PatternTip ("sei"));
			tips.Add (new PatternTip ("i"));
			break;
		case "犬":
			tips.Add (new PatternTip ("inu"));
			break;
		case "意":
			tips.Add (new PatternTip ("i"));
			break;
		case "運":
			tips.Add (new PatternTip ("unn"));
			tips.Add (new PatternTip ("un"));
			break;
		case "動":
			tips.Add (new PatternTip ("u"));
			break;
		case "恵":
			tips.Add (new PatternTip ("e"));
			break;
		case "笑":
			tips.Add (new PatternTip ("e"));
			break;
		case "猿":
			tips.Add (new PatternTip ("ete"));
			break;
		case "重":
			tips.Add (new PatternTip ("e"));
			break;
		case "同":
			tips.Add (new PatternTip ("ona"));
			break;
		case "置":
			tips.Add (new PatternTip ("o"));
			break;
		case "鬼":
			tips.Add (new PatternTip ("oni"));
			break;
		case "温":
			tips.Add (new PatternTip ("onn"));
			tips.Add (new PatternTip ("on"));
			break;
		case "親":
			tips.Add (new PatternTip ("oya"));
			break;
		case "男":
			tips.Add (new PatternTip ("otoko"));
			break;
		case "女":
			tips.Add (new PatternTip ("onna"));
			tips.Add (new PatternTip ("onnna"));
			tips.Add (new PatternTip ("me"));
			break;
		case "掟":
			tips.Add (new PatternTip ("okite"));
			break;
	   
		
		

		//　　　　　　　　　　か行
		
		case "母":
			tips.Add (new PatternTip ("kaa"));
			break;
		case "顔":
			tips.Add (new PatternTip ("gao"));
			break;
		case "傘":
			tips.Add (new PatternTip ("kasa"));
			break;
		case "嘩":
			tips.Add (new PatternTip ("ka"));
			break;
		case "賀":
			tips.Add (new PatternTip ("ga"));
			break;
		case "刀":
			tips.Add (new PatternTip ("katana"));
			break;
		case "固":
			tips.Add (new PatternTip ("kata"));
			break;
		case "紙":
			tips.Add (new PatternTip ("kami"));
			break;
		case "関":
			tips.Add (new PatternTip ("kann"));
			tips.Add (new PatternTip ("kan"));
			break;
		case "間":
			tips.Add (new PatternTip ("kann"));
			tips.Add (new PatternTip ("kan"));
			tips.Add (new PatternTip ("kenn"));
			tips.Add (new PatternTip ("ken"));
			break;
		case "狩":
			tips.Add (new PatternTip ("ga"));
			break;
		case "切":
			tips.Add (new PatternTip ("ki"));
			tips.Add (new PatternTip ("setu"));
			break;
		case "気":
			tips.Add (new PatternTip ("ki"));
			break;
		case "木":
			tips.Add (new PatternTip ("ki"));
			break;
		case "銀":
			tips.Add (new PatternTip ("ginn"));
			tips.Add (new PatternTip ("gin"));
			break;
		case "組":
			tips.Add (new PatternTip ("gumi"));
			break;
		case "係":
			tips.Add (new PatternTip ("kei"));
			break;
		case "消":
			tips.Add (new PatternTip ("ke"));
			break;
		case "喧":
			tips.Add (new PatternTip ("kenn"));
			tips.Add (new PatternTip ("ken"));
			break;
		case "化":
			tips.Add (new PatternTip ("ke"));
			break;
		case "子":
			tips.Add (new PatternTip ("ko"));
			tips.Add (new PatternTip ("go"));
			break;
		case "事":
			tips.Add (new PatternTip ("goto"));
			break;
		case "獄":
			tips.Add (new PatternTip ("goku"));
			break;
		case "後":
			tips.Add (new PatternTip ("go"));
			break;
		case "公":
			tips.Add (new PatternTip ("kou"));
			break;
		case "恋":
			tips.Add (new PatternTip ("koi"));
			break;
		case "心":
			tips.Add (new PatternTip ("kokoro"));
			break;

		
		

		//　　　　　　　　　　さ行
		
		case "最":
			tips.Add (new PatternTip ("sai"));
			break;
		case "里":
			tips.Add (new PatternTip ("sato"));
			break;
		case "桜":
			tips.Add (new PatternTip ("sakura"));
			break;
		case "忍":
			tips.Add (new PatternTip ("shinobi"));
			tips.Add (new PatternTip ("sinobi"));
			break;
		case "死":
			tips.Add (new PatternTip ("shi"));
			break;	
		case "者":
			tips.Add (new PatternTip ("ssha"));
			break;
		case "真":
			tips.Add (new PatternTip ("shinn"));
			tips.Add (new PatternTip ("shin"));
			break;
		case "粧":
			tips.Add (new PatternTip ("shou"));
			break;
		case "所":
			tips.Add (new PatternTip ("sho"));
			break;
		case "縮":
			tips.Add (new PatternTip ("shuku"));
			tips.Add (new PatternTip ("syuku"));
			break;
		case "時":
			tips.Add (new PatternTip ("ji"));
			break;
		case "状":
			tips.Add (new PatternTip ("jou"));
			break;
		case "好":
			tips.Add (new PatternTip ("zu"));
			break;
		case "図":
			tips.Add (new PatternTip ("zu"));
			break;
		case "世":
			tips.Add (new PatternTip ("se"));
			break;
		case "拙":
			tips.Add (new PatternTip ("se"));
			break;
		case "選":
			tips.Add (new PatternTip ("senn"));
			tips.Add (new PatternTip ("sen"));
			break;
		case "揃":
			tips.Add (new PatternTip ("soro"));
			break;

		

		

		//　　　　　　　　　　た行
		
		case "食":
			tips.Add (new PatternTip ("ta"));
			break;
		case "短":
			tips.Add (new PatternTip ("tann"));
			tips.Add (new PatternTip ("tan"));
			break;
		case "卵":
			tips.Add (new PatternTip ("tamago"));
			break;
		case "体":
			tips.Add (new PatternTip ("tai"));
			break;
		case "団":
			tips.Add (new PatternTip ("dann"));
			tips.Add (new PatternTip ("dan"));
			break;
		case "大":
			tips.Add (new PatternTip ("dai"));
			tips.Add (new PatternTip ("tai"));
			break;
		case "魂":
			tips.Add (new PatternTip ("tama"));
			break;
		case "脱":
			tips.Add (new PatternTip ("datu"));
			break;
		case "丁":
			tips.Add (new PatternTip ("chou"));
			break;
		case "長":
			tips.Add (new PatternTip ("chou"));
			break;
		case "知":
			tips.Add (new PatternTip ("shi"));
			tips.Add (new PatternTip ("chi"));
			break;
		case "注":
			tips.Add (new PatternTip ("chuu"));
			tips.Add (new PatternTip ("chu"));
			tips.Add (new PatternTip ("tyu"));
			break;
		case "潰":
			tips.Add (new PatternTip ("tubu"));
			break;
		case "辛":
			tips.Add (new PatternTip ("tura"));
			break;
		case "強":
			tips.Add (new PatternTip ("zuyo"));
			break;
		case "集":
			tips.Add (new  PatternTip ("tudo"));
			break;
		case "連":
			tips.Add (new PatternTip ("tu"));
			break;
		case "転":
			tips.Add (new PatternTip ("ten"));
			tips.Add (new PatternTip ("tenn"));
			break;
		case "天":
			tips.Add (new PatternTip ("tenn"));
			tips.Add (new PatternTip ("ten"));
			break;
		case "度":
			tips.Add (new PatternTip ("do"));
			break;
		case "供":
			tips.Add (new PatternTip ("domo"));
			break;
		case "届":
			tips.Add (new PatternTip ("todo"));
			break;
		
		

		//　　　　　　　　　　な行
		
		case "悩":
			tips.Add (new PatternTip ("naya"));
			break;
		case "鍋":
			tips.Add (new PatternTip ("nabe"));
			break;
		case "夏":
			tips.Add (new PatternTip ("natu"));
			break;
		case "何":
			tips.Add (new PatternTip ("nann"));
			tips.Add (new PatternTip ("nan"));
			tips.Add (new PatternTip ("nani"));
			break;
		case "日":
			tips.Add (new PatternTip ("nichi"));
			break;
		case "似":
			tips.Add (new PatternTip ("ni"));
			break;
		case "神":
			tips.Add (new PatternTip ("nigami"));
			break;
		case "粘":
			tips.Add (new PatternTip ("neba"));
			break;
		case "年":
			tips.Add (new PatternTip ("nenn"));
			tips.Add (new PatternTip ("nen"));
			break;
		case "然":
			tips.Add (new PatternTip ("nenn"));
			tips.Add (new PatternTip ("nen"));
			break;
		case "信":
			tips.Add (new PatternTip ("nobu"));
			break;
		case "野":
			tips.Add (new PatternTip ("no"));
			break;
		case "上":
			tips.Add (new PatternTip ("nobo"));
			break;

		

		
		//　　　　　　　　　　は行
		
		case "半":
			tips.Add (new PatternTip ("han"));
			tips.Add (new PatternTip ("hann"));
			break;
		case "話":
			tips.Add (new PatternTip ("hanashi"));
			tips.Add (new PatternTip ("hanasi"));
			break;
		case "番":
			tips.Add (new PatternTip ("bann"));
			tips.Add (new PatternTip ("ban"));
			break;
		case "花":
			tips.Add (new PatternTip ("hana"));
			break;
		case "肌":
			tips.Add (new PatternTip ("hada"));
			break;
		case "陽":
			tips.Add (new PatternTip ("hi"));
			break;
		case "人":
			tips.Add (new PatternTip ("hito"));
			tips.Add (new PatternTip ("jinn"));
			tips.Add (new PatternTip ("jin"));
			tips.Add (new PatternTip ("ri"));
			tips.Add (new PatternTip ("ninn"));
			tips.Add (new PatternTip ("nin"));
			break;
		case "光":
			tips.Add (new PatternTip ("hikari"));
			break;
		case "分":
			tips.Add (new PatternTip ("bunn"));
			tips.Add (new PatternTip ("bun"));
			break;
		case "筆":
			tips.Add (new PatternTip ("hude"));
			tips.Add (new PatternTip ("fude"));
			break;

		

		//　　　　　　　　　ま行
		
		case "身":
		tips.Add (new PatternTip ("mi"));
			break;
		case "見":
			tips.Add (new PatternTip ("mi"));
			break;
		case "虫":
			tips.Add (new PatternTip ("mushi"));
			tips.Add (new PatternTip ("musi"));
			break;
		case "物":
			tips.Add (new PatternTip ("mono"));
			break;
	
		
		

	    //　　　　　　　　　　　や行
		
		case "痩":
			tips.Add (new PatternTip ("ya"));
			break;
		case "奴":
			tips.Add (new PatternTip ("yatu"));
			break;
		case "闇":
			tips.Add (new PatternTip ("yami"));
			break;
		case "破":
			tips.Add (new PatternTip ("yabu"));
			break;
		case "雪":
			tips.Add (new PatternTip ("yuki"));
			break;
		case "夜":
			tips.Add (new PatternTip ("yoru"));
			tips.Add (new PatternTip ("yo"));
			break;

		
		
		//　　　　　　　　　　　ら行
		
		case "良":
			tips.Add (new PatternTip ("ra"));
			break;
		case "類":
			tips.Add (new PatternTip ("rui"));
			break;


		
		
		//　　　　　　　　　　　わ行
		
		case "渡":
			tips.Add (new PatternTip ("wata"));
			break;
		case "忘":
			tips.Add (new PatternTip ("wasu"));
			break;
		case "悪":
			tips.Add (new PatternTip ("waru"));
			break;
	
		
		
		
		

		default:
			if (s [0] == 'っ') {
				List<PatternTip> nextTips = new List<PatternTip> ();
				GetPatternTips (nextTips, s.Substring (1));
				foreach (PatternTip nextTip in nextTips) {
					tips.Add (new PatternTip (nextTip.alphabet [0] + nextTip.alphabet, nextTip.remainder));
				}
				tips.Add (new PatternTip ("xtu", s.Substring (1)));
				tips.Add (new PatternTip ("ltu", s.Substring (1)));
				tips.Add (new PatternTip ("xtsu", s.Substring (1)));
				tips.Add (new PatternTip ("ltsu", s.Substring (1)));
			} else if (s [0] == 'ん') {
				tips.Add (new PatternTip ("nn", s.Substring (1)));
			} else {
				tips.Add (new PatternTip (s));
			}
			break;
		}
	}

	string StringToKatakana(string s){
		return new string(s.Select(c => (c >= 'ぁ' && c <= 'ゖ') ? (char)(c + 'ァ' - 'ぁ') : c).ToArray());
	}
	string StringToHiragana(string s){
		return new string(s.Select(c => (c >= 'ァ' && c <= 'ヶ') ? (char)(c + 'ぁ' - 'ァ') : c).ToArray());
	}
}

public class PatternTip
{
	// アルファベットパターン
	public string alphabet;
	// 残り文字
	public string remainder;

	//ポップ待機かどうか
	public bool isPoped;

	public PatternTip(string alphabet, string remainder = ""){
		this.alphabet = alphabet;
		this.remainder = remainder;
	}
	public void Pop(){
		alphabet = alphabet.Substring (1);
		isPoped = true;
	}
}

