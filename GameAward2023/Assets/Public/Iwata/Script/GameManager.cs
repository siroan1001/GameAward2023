using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum eGameStatus
    {
        E_GAME_STATUS_START = 0,
        E_GAME_STATUS_JOINT,
        E_GAME_STATUS_ROT,
        E_GAME_STATUS_PLAY,
        E_GAME_STATUS_POUSE,
        E_GAME_STATUS_END,
        
        E_GAME_STATUS_MAX
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        m_PlayStage = GameObject.Find("PlayStage").transform;
        m_JointStage = GameObject.Find("JointStage").transform;
        m_Pose = GameObject.Find("Pose").transform;
    }

    [SerializeField] private static Transform m_PlayStage;       
    [SerializeField] private static Transform m_JointStage;      
    [SerializeField] private static Transform m_Pose;      

    [SerializeField] private static eGameStatus m_GameStatus;  
    [SerializeField] private static eGameStatus m_lastGameStatus;

    [SerializeField] private bool m_Debug = false;
    [SerializeField] private string m_DebugStage;

    static string szStage;

    // Start is called before the first frame update
    void Start()
    {
        m_GameStatus = eGameStatus.E_GAME_STATUS_JOINT;     //ゲームの状態
        m_lastGameStatus = m_GameStatus;                    //状態が変わったかを検出するために情報を退避させる
        ObjectBase.Start();                                 //オーディオとエフェクトを使えるように設定
        //ここでステージとガラクタをロードする
        if (m_Debug)
        {
            Debug.Log(m_DebugStage + "をよみこみます");
            LoadStageData_araki.SettingStageObjects(m_DebugStage + "Stage");
            Debug.Log("あをよみこみます");
            LoadStageData_araki.SettingJunks(m_DebugStage + "Garakuta");
            PlayStage.gameObject.SetActive(false);
            m_Pose.gameObject.SetActive(false);
            return;
        }
        Debug.Log("ゲーム始める準備");
        szStage = (int)WorldSelect_Ito.worldNum + "-" + (int)WorldSelect_Ito.stageNum;
        Debug.Log(szStage + "をよみこみます");
        LoadStageData_araki.SettingStageObjects(szStage + "Stage");
        LoadStageData_araki.SettingJunks(szStage + "Garakuta");
        PlayStage.gameObject.SetActive(false);
        m_Pose.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (m_GameStatus != m_lastGameStatus)
        {
            switch(m_lastGameStatus)
            {
                case eGameStatus.E_GAME_STATUS_JOINT:
                    switch (m_GameStatus)
                    {
                        case eGameStatus.E_GAME_STATUS_ROT:
                            m_JointStage.gameObject.SetActive(false);
                            m_PlayStage.gameObject.SetActive(true);
                            Vector3 startpos = m_PlayStage.Find("StageObject").Find("Start").transform.position;
                            GameObject core = Instantiate(m_JointStage.Find("Core").gameObject, startpos, m_JointStage.Find("Core").rotation);
                            // オブジェクトの回転角度を取得する
                            Quaternion currentRotation = core.transform.rotation;

                            // オブジェクトをY軸周りに回転させる
                            Quaternion targetRotation = Quaternion.AngleAxis(-10.0f, Vector3.up) * currentRotation;

                            // オブジェクトの回転を適用する
                            core.transform.rotation = targetRotation;


                            core.transform.parent = m_PlayStage.transform;
                            Destroy(core.GetComponent<CoreSetting_iwata>());
                            core.AddComponent<Core_Playing>();
                            core.GetComponent<Core_Playing>().StartRot = targetRotation;
                            core.GetComponent<Core_Playing>().StartFlag = true;
                            break;
                    }
                    break;
                case eGameStatus.E_GAME_STATUS_ROT:
                    switch(m_GameStatus)
                    {
                        case eGameStatus.E_GAME_STATUS_JOINT:
                            m_JointStage.gameObject.SetActive(true);
                            m_PlayStage.gameObject.SetActive(false);
                            m_PlayStage.Find("Core(Clone)").GetComponent<Core_Playing>().ResetPlayCore();
                            Destroy(m_PlayStage.Find("Core(Clone)").gameObject);
                            break;
                        case eGameStatus.E_GAME_STATUS_PLAY:
                            foreach(Transform child in m_PlayStage.Find("Core(Clone)").transform)
                            {
                                child.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                            }
                            
                            break;
                    }
                    break;
                case eGameStatus.E_GAME_STATUS_PLAY:
                    switch (m_GameStatus)
                    {
                        case eGameStatus.E_GAME_STATUS_ROT:
                            Destroy(m_PlayStage.Find("Core(Clone)").gameObject);
                            Vector3 startpos = m_PlayStage.Find("StageObject").Find("Start").transform.position;
                            GameObject core = Instantiate(m_JointStage.Find("Core").gameObject, startpos, Quaternion.identity);
                            core.transform.parent = m_PlayStage.transform;
                            Destroy(core.GetComponent<CoreSetting_iwata>());
                            core.AddComponent<Core_Playing>();
                            core.transform.rotation = core.GetComponent<Core_Playing>().StartRot;
                            Transform stageobject = PlayStage.Find("StageObject");
                            for (int i = stageobject.childCount - 1; i >= 0; i--)
                            {
                                Destroy(stageobject.GetChild(i).gameObject);
                            }
                            if(!m_Debug) LoadStageData_araki.SettingStageObjects(szStage + "Stage");
                            else LoadStageData_araki.SettingStageObjects(m_DebugStage + "Stage");
                            break;

                        case eGameStatus.E_GAME_STATUS_END:
                            Debug.Log("ゲームクリア！");
                            StartCoroutine(DelayedProcess());
                            break;

                    }
                    break;
                case eGameStatus.E_GAME_STATUS_POUSE:
                    m_Pose.gameObject.SetActive(false);
                    break;
            }
            if(m_GameStatus == eGameStatus.E_GAME_STATUS_POUSE)
            {
                m_Pose.gameObject.SetActive(true);
                Pose.activePose = true;
                return;
            }


            m_lastGameStatus = m_GameStatus;
        }
    }

    IEnumerator DelayedProcess()
    {
        yield return new WaitForSeconds(3); // 3秒待つ

        WorldSelect_Ito.stageNum++;
        if(WorldSelect_Ito.stageNum == WorldSelect_Ito.StageNum.Max)
        {
            WorldSelect_Ito.worldNum++;
            WorldSelect_Ito.stageNum = WorldSelect_Ito.StageNum.Stage1;
            if(WorldSelect_Ito.worldNum == WorldSelect_Ito.WorldNum.World3)
            {

            }
        }
        //SceneManager.LoadScene("GameScene_v2.0");
        Fade.instance.FadeToScene("GameScene_v2.0");
    }

    public static Transform PlayStage
    {
        get { return m_PlayStage; }
    }

    public static Transform JointStage
    {
        get { return m_JointStage; }
    }


    public static eGameStatus GameStatus
    {
        get { return m_GameStatus; }
        set { m_GameStatus = value; }
    }

    public static eGameStatus LastGameStatus
    {
        get { return m_lastGameStatus; }
        set { m_lastGameStatus = value; }
    }

}
