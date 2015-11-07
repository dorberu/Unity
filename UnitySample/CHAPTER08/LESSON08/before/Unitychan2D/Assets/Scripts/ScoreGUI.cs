using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScoreGUI : MonoBehaviour {
	
	private Text mText;
	
	// Use this for initialization
	void Start () {
		mText = GetComponent<Text> ();
	}
	
	// Update is called once per frame
	void Update () {
		//Scoreクラスからscoreを得る
		int score = Score.instance.score;
		//3桁になるように0を足す
		string scoreAddZero = score.ToString ("000");
		//テキストをGUIで表示する
		mText.text = "Score:" + scoreAddZero;
	}
}