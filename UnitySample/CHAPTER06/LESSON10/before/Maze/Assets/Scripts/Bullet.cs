using UnityEngine;
using System.Collections;


/*
 *	弾クラス
 *	Maruchu
 *
 *	何でもいいので接触したらエフェクトを出して消える
 */
public		class		Bullet				: MonoBehaviour {
	
	
	
	private		static readonly		float		bulletMoveSpeed			= 10.0f;					//1秒間に弾が進む距離
	
	
	public							GameObject	hitEffectPrefab			= null;						//ヒットエフェクトのプレハブ
	
	
	
	
	/*
	 *	毎フレーム呼び出される関数
	 */
	private		void	Update() {
		
		//移動
		{
			//1秒間の移動量
			Vector3		vecAddPos	= (Vector3.forward		*bulletMoveSpeed);
			/*
				Vector3.forward は new Vector3( 0f, 0f, 1f) と同じです

				ほかにもいろいろあるので下記のページを参照してほしい
				http://docs.unity3d.com/ScriptReference/Vector3.html

				そして Vector3 に transform.rotation を掛けると
				その方向へ曲げてくれる。この時、Vector3はZ+ の方向を、
				正面として考える
			 */
			
			//移動量、回転量には Time.deltaTime をかけて実行環境(フレーム数の差)による違いが出ないようにします
			transform.position	+= ((transform.rotation	 	*vecAddPos)		*Time.deltaTime);
		}
	}
	
	
	
	/*
	 *	Colliderが何かにヒットしたら呼ばれる関数
	 *
	 *	自分のGameObjectにCollider(IsTriggerを付ける) とRigidbodyを付けると呼ばれるようになる
	 */
	private		void	OnTriggerEnter( Collider hitCollider) {
		
		//ヒットエフェクトあるか確認する
		if( null!=hitEffectPrefab) {
			//自分と同じ位置でヒットエフェクトを出す
			Instantiate( hitEffectPrefab, transform.position, transform.rotation);
		}
		
		//このGameObjectを［Hierrchy］ビューから削除する
		Destroy( gameObject);
	}
	
	
	
	
}
