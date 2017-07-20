using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TimeBar : MonoBehaviour {

	public TimeScript t;
	Slider _slider;

	// Use this for initialization
	void Start () {
	
		_slider = GameObject.Find("Slider").GetComponent<Slider>();
	}
	//float _hp = 60;
	
	// Update is called once per frame
	void Update () {

		//_hp -= 0.017f;
		if(t.countTime < _slider.minValue) {
			// 最大を超えたら0に戻す
			t.countTime = _slider.maxValue;
		}

		// HPゲージに値を設定
		_slider.value = t.countTime;
	
	}
}
