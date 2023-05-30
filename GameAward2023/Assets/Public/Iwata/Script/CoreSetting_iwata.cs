using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoreSetting_iwata : ObjectBase
{
    public struct AttachFace
    {
        public Transform Trans;
        public bool isAttach;
        public bool isRelease;
    };

    /// <summary>
    /// 回転したときのボタンを押した方向
    /// </summary>
    public enum RotateFlag
    {
        E_ROTATE_FLAG_NULL = 0,
        E_ROTATE_FLAG_R,
        E_ROTATE_FLAG_L,
        E_ROTATE_FLAG_U,
        E_ROTATE_FLAG_D,

        E_ROTATE_FLAG_MAX
    }
    
    const float ROTATION = 90.0f;   // 回転角度
    const float DAMPING_RATE = 0.5f;   // 回転減衰率
    const float ENLARGE_SiZE = 1.25f;   //選択中のCoreの大きさ
    const float ORIZIN_SiZE = 1.00f;   //選択外のCoreの大きさ

    List<AttachFace> m_AttachFaces;	// 組み立てられる面
    List<Transform> hoge;
    int m_SelectFaceNum;     // 選択面の番号
    int m_timeToRotate;             // 回転時間
    float m_rotateY, m_rotateX;     // 角度
    float m_lateY, m_lateX;         // 遅延角度
    public int m_rotateFrameCnt;    // 回転フレームのカウント
    RotateFlag m_rotFlag;           //どっちに回転しているか
    bool m_isDepath;        // 面情報を取得し直すフラグ
    Vector3 AxisRotX;       //コアの回転用のX軸
    Vector3 AxisRotY;       //コアの回転用のY軸
    bool m_CanAttach;

    [SerializeField] PlayerController_iwata PController;
    [SerializeField] GameManager GM;
    [SerializeField] GameObject m_AttachJank;
    [SerializeField] float m_BreakForce = 1500.0f;

    // Start is called before the first frame update
    void Start()
    {
        AxisRotX = this.transform.right;        //縦回転するための軸登録
        AxisRotY = this.transform.up;           //横開店するための軸登録
        
        m_rotateY = m_rotateX = 0.0f;       //角度初期化
        m_lateY = m_lateX = 0.0f;       //遅延角度初期化

        // 回転時間を計算
        m_timeToRotate = (int)(Mathf.Log(0.00001f) / Mathf.Log(1.0f - DAMPING_RATE));
        
        //再検索が必要な時に立てるFlagを設定
        m_isDepath = false;

        //回転のフラグを初期化
        m_rotFlag = RotateFlag.E_ROTATE_FLAG_NULL;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //--- 回転中
        if (m_rotateFrameCnt > 0)
        {
            RotateCore();   // 回転処理			
            return;
        }
    }

    List<AttachFace> GetAttachFace()
    {
        // 面の格納先を用意
        List<AttachFace> attachFaces = new List<AttachFace>();
        AttachFace TempFace = new AttachFace();

        //--- 組み立てられる面を順番に格納
        foreach (Transform child in this.transform)
        {
            // 仮置きしているものと一緒ならスキップする
            if (child.gameObject == m_AttachJank) continue;
            if (child.gameObject.name == "CoreCenter") continue;

            // 手前に伸びるレイを用意
            Ray ray = new Ray(child.position, Vector3.back);
            RaycastHit hit;

            // 組み立てられない面はスルー
            // 手前に物があったらスキップ
            if (Physics.Raycast(ray, out hit, 10.0f))
            {
                if (hit.transform.gameObject != m_AttachJank)    //当たったのが仮置きしているの以外なら
                {
                    continue;
                }
            }
            

            //Transformの情報登録
            TempFace.Trans = child;

            //アタッチできる面かを判断するフラグを格納
            if(!child.GetComponent<IsAttachFace_iwata>())
            {
                TempFace.isAttach = true;
            }
            else
            {
                TempFace.isAttach = child.GetComponent<IsAttachFace_iwata>().CanAttach(Vector3.back);
            }

            //取り外せる面かを判断するフラグを格納
            if(child.name.Contains("Core_Child"))
            {
                TempFace.isRelease = false;
            }
            else
            {
                TempFace.isRelease = true;
            }
            
            attachFaces.Add(TempFace); // 面を格納
        }

        //--- ソート
        attachFaces.Sort((a, b) => {
            if (Mathf.Abs(a.Trans.position.y - b.Trans.position.y) > 0.75f)
            {
                // Y座標が異なる場合はY座標で比較する
                return b.Trans.position.y.CompareTo(a.Trans.position.y);
            }
            else
            {
                // Y座標が同じ場合はX座標で比較する
                return a.Trans.position.x.CompareTo(b.Trans.position.x);
            }
        });
        
        return attachFaces;
    }

    /// <summary>
    /// 通常のコアの回転
    /// </summary>
    void RotateCore()
    {
        float lastY = m_lateY;
        float lastX = m_lateX;

        //--- 遅延処理
        m_lateY = (m_rotateY - m_lateY) * DAMPING_RATE + m_lateY;
        m_lateX = (m_rotateX - m_lateX) * DAMPING_RATE + m_lateX;

        transform.RotateAround(transform.position, AxisRotY, m_lateY - lastY);
        transform.RotateAround(transform.position, AxisRotX, m_lateX - lastX);

        m_rotateFrameCnt++; // 回転フレームカウント

        //--- 回転終了時の処理
        if (m_rotateFrameCnt > m_timeToRotate)
        {
            transform.rotation = Quaternion.Euler(new Vector3(Mathf.Round(this.transform.rotation.eulerAngles.x), Mathf.Round(this.transform.rotation.eulerAngles.y), Mathf.Round(this.transform.rotation.eulerAngles.z)));

            m_rotateFrameCnt = 0;   // 回転フレームをリセット
            if (m_rotFlag == RotateFlag.E_ROTATE_FLAG_NULL) return;

            
            float hogepos = 0f;      //基準になるための座標
            switch (m_rotFlag)
            {
                case RotateFlag.E_ROTATE_FLAG_R:        //右に回転する
                    hogepos = m_AttachFaces[m_SelectFaceNum].Trans.position.y;      //基準のY軸の座標を取得
                    break;

                case RotateFlag.E_ROTATE_FLAG_L:        //左に回転する
                    hogepos = m_AttachFaces[m_SelectFaceNum].Trans.position.y;      //基準のY軸の座標を取得
                    break;

                case RotateFlag.E_ROTATE_FLAG_U:        //上に回転する
                    hogepos = m_AttachFaces[m_SelectFaceNum].Trans.position.x; ;      //基準のX軸の座標を取得
                    break;

                case RotateFlag.E_ROTATE_FLAG_D:
                    hogepos = m_AttachFaces[m_SelectFaceNum].Trans.position.x;
                    break;
            }
            
            m_AttachFaces = GetAttachFace();    // 次の組み立てられる面を取得
            int nextnum = 0;    //回転後の選択面の添え字を検索用
            List<int> numList = new List<int>();
            
            //回転の向きに応じた処理を行う
            switch (m_rotFlag)
            {
                case RotateFlag.E_ROTATE_FLAG_R:        //右に回転する
                    for (int i = 0; i < m_AttachFaces.Count; i++)
                    {
                        //Debug.Log(m_AttachFaces[i].Trans.name + ":" + Mathf.Abs(m_AttachFaces[i].Trans.position.y - hogepos));
                        if (Mathf.Abs(m_AttachFaces[i].Trans.position.y - hogepos) > 0.2f) continue;       //検索した面と基準のY座標を比較する

                        numList.Add(i);
                    }
                    nextnum = numList[0];

                    foreach(int child in numList)
                    {
                        Debug.Log(m_AttachFaces[child].Trans.name);
                        if (m_AttachFaces[nextnum].Trans.position.x > m_AttachFaces[child].Trans.position.x) nextnum = child;      //今の候補の面の座標より右に検索した面があるなら候補を変える
                    }

                    m_AttachJank.transform.Rotate(0.0f, -90.0f, 0.0f);
                    break;

                case RotateFlag.E_ROTATE_FLAG_L:        //左に回転する
                    
                    for (int i = 0; i < m_AttachFaces.Count; i++)
                    {
                        Debug.Log(m_AttachFaces[i].Trans.name + ":" + Mathf.Abs(m_AttachFaces[i].Trans.position.y - hogepos));
                        if (Mathf.Abs(m_AttachFaces[i].Trans.position.y - hogepos) > 0.2f) continue;       //検索した面と基準のY座標を比較する

                        numList.Add(i);
                    }
                    nextnum = numList[0];

                    foreach (int child in numList)
                    {
                        Debug.Log(m_AttachFaces[child].Trans.name);
                        if (m_AttachFaces[nextnum].Trans.position.x < m_AttachFaces[child].Trans.position.x) nextnum = child;      //今の候補の面の座標より右に検索した面があるなら候補を変える
                    }
                    
                    m_AttachJank.transform.Rotate(0.0f, 90.0f, 0.0f);
                    break;

                case RotateFlag.E_ROTATE_FLAG_U:        //上に回転する

                    for (int i = 0; i < m_AttachFaces.Count; i++)
                    {
                        Debug.Log(m_AttachFaces[i].Trans.name + ":" + Mathf.Abs(m_AttachFaces[i].Trans.position.x - hogepos));
                        if (Mathf.Abs(m_AttachFaces[i].Trans.position.x - hogepos) > 0.2f) continue;       //検索した面と基準のY座標を比較する

                        numList.Add(i);
                    }
                    nextnum = numList[0];

                    foreach (int child in numList)
                    {
                        Debug.Log(m_AttachFaces[child].Trans.name);
                        if (m_AttachFaces[nextnum].Trans.position.y > m_AttachFaces[child].Trans.position.y) nextnum = child;      //今の候補の面の座標より右に検索した面があるなら候補を変える
                    }

                    m_AttachJank.transform.Rotate(90.0f, 0.0f, 0.0f);
                    break;

                case RotateFlag.E_ROTATE_FLAG_D:

                    for (int i = 0; i < m_AttachFaces.Count; i++)
                    {
                        Debug.Log(m_AttachFaces[i].Trans.name + ":" + Mathf.Abs(m_AttachFaces[i].Trans.position.x - hogepos));
                        if (Mathf.Abs(m_AttachFaces[i].Trans.position.x - hogepos) > 0.2f) continue;       //検索した面と基準のY座標を比較する

                        numList.Add(i);
                    }
                    nextnum = numList[0];

                    foreach (int child in numList)
                    {
                        if (m_AttachFaces[nextnum].Trans.position.y < m_AttachFaces[child].Trans.position.y) nextnum = child;      //今の候補の面の座標より右に検索した面があるなら候補を変える
                    }

                    m_AttachJank.transform.Rotate(-90.0f, 0.0f, 0.0f);
                    break;
            }
            
            m_SelectFaceNum = nextnum;
            m_AttachJank.GetComponent<JankBase_iwata>().PutJank(m_AttachFaces[m_SelectFaceNum].Trans, this.transform);
            CheckCanAttach();
            
            m_rotFlag = RotateFlag.E_ROTATE_FLAG_NULL;
        }
    }

    /// <summary>
    /// 入力の対してのコアの処理
    /// </summary>
    /// <param name="axisX">横入力</param>
    /// <param name="axisY">縦入力</param>
    public void InputAxisCore(int axisX, int axisY)
    {
        if(GameManager.JointStage.GetComponent<JointStageManager>().JSStatus == JointStageManager.eJointStageStatus.E_JOINTSTAGE_STATUS_SELECT)
        {
            m_rotateY += ROTATION * axisX;  // 角度を設定
            m_rotateX -= ROTATION * axisY;  // 角度を設定
            m_rotateFrameCnt = 1;   // 最初のカウント
            AudioManager.PlaySE(AudioManager.SEKind.E_SE_KIND_PREV_KEYMOVE);
        }
        else if(GameManager.JointStage.GetComponent<JointStageManager>().JSStatus == JointStageManager.eJointStageStatus.E_JOINTSTAGE_STATUS_PUT)
        {
            this.transform.Rotate(0.0f, -10.0f, 0.0f, Space.World);      //コアの傾きを一時的に0，0，0に戻す
            Vector3 pos = m_AttachFaces[m_SelectFaceNum].Trans.position;
            pos.x += axisX;
            pos.y += axisY;
            Vector2 newxtFacePos = new Vector2(pos.x, pos.y);
            for (int i = 0; i < m_AttachFaces.Count; i++)
            {
                //--- 現在の面と次の面のXY座標をVector2に格納
                Vector2 currentFacePos = new Vector2(m_AttachFaces[i].Trans.position.x, m_AttachFaces[i].Trans.position.y);

                // XY平面での距離が離れすぎていたらスルー
                if (Vector2.Distance(currentFacePos, newxtFacePos) > 0.05f) continue;
                
                this.transform.Rotate(0.0f, 10.0f, 0.0f, Space.World);      //コアの傾きをもとに戻す

                m_SelectFaceNum = i;
                m_AttachJank.GetComponent<JankBase_iwata>().PutJank(m_AttachFaces[i].Trans, this.transform);
                CheckCanAttach();
                
                return;
            }
            
            this.transform.Rotate(0.0f, 10.0f, 0.0f, Space.World);      //コアの傾きをもとに戻す

            m_rotateY += ROTATION * (int)axisX;  // 角度を設定
            m_rotateX -= ROTATION * (int)axisY;  // 角度を設定
            m_rotateFrameCnt = 1;   // 最初のカウント
            if (axisX < 0)
            {
                m_rotFlag = RotateFlag.E_ROTATE_FLAG_L;
            }
            else if(axisX > 0)
            {
                m_rotFlag = RotateFlag.E_ROTATE_FLAG_R;
            }
            else if (axisY < 0)
            {
                m_rotFlag = RotateFlag.E_ROTATE_FLAG_D;
            }
            else if(axisY > 0)
            {
                m_rotFlag = RotateFlag.E_ROTATE_FLAG_U;
            }
        }
    }

    /// <summary>
    /// 組み立てることができるか判定する
    /// </summary>
    public void CheckCanAttach()
    {
        Renderer[] renderers = m_AttachJank.GetComponent<JankBase_iwata>().Renderers;
        if (m_AttachFaces[m_SelectFaceNum].Trans.GetComponent<JankStatus>().CanColliderFlags(this.transform) && m_AttachJank.GetComponent<JankStatus>().CanCollisionFlags(this.transform))
        {
            //Debug.Log("できるよー");
            foreach (Renderer child in renderers)
            {
                child.material.SetInt("_CraftFlag", 1);
            }

            m_CanAttach = true;
        }
        else
        {
            //Debug.Log("むりだよー");

            foreach (Renderer child in renderers)
            {
                child.material.SetInt("_CraftFlag", 0);
            }

            m_CanAttach = false;
        }
    }

    /// <summary>
    /// カーソルで選択されたガラクタをコアに配置する
    /// </summary>
    /// <param name="jank">カーソルで選択されたガラクタの情報</param>
    public void PutJank(GameObject jank)
    {
        m_AttachFaces = GetAttachFace();    // 次の組み立てられる面を取得
        m_SelectFaceNum = 0;        //選択している場所を左上に初期化
        m_AttachJank = jank;        //仮置きされているガラクタを登録
        m_AttachJank.GetComponent<JankBase_iwata>().SetJank(m_AttachFaces[m_SelectFaceNum].Trans);        //ガラクタの仮置きの処理
        CheckCanAttach();

        Renderer[] renderers = m_AttachJank.GetComponent<JankBase_iwata>().Renderers;
        foreach (Renderer child in renderers)
        {
            child.material.SetInt("_ColorFlag", 1);
            //child.material.SetInt("_CraftFlag", 1);
        }

    }

    /// <summary>
    /// 仮置きしているオブジェクトを確定させる
    /// </summary>
    /// <returns></returns>
    public bool JointCore()
    {
        if(m_CanAttach)
        {
            m_AttachJank.GetComponent<JankBase_iwata>().Orizin.SetActive(false);
            FixedJoint comp = m_AttachJank.AddComponent<FixedJoint>();
            comp.connectedBody = m_AttachFaces[m_SelectFaceNum].Trans.GetComponent<Rigidbody>();
            comp.breakForce = m_BreakForce;
            m_AttachFaces[m_SelectFaceNum].Trans.GetComponent<JankStatus>().ConnectedChild = m_AttachJank;
            Renderer[] renderers = m_AttachJank.GetComponent<JankBase_iwata>().Renderers;
            foreach (Renderer child in renderers)
            {
                child.material.SetInt("_ColorFlag", 0);
                //child.material.SetInt("_CraftFlag", 0);
            }
            m_AttachJank = null;
            m_AttachFaces.Clear();
            m_SelectFaceNum = 0;
            Debug.Log("いいね");



            //m_AttachJank.GetComponent<Renderer>().material.SetInt("_ColorFlag", 0);
            //m_AttachJank.GetComponent<Renderer>().material.SetInt("_CraftFlag", 0);

			AudioManager.PlaySE(AudioManager.SEKind.E_SE_KIND_ASSEMBLE);
            return true;
        }
        else
        {
            Debug.Log("むりっていってんだろ");
            return false;
        }
    }

    public void CanselCore()
    {
        Destroy(m_AttachJank);
    }

    public void ReleaseCore()
    {
        if(m_AttachFaces[m_SelectFaceNum].isRelease)
        {
            //外す処理
            Debug.Log(m_AttachFaces[m_SelectFaceNum].Trans.name + "外した");
            m_AttachFaces[m_SelectFaceNum].Trans.GetComponent<SpownJank_iwata>().RemoveCore();
            m_isDepath = true;
        }
        else
        {
            //外せなかったときに鳴らすSE
            Debug.Log(m_AttachFaces[m_SelectFaceNum].Trans.name + "外せない");
        }
    }

    public void JointToRot()
    {
        GameObject clone = Instantiate(this.gameObject, new Vector3(-9.0f, 1.5f, -9.0f), Quaternion.identity);
        clone.AddComponent<RotationCore>();
        clone.AddComponent<CloneCoreMove>();
    }

    public void RotToJoint()
    {
        m_isDepath = true;
        this.transform.position = new Vector3(-4.0f, 11.0f, -38.0f);
        this.transform.rotation = Quaternion.identity;
        foreach (Transform child in this.transform)
        {
            child.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        }
    }

    public void PlayToJoint()
    {
        m_isDepath = true;
        this.transform.position = new Vector3(-4.0f, 11.0f, -38.0f);
        this.transform.rotation = Quaternion.identity;
        foreach (Transform child in this.transform)
        {
            child.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        }
    }

    public void PlayToRot()
    {
        this.transform.rotation = Quaternion.identity;
    }

    public GameObject AttachJank
    {
        get { return m_AttachJank; }
    }

    public Transform SelectFace
    {
        get { return m_AttachFaces[m_SelectFaceNum].Trans; }
    }

}