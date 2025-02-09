using UnityEngine;
using System.Collections;




/*
 *	プレーヤークラス 基底
 *	Maruchu
 *
 *	キャラクターの移動、メカニム(モーション)の制御など
 */
public		class		Player_Base				: HitObject {




//プレーヤーの操作の種類
protected	enum	PlayerInput {
	 Move_Left		//移動 左
	,Move_Up		//移動 上
	,Move_Right		//移動 右
	,Move_Down		//移動 下
	,Shoot			//射撃
	,EnumMax		//最大数
}

private		static readonly		float		MOVE_ROTATION_Y_LEFT	= -90f;		//移動方向 左
private		static readonly		float		MOVE_ROTATION_Y_UP		=   0f;		//移動方向 上
private		static readonly		float		MOVE_ROTATION_Y_RIGHT	=  90f;		//移動方向 右
private		static readonly		float		MOVE_ROTATION_Y_DOWN	= 180f;		//移動方向 下

public							float		MOVE_SPEED				= 5.0f;		//移動の速度





public							GameObject	playerObject			= null;		//動かす対象のモデル
public							GameObject	bulletObject			= null;		//弾プレハブ


public							GameObject	hitEffectPrefab			= null;		//ヒットエフェクトのプレハブ





private							float		m_rotationY				= 0.0f;		//プレーヤーの回転角度

protected						bool[]		m_playerInput			= new bool[ (int)PlayerInput.EnumMax];		//押されている操作

protected						bool		m_playerDeadFlag		= false;		//プレーヤーが死んだフラグ




/*
 *	毎フレーム呼び出される関数
 */
private		void	Update() {

	//プレーヤーが死んでいる
	if( m_playerDeadFlag) {
		//すべての処理を無視する
		return;
	}

	//フラグ初期化
	ClearInput();
	//入力処理取得
	GetInput();

	//移動処理
	CheckMove();
}


/*
 *	入力処理のチェック
 */
private			void	ClearInput() {
	//フラグ初期化
	int	i;
	for( i=0; i<(int)PlayerInput.EnumMax; i++) {
		m_playerInput[ i]	= false;
	}
}
/*
 *	入力処理のチェック
 */
protected	virtual	void	GetInput() {
}


/*
 *	移動処理のチェック
 */
private			void	CheckMove() {

	//アニメーター(メカニム)を取得
	Animator	animator	= playerObject.GetComponent<Animator>();

	//弾に当たっていなければ移動OK
	float	moveSpeed	= MOVE_SPEED;		//移動速度
	bool	shootFlag	= false;			//弾を撃つフラグ

	//移動と回転
	{
		//キー操作による回転と移動
		if( m_playerInput[ (int)PlayerInput.Move_Left]) {
			//左
			m_rotationY		= MOVE_ROTATION_Y_LEFT;
		} else
		if( m_playerInput[ (int)PlayerInput.Move_Up]) {
			//上
			m_rotationY		= MOVE_ROTATION_Y_UP;
		} else
		if( m_playerInput[ (int)PlayerInput.Move_Right]) {
			//右
			m_rotationY		= MOVE_ROTATION_Y_RIGHT;
		} else
		if( m_playerInput[ (int)PlayerInput.Move_Down]) {
			//下
			m_rotationY		= MOVE_ROTATION_Y_DOWN;
		} else {
			//何も押してなければ移動しない
			moveSpeed		= 0f;
		}

		//向いている方向をオイラー角で入れる
		transform.rotation	= Quaternion.Euler( 0, m_rotationY, 0);		//Y軸回転でキャラの向きを横に動かせる

		//移動量を Transform に渡して移動させる
		transform.position	+= ((transform.rotation	 	*(Vector3.forward	*moveSpeed))		*Time.deltaTime);
	}

	//射撃
	{
		//射撃ボタン(クリック)押しているかどうか判定
		if( m_playerInput[ (int)PlayerInput.Shoot]) {
			//撃った
			shootFlag	= true;

			//弾を生成する位置
			Vector3 vecBulletPos	= transform.position;
			//進行方向にちょっと前へ
			vecBulletPos			+= (transform.rotation	*Vector3.forward);
			//Yは高さを適当に上げる
			vecBulletPos.y			= 2.0f;

			//弾を生成
			Instantiate( bulletObject, vecBulletPos, transform.rotation);
		} else {
			//撃ってない
			shootFlag	= false;
		}
	}


	//メカニム
	{
		//Animatorで設定した値を渡す
		animator.SetFloat(	"Speed",	moveSpeed);		//移動量
		animator.SetBool(	"Shoot",	shootFlag);		//射撃フラグ
	}
}




/*
 *	Collider が何かにヒットしたら呼ばれる関数
 *
 *	自分の GameObject に Collider(IsTriggerをつける) と Rigidbody をつけると呼ばれるようになる
 */
private		void	OnTriggerEnter( Collider hitCollider) {

	//ヒットしてよいか確認
	if( false==IsHitOK( hitCollider.gameObject)) {
		//このオブジェクトにはあたってはいけない
		return;
	}

	//弾に当たった
	{
		//アニメーター(メカニム)を取得
		Animator	animator	= playerObject.GetComponent<Animator>();

		//メカニムに死んだことを通知
		animator.SetBool(	"Dead",		true);		//死んだフラグ
	}

	//ヒットエフェクトがあるかどうか判定
	if( null!=hitEffectPrefab) {
		//自分と同じ位置でヒットエフェクトを出す
		Instantiate( hitEffectPrefab, transform.position, transform.rotation);
	}

	//このプレーヤーは死んだ状態にする
	m_playerDeadFlag	= true;
}




}
