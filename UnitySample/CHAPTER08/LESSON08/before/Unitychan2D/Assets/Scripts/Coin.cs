using UnityEngine;
using System.Collections;
//AudioSourceを必要とする
//これを記述しておくとこのコンポーネントを追加した際に
//一緒にAudioSorceコンポーネントも追加される
[RequireComponent(typeof(AudioSource))]

/*
 * Coinクラス
 * 
 * Playerに当たった時に音を鳴らす
 * 
 */      
public class Coin : MonoBehaviour {
	
	
	private AudioSource mAudio;
	private Renderer mRenderer;
	private Collider2D mCollider2D;
	
	/*
	 * はじめに呼ばれる関数
	 */
	void Start () {
		//必要なコンポーネントを取得する
		mAudio = GetComponent<AudioSource> ();
		mRenderer = GetComponent<Renderer> ();
		mCollider2D = GetComponent<Collider2D>();
	}
	
	void OnTriggerEnter2D(Collider2D other){
		//もし 接触したオブジェクトのタグが"Player"なら
		if (other.tag == "Player") {
			//スコアに1を足す
			Score.instance.Add ();
			
			//描画を消す
			mRenderer.enabled = false;
			//当たりを消す
			mCollider2D.enabled = false;
			
			//音を再生する
			mAudio.Play();
			//音が流れ終わると消える
			Destroy(gameObject,mAudio.clip.length);
		}
	}
	
}
