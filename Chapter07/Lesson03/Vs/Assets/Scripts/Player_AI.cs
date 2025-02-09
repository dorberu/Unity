using UnityEngine;
using System.Collections;




/*
 *	プレーヤークラス（AIが操作）
 *	Maruchu
 */
public		class		Player_AI				: Player_Base {



//チェック方向
private	enum	CheckDir {
	//［←］ ［↑］ ［→］ ［↓］キー の順番
	 Left		//左
	,Up			//上
	,Right		//右
	,Down		//下
	,EnumMax	//最大数
}
//チェック情報
private	enum	CheckData {
	 X			//X軸
	,Y			//Y軸
	,EnumMax	//最大数
}

private		static readonly		int[][]		CHECK_DIR_LIST			= new int[ (int)CheckDir.EnumMax][] {		//チェック方向
	//										 X		 Y
	 new int[ (int)CheckData.EnumMax] {		-1,		 0		}
	,new int[ (int)CheckData.EnumMax] {		 0,		 1		}
	,new int[ (int)CheckData.EnumMax] {		 1,		 0		}
	,new int[ (int)CheckData.EnumMax] {		 0,		-1		}
};

private		static	readonly	int			AI_PRIO_MIN				= 99;						//AI優先度で一番低い値



private		static	readonly	float		AI_INTERVAL_MIN			= 0.5f;						//AI思考の間隔 最短
private		static	readonly	float		AI_INTERVAL_MAX			= 0.8f;						//AI思考の間隔 最長

private		static	readonly	float		AI_IGNORE_DISTANCE		= 2.0f;						//プレーヤーにこれ以上近づかない

private		static	readonly	float		SHOOT_INTERVAL			= 1.0f;						//射撃の間隔



private							float		m_aiInterval			= 0f;						//AIの思考を更新するまでの時間
private							float		m_shootInterval			= 0f;						//射撃の間隔


private							PlayerInput	m_pressInput			= PlayerInput.Move_Left;	//AIが行う入力の種類





/*
 *	入力処理のチェック
 */
protected	override	void	GetInput() {

	//ユーザーが動かしているプレーヤーのオブジェクトを取得
	GameObject	mainObject		= Player_Key.m_mainPlayer;
	if( null==mainObject) {
		//プレーヤーがいなかったら思考を中断
		return;
	}



	//AIの思考を更新するまでの時間
	m_aiInterval	-= Time.deltaTime;

	//射撃の思考を更新するまでの時間
	m_shootInterval	-= Time.deltaTime;



	//プレーヤーと自分の距離を算出
	Vector3		aiSubPosition	= (transform.position	-mainObject.transform.position);
	aiSubPosition.y		= 0f;


	//距離が離れていたら動く
	if( aiSubPosition.magnitude > AI_IGNORE_DISTANCE) {

		//一定時間ごとにAIを更新
		if( m_aiInterval < 0f) {

			//次の思考までこの時間待つ
			m_aiInterval		= Random.Range( AI_INTERVAL_MIN, AI_INTERVAL_MAX);		//ランダムに時間を決定


			//AIがいる位置から上下左右の優先度を取得する
			int[]	prioTable	= GetMovePrioTable();

			//最も優先度の高い場所の数字を算出する
			int		highest		= AI_PRIO_MIN;
			int		i;
			for( i=0; i<(int)CheckDir.EnumMax; i++) {
				//数値が低いほうが優先度が高い
				if( highest > prioTable[ i]) {
					//優先度更新
					highest		= prioTable[ i];
				}
			}

			//どの方向の優先度が高い？
			//この入力をする
			PlayerInput	pressInput	= PlayerInput.Move_Left;
			if( highest==prioTable[ (int)CheckDir.Left]) {
				//左に移動
				pressInput	= PlayerInput.Move_Left;
			} else
			if( highest==prioTable[ (int)CheckDir.Right]) {
				//右に移動
				pressInput	= PlayerInput.Move_Right;
			} else
			if( highest==prioTable[ (int)CheckDir.Up]) {
				//上に移動
				pressInput	= PlayerInput.Move_Up;
			} else
			if( highest==prioTable[ (int)CheckDir.Down]) {
				//下に移動
				pressInput	= PlayerInput.Move_Down;
			}
			m_pressInput	= pressInput;
		}

		//入力
		m_playerInput[ (int)m_pressInput]	= true;
	}


	//射撃の思考をするか判定
	if( m_shootInterval < 0f) {

		//X、またはZの距離が近い場合、直線上にいると判断して射撃を行う
		if( (Mathf.Abs( aiSubPosition.x) < 1f) || (Mathf.Abs( aiSubPosition.z) < 1f)) {
			//射撃操作
			m_playerInput[ (int)PlayerInput.Shoot]	= true;

			//次の射撃はこの時間が経過するまで待つ(連射の抑制)
			m_shootInterval	= SHOOT_INTERVAL;
		}
	}
}








/*
 *	位置からグリッドへ変換 グリッドX
 */
private		int		GetGridX( float posX) {
	//グリッドの範囲内に収まるように Mathf.Clamp で制限をかける
	return	Mathf.Clamp((int)((posX) /Field.BLOCK_SCALE),0,(Field.FIELD_GRID_X -1));
}
/*
 *	位置からグリッドへ変換 グリッドY
 */
private		int		GetGridY( float posZ) {
	//UnityではXZ平面が地平線
	return	Mathf.Clamp((int)((posZ) /Field.BLOCK_SCALE),0,(Field.FIELD_GRID_Y -1));
}



/*
 *	AIが移動するときの優先度の算出
 */
private		int[]	GetMovePrioTable() {

	int	i, j;

	//自分自身(AI)の位置
	Vector3		aiPosition	= transform.position;
	//グリッドに変換
	int			aiX			= GetGridX( aiPosition.x);
	int			aiY			= GetGridY( aiPosition.z);

	//ユーザーが動かしているプレーヤーのオブジェクトを取得
	GameObject	mainObject		= Player_Key.m_mainPlayer;
	//攻撃目標の位置を取得
	Vector3		playerPosition	= mainObject.transform.position;
	//グリッドに変換
	int			playerX			= GetGridX( playerPosition.x);
	int			playerY			= GetGridY( playerPosition.z);
	int			playerGrid		= playerX	+(playerY *Field.FIELD_GRID_X);


	//グリッドの各位置の優先度を格納する配列
	int[]		calcGrid		= new int[ (Field.FIELD_GRID_X *Field.FIELD_GRID_Y)];
	//初期化
	for( i=0; i<(Field.FIELD_GRID_X *Field.FIELD_GRID_Y); i++) {
		//優先度を最低にする
		calcGrid[ i]	= AI_PRIO_MIN;
	}



	//プレーヤーが現在いる場所にまず 1 を入れる
	calcGrid[ playerGrid]	= 1;


	//チェックする優先度はまず 1 から
	int			checkPrio		= 1;
	//チェック用変数
	int			checkX;
	int			checkY;
	int			tempX;
	int			tempY;
	int			tempGrid;
	//何かチェックしたら true
	bool		update;
	do {
		//初期化
		update	= false;

		//チェック開始
		for( i=0; i<(Field.FIELD_GRID_X *Field.FIELD_GRID_Y); i++) {
			//チェックする優先度でないなら無視
			if( checkPrio!=calcGrid[ i]) {
				continue;
			}

			//このグリッドがチェックする優先度の場所
			checkX	= (i %Field.FIELD_GRID_X);
			checkY	= (i /Field.FIELD_GRID_X);

			//そこから上下左右の場所をチェック
			for( j=0; j<(int)CheckDir.EnumMax; j++) {
				//調べる場所の隣
				tempX	= (checkX +CHECK_DIR_LIST[ j][ (int)CheckData.X]);
				tempY	= (checkY +CHECK_DIR_LIST[ j][ (int)CheckData.Y]);
				//グリッドの外？
				if( (tempX < 0) || (tempX >= Field.FIELD_GRID_X) || (tempY < 0) || (tempY >= Field.FIELD_GRID_Y)) {
					//場外なので無視
					continue;
				}
				//ここを調べる
				tempGrid	= (tempX +(tempY *Field.FIELD_GRID_X));

				//隣が壁かチェック
				if( Field.ObjectKind.Block==(Field.ObjectKind)Field.GRID_OBJECT_DATA[ tempGrid]) {
					//壁なら無視
					continue;
				}

				//この場所の優先度の数字が現在チェックしている優先度より大きければ更新
				if( calcGrid[ tempGrid] > (checkPrio +1)) {
					//値を更新
					calcGrid[ tempGrid] = (checkPrio +1);	//この数字が次にチェックするときの優先度
					//フラグを立てる
					update	= true;
				}
			}
		}

		//チェックする優先度を +1 する
		checkPrio++;

		//何か更新があればもう1回だけまわす
	} while( update);



	//AIの周辺の優先度テーブル
	int[]		prioTable		= new int[ (int)CheckDir.EnumMax];

	//優先度テーブルが作成できたらAIの周辺の優先度を取得
	for( i=0; i<(int)CheckDir.EnumMax; i++) {

		//調べる場所の隣
		tempX	= (aiX +CHECK_DIR_LIST[ i][ (int)CheckData.X]);
		tempY	= (aiY +CHECK_DIR_LIST[ i][ (int)CheckData.Y]);
		//グリッドの外？
		if( (tempX < 0) || (tempX >= Field.FIELD_GRID_X) || (tempY < 0) || (tempY >= Field.FIELD_GRID_Y)) {
			//場外なので優先度を最低にする
			prioTable[ i]	= AI_PRIO_MIN;
			continue;
		}

		//この場所の優先度を代入
		tempGrid	= (tempX +(tempY *Field.FIELD_GRID_X));
		prioTable[ i]	= calcGrid[ tempGrid];
	}


	//優先度のテーブルをデバッグ出力
	{
		//デバッグ用文字列
		string	temp	= "";

		//優先度テーブルが作成できたらAIの周辺の優先度を取得
		temp	+= "PRIO TABLE\n";
		for( tempY=0; tempY<Field.FIELD_GRID_Y; tempY++) {
			for( tempX=0; tempX<Field.FIELD_GRID_X; tempX++) {

				//Y軸は上下逆に出力されてしまうので逆さまにする
				temp	+= "\t\t"+ calcGrid[ tempX +((Field.FIELD_GRID_Y -1 -tempY) *Field.FIELD_GRID_X)] +"";

				//自分の位置
				if( (aiX==tempX) && (aiY==(Field.FIELD_GRID_Y -1 -tempY))) {
					temp	+= "*";
				}
			}
			temp	+= "\n";
		}
		temp	+= "\n";

		//移動方向別の優先度情報
		temp	+= "RESULT\n";
		for( i=0; i<(int)CheckDir.EnumMax; i++) {
			//この場所の優先度を代入
			temp	+= "\t"+ prioTable[ i] +"\t"+ (CheckDir)i +"\n";
		}

		//出力
		Debug.Log( ""+ temp);
	}


	//4方向の優先度情報を返す
	return	prioTable;
}





}
