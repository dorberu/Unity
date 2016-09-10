using UnityEngine;
using System.Collections;

//AudioSourceを必要とする
//これを記述しておくとこのコンポーネントを追加した際に
//一緒にAudioSorceコンポーネントも追加される
[RequireComponent(typeof(AudioSource))]

/*
 * Playerクラス
 * 
 * UnityChan2DController以外のことを制御する
 * 
 */
public class PlayerController : MonoBehaviour {
	
	
	public AudioClip jumpVoice;  //ジャンプした時の声
	public AudioClip damageVoice; //ダメージを受けた時雄の声
	
	private AudioSource mAudio;  //AudioSource
	
	/*
    * はじめに呼ばれる関数
    */
	void Start () {
		mAudio = GetComponent<AudioSource> ();
	}
	
	/*
     * ダメージを受けた時に呼ばれる関数
     */
	void OnDamage(){
		//声を出す
		PlayerVoice (damageVoice);
	}
	
	void Jump(){
		//声を出す
		PlayerVoice (jumpVoice);
	}
	
	void PlayerVoice(AudioClip clip){
		//音を消す
		mAudio.Stop ();
		//音を再生する
		mAudio.PlayOneShot (clip);
	}
}