using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class JankBase_iwata : JankStatus
{
    [SerializeField] Vector3 m_StartPos;      //開始時のポジション
    [SerializeField] Quaternion m_StartRot;      //開始時の回転
    [SerializeField] GameObject m_Origin;     //クローンなら元のオブジェクトを入れるよう
    [SerializeField] Renderer[] m_Renderer;

    /// <summary>
    /// 各ジャンク特有の処理を行う
    /// </summary>
    public abstract void work();

    /// <summary>
    /// 各ジャンクのパラメータを取得する
    /// </summary>
    public abstract List<float> GetParam();

    /// <summary>
    /// 各ジャンクのパラメータを設定する
    /// </summary>
    /// <param name="paramList">設定するパラメータのリスト</param>
    public abstract void SetParam(List<float> paramList);

    // Start is called before the first frame update
    protected void Start()
    {
        m_StartPos = transform.position;      //初期座標を登録
        m_StartRot = transform.rotation;      //初期回転を登録
        m_Renderer = GetComponentsInChildren<Renderer>(true);
    }

    ///<summary>
    ///ガラクタをガラクタ山に戻す時のガラクタの処理
    ///</summary>
    public void ReturnJank()
    {
        this.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        this.transform.position = m_StartPos;
        this.transform.rotation = m_StartRot;
    }

    /// <summary>
    /// 仮置きしたガラクタの処理
    /// </summary>
    /// <param name="trans">選択面の情報</param>
    public void SetJank(Transform trans)
    {
        Transform CoreTrans = trans.parent;     //コアの大元を取得

        this.transform.rotation = Quaternion.identity;      //回転を初期化

        PutJank(trans, CoreTrans);

        //this.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;        //力が加わっても移動回転しないように固定
        
    }

    public void PutJank(Transform trans, Transform core)
    {
        core.Rotate(0.0f, -10.0f, 0.0f, Space.World);      //コアの傾きを一時的に0，0，0に戻す
        this.transform.parent = null;
        Vector3 pos = trans.transform.position;     //付ける面の座標取得
        BoxCollider thiscollider = GetComponent<BoxCollider>();
        BoxCollider corecollider = trans.GetComponent<BoxCollider>();
        float[] dot = new float[3];
        dot[0] = Vector3.Dot(transform.forward, Vector3.forward);
        dot[1] = Vector3.Dot(transform.right, Vector3.forward);
        dot[2] = Vector3.Dot(transform.up, Vector3.forward);

        int nearestValue = 0;
        float nearestDistance = float.MaxValue;
        for (int i = 0; i < 3; i++)
        {
            float distance = Mathf.Abs(Mathf.Abs(dot[i]) - 1f);
            if (distance < nearestDistance)
            {
                nearestValue = i;
                nearestDistance = distance;
            }
        }
        switch (nearestValue)
        {
            case 0:
                pos.z -= corecollider.size.z / 2.0f;     //付ける面の大きさの半分ずらす
                pos.z -= thiscollider.size.z / 2.0f;        //自分の半分ずらす
                break;
            case 1:
                pos.z -= corecollider.size.z / 2.0f;     //付ける面の大きさの半分ずらす
                pos.z -= thiscollider.size.x / 2.0f;        //自分の半分ずらす
                break;
            case 2:
                pos.z -= corecollider.size.z / 2.0f;     //付ける面の大きさの半分ずらす
                pos.z -= thiscollider.size.y / 2.0f;        //自分の半分ずらす
                break;
        }
        this.transform.position = pos;      //ずらして決めた座標に移動する
        this.transform.parent = core;
        core.Rotate(0.0f, 10.0f, 0.0f, Space.World);       //一時的に0，0，0にしていた回転を戻す
    }

    public void RotJank(int axisX, int axisY, Transform core)
    {
        Transform trans = core.GetComponent<CoreSetting_iwata>().SelectFace;
        core.Rotate(0.0f, -10.0f, 0.0f, Space.World);      //コアの傾きを一時的に0，0，0に戻す
        if (axisX != 0)
        {
            transform.Rotate(0.0f, -90.0f * axisX, 0.0f, Space.World);
        }
        else if(axisY != 0)
        {
            transform.Rotate(90.0f * axisY, 0.0f, 0.0f, Space.World);
        }
        core.GetComponent<CoreSetting_iwata>().CheckCanAttach();
        Vector3 pos = trans.transform.position;     //付ける面の座標取得
        BoxCollider thiscollider = GetComponent<BoxCollider>();
        BoxCollider corecollider = trans.GetComponent<BoxCollider>();
        float[] dot = new float[3];
        dot[0] = Vector3.Dot(transform.forward, Vector3.forward);
        dot[1] = Vector3.Dot(transform.right, Vector3.forward);
        dot[2] = Vector3.Dot(transform.up, Vector3.forward);
        
        int nearestValue = 0;
        float nearestDistance = float.MaxValue;
        for (int i = 0; i < 3; i++)
        {
            float distance = Mathf.Abs(Mathf.Abs(dot[i]) - 1f);
            if (distance < nearestDistance)
            {
                nearestValue = i;
                nearestDistance = distance;
            }
        }
        switch (nearestValue)
        {
            case 0:
                pos.z -= corecollider.size.z / 2.0f;     //付ける面の大きさの半分ずらす
                pos.z -= thiscollider.size.z / 2.0f;        //自分の半分ずらす
                break;
            case 1:
                pos.z -= corecollider.size.z / 2.0f;     //付ける面の大きさの半分ずらす
                pos.z -= thiscollider.size.x / 2.0f;        //自分の半分ずらす
                break;
            case 2:
                pos.z -= corecollider.size.z / 2.0f;     //付ける面の大きさの半分ずらす
                pos.z -= thiscollider.size.y / 2.0f;        //自分の半分ずらす
                break;
        }

        this.transform.position = pos;      //ずらして決めた座標に移動する
        core.Rotate(0.0f, 10.0f, 0.0f, Space.World);       //一時的に0，0，0にしていた回転を戻す
    }

    public GameObject Orizin
    {
        get { return m_Origin; }
        set { m_Origin = value; }
    }

    public Renderer[] Renderers
    {
        get { return m_Renderer; }
    }

}
