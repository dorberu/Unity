using UnityEngine;
using System;
using System.Collections;
using live2d;
using live2d.framework;

[ExecuteInEditMode]
public class DemoAndMotion : MonoBehaviour
{
	// ○○.moc.bytesファイルをアタッチします
	public TextAsset m_mocFile;
	// physics.json ファイルをアタッチします
	public TextAsset m_physicsFile;
	// テクスチャファイルをアタッチします
	public Texture2D[] m_textureFiles;
	// ○○.mtn.bytesファイルをアタッチします
	public TextAsset[] m_motionFiles;
	
	// Live2Dのモデルを.mocファイルから生成します
	private Live2DModelUnity m_live2DModel;
	// まばたきを管理します
	private EyeBlinkMotion m_eyeBlink = new EyeBlinkMotion();
	// ドラッグ管理用です
	private L2DTargetPoint m_dragMgr = new L2DTargetPoint();
	// 物理演算を管理します
	private L2DPhysics m_physics;
	//キャンバス管理用変数
	private Matrix4x4 m_live2DCanvasPos;
	// モーション複数格納用変数
	private Live2DMotion[] m_motions;
	// モーション管理用マネージャ
	private MotionQueueManager m_motionMgr;
	
	void Start () 
	{
		Live2D.init(); // Live2Dの初期化
		load(); // 下記Load()関数を実行
	}
	
	void load()
	{
		// .moc.bytesファイルをロードしてセット
		m_live2DModel = Live2DModelUnity.loadModel(m_mocFile.bytes);
		
		// テクスチャのファイル数だけ読み込んでセット
		for (int i = 0; i < m_textureFiles.Length; i++)
		{
			m_live2DModel.setTexture(i, m_textureFiles[i]);
		}
		
		// キャンバスを用意
		float modelWidth = m_live2DModel.getCanvasWidth();
		m_live2DCanvasPos = Matrix4x4.Ortho(0, modelWidth, modelWidth, 0, -50.0f, 50.0f);
		
		// 物理設定ファイルが空ならロード
		if (m_physicsFile != null) m_physics = L2DPhysics.load(m_physicsFile.bytes);
		
		// モーション管理用変数を用意
		m_motionMgr = new MotionQueueManager();
		
		// モーションファイルの数だけモーション管理用配列を確保
		m_motions = new Live2DMotion[m_motionFiles.Length];
		// モーションファイルの数だけモーションを読み込み
		for (int i = 0; i < m_motionFiles.Length; i++)
		{
			m_motions[i] = Live2DMotion.loadMotion(m_motionFiles[i].bytes);
		}
	}
	
	
	void Update()
	{
		// マウスカーソルの座標を取得
		var pos = Input.mousePosition;
		if (Input.GetMouseButtonDown(0))
		{
			// マウス左ボタンが押し下げられた瞬間の処理を記述
		}
		// 押し下げられた瞬間ではないが,マウス左ボタンが押されている状態(ドラッグ中)
		else if (Input.GetMouseButton(0))
		{
			// ドラッグ管理変数に計算した座標をセット
			m_dragMgr.Set(pos.x / Screen.width*2-1, pos.y/Screen.height*2-1);
		}
		// マウス左ボタンが押し上げられた瞬間
		else if (Input.GetMouseButtonUp(0))
		{
			// ドラッグ管理変数を(0,0)にセット
			m_dragMgr.Set(0, 0);
		}
		
		// 任意のモーションをZキーで再生
		if (Input.GetKeyDown (KeyCode.Z)) 
		{
			m_motionMgr.startMotion(m_motions[2]);
		}
	}
	
	// レンダリング時に呼び出される
	void OnRenderObject()
	{
		// Live2Dモデルが空ならロード
		if (m_live2DModel == null)
		{
			load();
		}
		
		m_live2DModel.setMatrix(transform.localToWorldMatrix * m_live2DCanvasPos);
		
		// もしアプルリケーションがプレイ中でなければモデルを更新して描画し戻る
		if ( ! Application.isPlaying)
		{
			m_live2DModel.update();
			m_live2DModel.draw();
			return;
		}
		
		Idle (); // 下記 Idle() 関数の中身を実行し ここに戻ってきます
		Drag (); // 下記 Drag() 関数の中身を実行し ここに戻ってきます ※Idle()の後に実行させてください
		
		// まばたき処理
		m_eyeBlink.setParam(m_live2DModel);
		
		// 物理演算を更新
		if (m_physics != null) m_physics.updateParam(m_live2DModel);
		
		// モデルの更新
		m_live2DModel.update();
		
		// モデルの描画
		m_live2DModel.draw();
	}
	
	// アイドリングモーションをランダム再生します
	void Idle ()
	{
		// 現在のモーションが終わっていれば
		if (m_motionMgr.isFinished())
		{
			// 乱数を生成
			int rnd = UnityEngine.Random.Range(0, m_motions.Length - 1);
			// モーションを再生
			m_motionMgr.startMotion(m_motions[rnd]);
		}
		m_motionMgr.updateParam(m_live2DModel);
	}
	
	// ドラッグした結果をLive2Dのパラメータに反映
	void Drag ()
	{
		m_dragMgr.update();
		// 顔の向きで追いかける処理
		m_live2DModel.addToParamFloat("PARAM_ANGLE_X", m_dragMgr.getX() * 30);
		m_live2DModel.addToParamFloat("PARAM_ANGLE_Y", m_dragMgr.getY() * 30);
		
		// 身体の向きで追いかける処理
		m_live2DModel.addToParamFloat("PARAM_BODY_ANGLE_X", m_dragMgr.getX() * 10);
		
		// 目で追いかける処理
		m_live2DModel.addToParamFloat("PARAM_EYE_BALL_X", m_dragMgr.getX());
		m_live2DModel.addToParamFloat("PARAM_EYE_BALL_Y", m_dragMgr.getY());
		
		// 時間に応じてサインカーブで波のように呼吸のパラメータを更新
		double timeSec = UtSystem.getUserTimeMSec() / 1000.0;
		double t = timeSec * 2 * Math.PI;
		m_live2DModel.setParamFloat("PARAM_BREATH", (float)(0.5f + 0.5f * Math.Sin(t / 3.0)));
	}
}