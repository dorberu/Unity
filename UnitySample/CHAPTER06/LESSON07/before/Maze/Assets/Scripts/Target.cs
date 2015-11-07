using UnityEngine;
using System.Collections;


/*
 *	ターゲットクラス
 *	Maruchu
 *
 *	Bulletクラスを持った GameObject が接触すると破壊される
 *	すべてのターゲットが破壊されるとステージクリアとする
 */
public		class		Target				: MonoBehaviour {
	
	
	
	
	public							GameObject	hitEffectPrefab			= null;						//ヒットエフェクトのプレハブ
	
	private	static					int			m_allTargetNum			= 0;						//ステージに配置されているターゲットの数
	
	
	
	/*
	 *	起動時に呼び出される関数
	 */
	private		void	Awake() {
		//ターゲットの総数を覚える
		m_allTargetNum++;
	}
	
	
	
	
	/*
	 *	Colliderが何かにヒットしたら呼ばれる関数
	 *
	 *	自分のGameObjectにCollider(IsTriggerを付ける) とRigidbodyを付けると呼ばれるようになる
	 */
	private		void	OnTriggerEnter( Collider hitCollider) {
		
		//相手のGameObjectを取得
		GameObject	hitObject	= hitCollider.gameObject;
		
		//相手は弾なのか確認する
		if( null==hitObject.GetComponent<Bullet>()) {
			//弾ではなかったので無視
			return;
		}
		
		
		//破壊されたオブジェクトの処理をする
		
		//ヒットエフェクトがあるか確認する
		if( null!=hitEffectPrefab) {
			//自分と同じ位置でヒットエフェクトを出す
			Instantiate( hitEffectPrefab, transform.position, transform.rotation);
		}
		
		//ステージクリアのチェック
		{
			//ターゲットの総数から自分の分を削除する
			m_allTargetNum--;
			
			//もしターゲットの数が 0 になったらステージをクリアする
			if( m_allTargetNum <= 0) {
				
				//ステージクリアにする
				Game.SetStageClear();
			}
		}
		
		//このGameObjectを［Hierarchy］ビューから削除する
		Destroy( gameObject);
	}
	
	
	
	
}
