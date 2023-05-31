using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//--- レイの衝突状況の遷移
public enum E_RAY_HIT_STATE
{
	ENTER,	//当たった瞬間
	EXIT,	//離れた瞬間
	STAY,	//当たっている
	NOT_HIT	//当たっていない
}

public class CursorController_araki : ObjectBase
{
	static RectTransform m_rectTransform;   // カーソルの座標情報
    [SerializeReference] GameObject m_lastPointJunk; // 前フレームで指していたガラクタのデータ
	GameObject m_selectJunk;
	CoreSetting_iwata m_coreSetting;
    [SerializeReference] GameObject m_previewJunk;   // プレビュー用ガラクタのデータ
	[SerializeReference] PreviewCamera_araki m_previreCamera;
	JointStageManager m_jointStageManager;
    float axisX = 0.0f;
    float axisY = 0.0f;
	bool m_isHighSpeed = false;
<<<<<<< HEAD
    E_RAY_HIT_STATE m_state;
=======
	E_RAY_HIT_STATE m_state;
>>>>>>> main

    // Start is called before the first frame update
    void Start()
	{
		m_rectTransform = GetComponent<RectTransform>();
		m_rectTransform.anchoredPosition = new Vector2(0.0f, 0.0f);
		m_lastPointJunk = null;
		m_jointStageManager = transform.root.GetComponent<JointStageManager>();
		m_coreSetting = GameObject.Find("Core").GetComponent<CoreSetting_iwata>();
	}

    // Update is called once per frame
    void Update()
    {
        if (GameManager.GameStatus != GameManager.eGameStatus.E_GAME_STATUS_JOINT) return;
		if (m_jointStageManager.JSStatus == JointStageManager.eJointStageStatus.E_JOINTSTAGE_STATUS_PUT) return;

<<<<<<< HEAD
        //--- プレビュー用ガラクタを生成
        m_state = CheckRayHitState();
        switch (m_state)
=======
		//--- プレビュー用ガラクタを生成
		m_state = CheckRayHitState();
		Debug.Log(m_state);
		switch (m_state)
>>>>>>> main
		{
			case E_RAY_HIT_STATE.ENTER: // 指した瞬間
				m_previreCamera.StartNoise();	// ノイズ開始
                AudioManager.PlaySE(AudioManager.SEKind.E_SE_KIND_NOISE);
				m_selectJunk = m_lastPointJunk;
				break;
			case E_RAY_HIT_STATE.EXIT:  // 離れた瞬間
                AudioManager.StopSE(AudioManager.SEKind.E_SE_KIND_NOISE);

				// 仮置きを開始した場合はプレビューを破棄しない
				if (m_coreSetting.AttachJank != null) break;

				//--- プレビューを破棄
				m_selectJunk = null;
				if (m_previewJunk == null) break;
				Destroy(m_previewJunk);
				m_previewJunk = null;
				m_previreCamera.StartNoise();   // ノイズ開始
				break;
			case E_RAY_HIT_STATE.STAY:
				if (!m_previreCamera.isEndNoise) break;

                //--- ノイズが終わったらガラクタを生成
                AudioManager.StopSE(AudioManager.SEKind.E_SE_KIND_NOISE);
                AudioManager.PlaySE(AudioManager.SEKind.E_SE_KIND_MONITORON);
                m_previewJunk = (GameObject)Instantiate((Object)m_selectJunk,
					new Vector3(), Quaternion.identity);
				// 動作を固定
				m_previewJunk.GetComponent<Rigidbody>().constraints
					= RigidbodyConstraints.FreezeAll;
				m_previewJunk.AddComponent<PreviewJunk_araki>();
				break;
			case E_RAY_HIT_STATE.NOT_HIT:
				// 仮置き中の場合はプレビューを破棄しない
				if (m_coreSetting.AttachJank != null) break;

				//--- プレビューを破棄
				if (m_selectJunk != null) m_selectJunk = null;
				if (m_previewJunk == null) break;
				Destroy(m_previewJunk);
				m_previewJunk = null;
				m_previreCamera.StartNoise();   // ノイズ開始
				break;
			default:    // 上記以外の場合は処理しない
				break;
		}

        axisX = PadInput.GetAxis("Horizontal_R");
        axisY = PadInput.GetAxis("Vertical_R");

		//--- カーソルの移動速度切り替え
		if (!PadInput.GetKeyDown(KeyCode.JoystickButton9)) return;
		m_isHighSpeed = !m_isHighSpeed;

		//--- カーソルの色を変更
		Image image = GetComponent<Image>();
		if(m_isHighSpeed) image.color = new Color32(0, 255, 255, 255);	// 水色
		else image.color = new Color32(255, 0, 0, 255);					// 赤色
	}

    private void FixedUpdate()
    {
		float speed = m_isHighSpeed ? 9.0f : 5.5f;

		//--- 移動処理
		Vector2 pos = m_rectTransform.anchoredPosition;
        pos.x += axisX * speed;
        pos.y += axisY * speed;

        //--- 画面外に出ていくのを防ぐ(左画面のみ移動可能)
        if (pos.x > Screen.width / 2.0f) pos.x = Screen.width / 2.0f;
        if (pos.x < -Screen.width / 2.0f) pos.x = -Screen.width / 2.0f;
        if (pos.y > Screen.height / 2.0f) pos.y = Screen.height / 2.0f;
        if (pos.y < -Screen.height / 2.0f) pos.y = -Screen.height / 2.0f;

        m_rectTransform.anchoredPosition = pos;	// カーソルの位置を確定
    }

    /// <summary>
    /// カーソルとガラクタの衝突状況を取得
    /// </summary>
    E_RAY_HIT_STATE CheckRayHitState()
	{
		//--- カメラを取得
		GameObject cam = GameObject.Find("JointCamera");
		if (cam == null) return E_RAY_HIT_STATE.NOT_HIT;    // カメラが無ければ処理しない

		//--- レイで当たり判定を取る
		Ray ray = GetCameraToRay(cam);
		RaycastHit[] hits = Physics.RaycastAll(ray);
		float minZ = Mathf.Infinity;
		GameObject minZJunk = null;

		//--- 取得したオブジェクトから最前にあるガラクタを求める
		foreach(RaycastHit junk in hits)
		{
			//--- 条件外の物は省く
			if (junk.transform.tag != "Jank") continue;
			if (junk.transform.position.z >= minZ) continue;

			minZ = junk.transform.position.z;		// Z座標の最小値を更新
			minZJunk = junk.transform.gameObject;	// 最前のオブジェクトを更新
		}

		// カーソル(レイ)が何も指していなかったら
		if (minZJunk == null)
		{
			GameObject temp = m_lastPointJunk;
			m_lastPointJunk = null; // 過去のデータをリセット

			// 前フレームでガラクタを指していた場合
			if (temp != null) return E_RAY_HIT_STATE.EXIT;

			return E_RAY_HIT_STATE.NOT_HIT;
		}

		// ガラクタに当たっていなければ処理しない
		if (minZJunk.transform.parent.name != "Jank")
		{
			GameObject temp = m_lastPointJunk;
			m_lastPointJunk = null; // 過去のデータをリセット

			// 前フレームでガラクタを指していた場合
			if (temp != null) return E_RAY_HIT_STATE.EXIT;

			return E_RAY_HIT_STATE.NOT_HIT;
		}

		//--- 前フレームでガラクタを指していなかった場合
		if (m_lastPointJunk == null)
		{
			m_lastPointJunk = minZJunk;  // 過去のデータとして退避
			return E_RAY_HIT_STATE.ENTER;
		}

		// 指し続けている場合
		if (m_lastPointJunk == minZJunk) return E_RAY_HIT_STATE.STAY;

		//--- 何も指さなくなった場合
		m_lastPointJunk = null; // 過去のデータをリセット
		return E_RAY_HIT_STATE.EXIT;
	}

	/// <summary>
	/// カーソルからのレイを取得
	/// </summary>
	public static Ray GetCameraToRay(GameObject cam)
	{
        // カメラ→カーソル(ワールド座標系)のレイを取得
		Vector2 pos = m_rectTransform.anchoredPosition;
        Camera camdata = cam.GetComponent<Camera>();
		return camdata.ScreenPointToRay(new Vector3(pos.x + Screen.width / 2.0f, pos.y + Screen.height / 2.0f, 0.0f));
	}

    public GameObject SelectJank
    {
        get { return m_lastPointJunk; }
    }

	public GameObject GetAttachJunk()
	{
		//--- カメラを取得
		GameObject cam = GameObject.Find("JointCamera");
		if (cam == null) return null;    // カメラが無ければ処理しない

		//--- レイで当たり判定を取る
		Ray ray = GetCameraToRay(cam);
		RaycastHit hit;
		// 入れ子を削減する為に否定で判定
		if (Physics.Raycast(ray, out hit)) // カーソルが指す物を取得
		{
			if (hit.transform.GetComponents<FixedJoint>().Length <= 0) return null;
			if (hit.transform.tag == "Jank") return hit.transform.gameObject;
		}

		return null;
	}

	private void OnDisable()
	{
		if (m_previewJunk == null) return;

		//--- スタートしたらプレビュー用ガラクタを削除する
		Destroy(m_previewJunk);
		m_previewJunk = null;
	}

<<<<<<< HEAD
    public E_RAY_HIT_STATE state
    {
        get { return m_state; }
    }

=======
	public E_RAY_HIT_STATE state
	{
		get { return m_state; }
	}
>>>>>>> main
}