using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoreSetting_iwata : MonoBehaviour
{
    const float ROTATION = 90.0f;   // 回転角度
    const float DAMPING_RATE = 0.5f;   // 回転減衰率
    const float ENLARGE_SiZE = 1.25f;   //選択中のCoreの大きさ
    const float ORIZIN_SiZE = 1.00f;   //選択外のCoreの大きさ

    List<Transform> m_attachFaces;	// 組み立てられる面
    int m_SelectFaceNum;     // 選択面の番号
    int m_timeToRotate;             // 回転時間
    float m_rotateY, m_rotateX;     // 角度
    float m_lateY, m_lateX;         // 遅延角度
    public int m_rotateFrameCnt;           // 回転フレームのカウント
    bool m_isDepath;        // 面情報を取得し直すフラグ

    //[SerializeReference] AudioClip m_RotSound;  //オーディオファイルの情報
    //AudioSource audioSource;    //再生するためのハンドル

    // Start is called before the first frame update
    void Start()
    {
        m_attachFaces = GetAttachFace();	// 組み立てられる面を取得
        m_SelectFaceNum = 0;
        m_rotateY = m_rotateX = 0.0f;
        m_lateY = m_lateX = 0.0f;

        // 回転時間を計算
        m_timeToRotate = (int)(Mathf.Log(0.00001f) / Mathf.Log(1.0f - DAMPING_RATE));

        //選ばれているCoreを大きくする
        EnlargeSizeCore();

        //再検索が必要な時に立てるFlagを設定
        m_isDepath = false;

        //audioSource = GetComponent<AudioSource>();


    }

    // Update is called once per frame
    void Update()
    {
        //--- 回転中
        if (m_rotateFrameCnt > 0)
        {
            RotateCore();   // 回転処理			
            return;
        }

        if (m_isDepath)
        {
            m_attachFaces = GetAttachFace();    // 次の組み立てられる面を取得

            // 選択中の面を大きく協調する
            EnlargeSizeCore();

            m_isDepath = false;
        }
    }

    List<Transform> GetAttachFace()
    {
        // 面の格納先を用意
        List<Transform> attachFaces = new List<Transform>();

        //--- 組み立てられる面を順番に格納
        foreach (Transform child in this.transform)
        {
            if (child.tag != "Player") continue;

            // 手前に伸びるレイを用意
            Ray ray = new Ray(child.position, Vector3.back);
            RaycastHit hit;

            // 組み立てられない面はスルー
            // 手前に物があったらスキップ
            if (Physics.Raycast(ray, out hit, 10.0f)) continue;

            attachFaces.Add(child); // 面を格納
        }

        //--- ソート
        attachFaces.Sort((a, b) => {
            if (Mathf.Abs(a.position.y - b.position.y) > 0.75f)
            {
                // Y座標が異なる場合はY座標で比較する
                return b.position.y.CompareTo(a.position.y);
            }
            else
            {
                // Y座標が同じ場合はX座標で比較する
                return a.position.x.CompareTo(b.position.x);
            }
        });

        return attachFaces;
    }

    void EnlargeSizeCore()
    {
        m_attachFaces[m_SelectFaceNum].localScale = new Vector3(ENLARGE_SiZE, ENLARGE_SiZE, ENLARGE_SiZE);
    }

    void UndoSizeCore()
    {
        m_attachFaces[m_SelectFaceNum].localScale = new Vector3(ORIZIN_SiZE, ORIZIN_SiZE, ORIZIN_SiZE);
    }

    public void ChangeFaceX(float axis)
    {
        UndoSizeCore();
        Vector3 pos = m_attachFaces[m_SelectFaceNum].position;
        pos.x += axis;
        for(int i = 0; i < m_attachFaces.Count; i++)
        {
            //--- 現在の面と次の面のXY座標をVector2に格納
            Vector2 currentFacePos = new Vector2(m_attachFaces[i].position.x, m_attachFaces[i].position.y);
            Vector2 newxtFacePos = new Vector2(pos.x, pos.y);

            // XY平面での距離が離れすぎていたらスルー
            if (Vector2.Distance(currentFacePos, newxtFacePos) > 0.05f) continue;

            m_SelectFaceNum = i;
            EnlargeSizeCore();
            return;
        }

        m_rotateY += ROTATION * (int)axis;  // 角度を設定
        m_rotateFrameCnt = 1;	// 最初のカウント
        m_isDepath = true;
        //audioSource.PlayOneShot(m_RotSound);    //SEの再生
    }

    public void ChangeFaceY(float axis)
    {
        UndoSizeCore();
        Vector3 pos = m_attachFaces[m_SelectFaceNum].position;
        pos.y += axis;
        for (int i = 0; i < m_attachFaces.Count; i++)
        {
            //--- 現在の面と次の面のXY座標をVector2に格納
            Vector2 currentFacePos = new Vector2(m_attachFaces[i].position.x, m_attachFaces[i].position.y);
            Vector2 newxtFacePos = new Vector2(pos.x, pos.y);

            // XY平面での距離が離れすぎていたらスルー
            if (Vector2.Distance(currentFacePos, newxtFacePos) > 0.05f) continue;

            m_SelectFaceNum = i;
            EnlargeSizeCore();
            return;
        }

        m_rotateX -= ROTATION * (int)axis;  // 角度を設定
        m_rotateFrameCnt = 1;   // 最初のカウント
        m_isDepath = true;
        //audioSource.PlayOneShot(m_RotSound);    //SEの再生
    }

    //Coreを回転される
    void RotateCore()
    {
        float lastY = m_lateY;
        float lastX = m_lateX;

        //--- 遅延処理
        m_lateY = (m_rotateY - m_lateY) * DAMPING_RATE + m_lateY;
        m_lateX = (m_rotateX - m_lateX) * DAMPING_RATE + m_lateX;

        //--- 座標計算
        this.transform.Rotate(Vector3.up, m_lateY - lastY, Space.World);
        this.transform.Rotate(Vector3.right, m_lateX - lastX, Space.World);

        m_rotateFrameCnt++; // 回転フレームカウント

        //--- 回転終了時の処理
        if (m_rotateFrameCnt > m_timeToRotate)
        {
            m_attachFaces = GetAttachFace();    // 次の組み立てられる面を取得
            m_rotateFrameCnt = 0;   // 回転フレームをリセット
            m_SelectFaceNum = 0;    // 選択面の番号をリセット
            m_attachFaces[m_SelectFaceNum].localScale = new Vector3(1.25f, 1.25f, 1.25f);   // 現在の面を協調
        }
    }
}
