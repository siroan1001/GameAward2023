using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WorldSelect_Ito : MonoBehaviour
{
    public enum WorldNum
    {
        World3 = 1,     //第３階層
        World2,         //第２階層
        World1,         //第１階層
        MAX
    }
    public enum StageNum
    {
        Stage1 = 1,     //ステージ１
        Stage2,         //ステージ２
        Stage3,         //ステージ３
        Stage4,         //ステージ４
        Stage5,         //ステージ５
        Stage6,         //ステージ６
        Stage7,         //ステージ７
        Stage8,         //ステージ８
        Stage9,         //ステージ９
        Stage10,        //ステージ１０
        Max,
    }

    //キャンバス取得
    [SerializeReference] GameObject World1;
    [SerializeReference] GameObject World2;
    [SerializeReference] GameObject World3;
    [SerializeReference] GameObject W1Stage;
    [SerializeReference] GameObject W2Stage;
    [SerializeReference] GameObject W3Stage;

    private GameObject worldStageCtrlObj;

    public int unlockstage1Num = 0;　//ステージ解放用
    public int LoadSceneNum = 0;     //ステージロード用
    public static WorldNum worldNum; //ワールド選択管理用
    public static StageNum stageNum; //ステージ選択管理用
    private StageNum oldStageNum;    //ステージ選択管理用

    private string Scene;            //シーン先決定(名前をStartで指定)

    private int frameCount;          //フレームカウント用
    private bool activeWorld;
    private bool activeStage;

    private RectList W3RectChange;
    private RectList W2RectChange;
    private RectList W1RectChange;

    // Start is called before the first frame update
    void Start()
    {
        //コントロールオブジェクト
        worldStageCtrlObj = GameObject.Find("WorldStageCtrlObj");
        W3RectChange = W3Stage.GetComponent<RectList>();
        W2RectChange = W2Stage.GetComponent<RectList>();
        W1RectChange = W1Stage.GetComponent<RectList>();

        //初期化
        worldNum = WorldNum.World3;
        stageNum = StageNum.Stage1;
        oldStageNum = StageNum.Max;
        frameCount = 0;
        unlockstage1Num = 1;
        activeWorld = true;
        activeStage = false;

        //シーン遷移するステージ名
        Scene = "GameScene_v2.0";

        //ステージセレクトのimageの大きさの初期化
        W3RectChange.SetSizeImage((int)stageNum - 1, 1.3f);
        W2RectChange.SetSizeImage((int)stageNum - 1, 1.3f);
        W1RectChange.SetSizeImage((int)stageNum - 1, 1.3f);
    }

    // Update is called once per frame
    void Update()
    {
        //ポーズ画面の処理
        if(activeWorld)
            WorldSelect();  
        
        //決定した画面の表示
        if(PadInput.GetKeyDown(KeyCode.JoystickButton0))
        {
            activeWorld = false;
            ActiveStageList();
        }

        //ステージ選択画面表示後の処理
        if(activeStage)
        {
            SelectStage();
        }

        frameCount++;
    }

    private void WorldSelect()
    {
        //ステージ選択の入力処理
        worldNum += AxisInput.GetAxisRawRepeat("Vertical_PadX");

        //ステージ選択をループせずに止める
        if (worldNum == WorldNum.World3 - 1)
        {
           AudioManager.PlaySE(AudioManager.SEKind.E_SE_KIND_BEEP);
           worldNum = WorldNum.World3;
        }
        if (worldNum == WorldNum.MAX)
        {
            AudioManager.PlaySE(AudioManager.SEKind.E_SE_KIND_BEEP);
            worldNum = WorldNum.World1;
        }

        switch (worldNum)
        {
            case WorldNum.World3:
                World1.SetActive(false);
                World2.SetActive(false);
                World3.SetActive(true);
                break;

            case WorldNum.World2:
                World1.SetActive(false);
                World2.SetActive(true);
                World3.SetActive(false);
                break;

            case WorldNum.World1:
                World1.SetActive(true);
                World2.SetActive(false);
                World3.SetActive(false);
                break;
        }
    }
    private void ActiveStageList()
    {
        switch(worldNum)
        {
            case WorldNum.World1:
                World1.SetActive(false);
                W1Stage.SetActive(true);
                break;

            case WorldNum.World2:
                World2.SetActive(false);
                W2Stage.SetActive(true);
                break;

            case WorldNum.World3:
                World3.SetActive(false);
                W3Stage.SetActive(true);
                break;
        }
        activeStage = true;
    }

    private void SelectStage()
    {
        //過去の選択番号を退避
        oldStageNum = stageNum;

        //入力処理
        stageNum += AxisInput.GetAxisRawRepeat("Horizontal_PadX");
        stageNum -= AxisInput.GetAxisRawRepeat("Vertical_PadX") * 5;


        if (stageNum == StageNum.Stage1 - 1)
        {
            stageNum = StageNum.Stage10;
        }
        if (stageNum == StageNum.Stage10 + 1)
        {
            stageNum = StageNum.Stage1;
        }
        if(stageNum <= StageNum.Stage1)
        {
            stageNum += 10;
        }
        if(stageNum >= StageNum.Max)
        {
            stageNum -= 10;
        }

        switch (stageNum)
        {
            case StageNum.Stage1:                
                //一ステージのみ連続入力でステージに入らないように抑制
                if (frameCount > 100 && PadInput.GetKeyDown(KeyCode.JoystickButton0))
                {
                    if (unlockstage1Num >= 1)
                    {
                        LoadSceneNum = (int)StageNum.Stage1;
                        SceneManager.LoadScene(Scene);
                    }
                    else
                    {
                        AudioManager.PlaySE(AudioManager.SEKind.E_SE_KIND_BEEP);
                    }
                }         

                break;

            case StageNum.Stage2:

                if (PadInput.GetKeyDown(KeyCode.JoystickButton0))
                {
                    if (unlockstage1Num >= 2)
                    {
                        LoadSceneNum = (int)StageNum.Stage2;
                        SceneManager.LoadScene(Scene);
                    }
                    else
                    {
                        AudioManager.PlaySE(AudioManager.SEKind.E_SE_KIND_BEEP);
                    }
                }
                break;

            case StageNum.Stage3:

                if (PadInput.GetKeyDown(KeyCode.JoystickButton0))
                {
                    if (unlockstage1Num >= 3)
                    {
                        LoadSceneNum = (int)StageNum.Stage3;
                        SceneManager.LoadScene(Scene);
                    }
                    else
                    {
                        AudioManager.PlaySE(AudioManager.SEKind.E_SE_KIND_BEEP);
                    }
                }
                break;

            case StageNum.Stage4:

                if (PadInput.GetKeyDown(KeyCode.JoystickButton0))
                {
                    if (unlockstage1Num >= 4)
                    {
                        LoadSceneNum = (int)StageNum.Stage4;
                        SceneManager.LoadScene(Scene);
                    }
                    else
                    {
                        AudioManager.PlaySE(AudioManager.SEKind.E_SE_KIND_BEEP);
                    }
                }
                break;

            case StageNum.Stage5:

                if (PadInput.GetKeyDown(KeyCode.JoystickButton0))
                {
                    if (unlockstage1Num >= 5)
                    {
                        LoadSceneNum = (int)StageNum.Stage5;
                        SceneManager.LoadScene(Scene);
                    }
                    else
                    {
                        AudioManager.PlaySE(AudioManager.SEKind.E_SE_KIND_BEEP);
                    }
                }
                break;

            case StageNum.Stage6:

                if (PadInput.GetKeyDown(KeyCode.JoystickButton0))
                {
                    if (unlockstage1Num >= 6)
                    {
                        LoadSceneNum = (int)StageNum.Stage6;
                        SceneManager.LoadScene(Scene);
                    }
                    else
                    {
                        AudioManager.PlaySE(AudioManager.SEKind.E_SE_KIND_BEEP);
                    }
                }
                break;

            case StageNum.Stage7:

                if (PadInput.GetKeyDown(KeyCode.JoystickButton0))
                {
                    if (unlockstage1Num >= 7)
                    {
                        LoadSceneNum = (int)StageNum.Stage7;
                        SceneManager.LoadScene(Scene);
                    }
                    else
                    {
                        AudioManager.PlaySE(AudioManager.SEKind.E_SE_KIND_BEEP);
                    }
                }
                break;

            case StageNum.Stage8:

                if (PadInput.GetKeyDown(KeyCode.JoystickButton0))
                {
                    if (unlockstage1Num >= 8)
                    {
                        LoadSceneNum = (int)StageNum.Stage8;
                        SceneManager.LoadScene(Scene);
                    }
                    else
                    {
                        AudioManager.PlaySE(AudioManager.SEKind.E_SE_KIND_BEEP);
                    }
                }
                break;

            case StageNum.Stage9:

                if (PadInput.GetKeyDown(KeyCode.JoystickButton0))
                {
                    if (unlockstage1Num >= 9)
                    {
                        LoadSceneNum = (int)StageNum.Stage9;
                        SceneManager.LoadScene(Scene);
                    }
                    else
                    {
                        AudioManager.PlaySE(AudioManager.SEKind.E_SE_KIND_BEEP);
                    }
                }
                break;

            case StageNum.Stage10:

                if (PadInput.GetKeyDown(KeyCode.JoystickButton0))
                {
                    if (unlockstage1Num >= 10)
                    {
                        LoadSceneNum = (int)StageNum.Stage10;
                        SceneManager.LoadScene(Scene);
                    }
                    else
                    {
                        AudioManager.PlaySE(AudioManager.SEKind.E_SE_KIND_BEEP);
                    }
                }
                break;
        }

        if (PadInput.GetKeyDown(KeyCode.JoystickButton1))
        {
            ReturnWorld();
            stageNum = StageNum.Stage1;
            frameCount = 0;
        }

        if (oldStageNum == stageNum) return;        
        switch (worldNum)
        {
            case WorldNum.World3:
                W3RectChange.SetSizeImage((int)stageNum - 1, 1.3f);
                W3RectChange.SetSizeImage((int)oldStageNum - 1, 1.0f);
                break;
            case WorldNum.World2:
                W2RectChange.SetSizeImage((int)stageNum - 1, 1.3f);
                W2RectChange.SetSizeImage((int)oldStageNum - 1, 1.0f);
                break;
            case WorldNum.World1:
                W1RectChange.SetSizeImage((int)stageNum - 1, 1.3f);
                W1RectChange.SetSizeImage((int)oldStageNum - 1, 1.0f);
                break;
        }
    }

    private void ReturnWorld()
    {
        switch (worldNum)
        {
            case WorldNum.World1:
                World1.SetActive(true);
                W1Stage.SetActive(false);
                break;

            case WorldNum.World2:
                World2.SetActive(true);
                W2Stage.SetActive(false);
                break;

            case WorldNum.World3:
                World3.SetActive(true);
                W3Stage.SetActive(false);
                break;
        }
        activeStage = false;
        activeWorld = true;
    }
}
