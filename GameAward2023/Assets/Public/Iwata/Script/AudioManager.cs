using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public enum BGMKind
    {
        E_BGM_KIND_TITLE = 0,   //  0
        E_BGM_KIND_STAGE1,      //  1
        E_BGM_KIND_MAX
    }
    public enum SEKind
    {
        E_SE_KIND_SELECT = 0,   //  2
        E_SE_KIND_HOGE,         //  3
        E_SE_KIND_KEYMOVE,      //  4 "タイトル、オプション画面、ステージ選択でのキー入力の際に再生する。"
        E_SE_KIND_KETTEI,       //  5 "タイトル、オプション画面、ステージ選択での操作の決定をする際に再生。"
        E_SE_KIND_CANCEL,       //  6 "タイトル、オプション画面、ステージ選択での操作を戻るをする際に再生。"
        E_SE_KIND_BEEP,         //  7 "タイトル、オプション画面、ステージ選択での操作を決定した際に選択不可の区画、階層を選択していた場合再生。"
        E_SE_KIND_GAMESTART,    //  8 "タイトル画面ではじめから、続きからを選択した状態で決定ボタン押したあと再生。"
        E_SE_KIND_ASSEMBLE,     //  9 "ガラクタをコアの面に組み立てた時の効果音。"
        E_SE_KIND_PREV_KEYMOVE, // 10 "プレビュー画面で決定後、面を選択する際のキー移動時に再生する。"
        E_SE_KIND_NOISE,        // 11 "組み立て画面。ガラクタにカーソルを合わせた際に出るプレビュー画面の砂嵐と同時に再生。"
        E_SE_KIND_MONITORON,    // 12 "組み立て画面。ガラクタにカーソルを合わせた際に出るプレビュー画面の砂嵐表示後、ガラクタのプレビューを表示する際に再生。"
        E_SE_KIND_MONITOROFF,   // 13 "組み立て画面。ガラクタにカーソルを合わせた際に出るプレビュー画面の砂嵐表示後、ガラクタのプレビューを表示する際に再生。"
        E_SE_KIND_OPTION,       // 14 "メニューを開く関連したボタンを押すと再生される。タイトルでのオプションを押しても再生"
        E_SE_KIND_WIND,         // 15 "ステージ内オブジェクトの送風機から発生する風の効果音。送風機がある場合は再生し続ける"
        E_SE_KIND_FIRE,         // 16 "ステージ内オブジェクトの燃えているドラム缶の炎の効果音。"
        E_SE_KIND_HARETU,       // 17 "スプリングガラクタのタイヤが釘コンクリートに接触すると破裂し、その際再生されるＳＥ"
        E_SE_KIND_EXPLOTION,    // 18 "特殊ガラクタのドラム缶がダメージオブジェクトの燃えているドラム缶に触れると爆発し、その際に再生"
        E_SE_KIND_MAX
    }

    public static Dictionary<BGMKind, string> BgmTypeNames = new Dictionary<BGMKind, string>()
    {
        { BGMKind.E_BGM_KIND_TITLE,     "TITLE" },
        { BGMKind.E_BGM_KIND_STAGE1,    "STAGE1" },
        { BGMKind.E_BGM_KIND_MAX,       "MAX" },
    };

    public static Dictionary<SEKind, string> SeTypeNames = new Dictionary<SEKind, string>()
    {
        { SEKind.E_SE_KIND_SELECT,          "SELECT" },
        { SEKind.E_SE_KIND_HOGE,            "HOGE" },
        { SEKind.E_SE_KIND_KEYMOVE,         "KEYMOVE" },
        { SEKind.E_SE_KIND_KETTEI,          "KETTEI" },
        { SEKind.E_SE_KIND_CANCEL,          "CANCEL" },
        { SEKind.E_SE_KIND_BEEP,            "BEEP" },
        { SEKind.E_SE_KIND_GAMESTART,       "GAMESTART" },
        { SEKind.E_SE_KIND_ASSEMBLE,        "ASSEMBLE" },
        { SEKind.E_SE_KIND_PREV_KEYMOVE,    "PREV_KEYMOVE" },
        { SEKind.E_SE_KIND_NOISE,           "NOISE" },
        { SEKind.E_SE_KIND_MONITORON,       "MONITORON" },
        { SEKind.E_SE_KIND_MONITOROFF,      "MONITOROFF" },
        { SEKind.E_SE_KIND_OPTION,          "OPTION" },
        { SEKind.E_SE_KIND_WIND,            "WIND" },
        { SEKind.E_SE_KIND_FIRE,            "FIRE" },
        { SEKind.E_SE_KIND_HARETU,          "HARETU" },
        { SEKind.E_SE_KIND_EXPLOTION,       "EXPLOTION" },
        { SEKind.E_SE_KIND_MAX,             "MAX" },
        
    };

    public static string BGMTypeToString(BGMKind BGMType)
    {
        if (BgmTypeNames.ContainsKey(BGMType))
        {
            return BgmTypeNames[BGMType];
        }
        else
        {
            return BGMType.ToString();
        }
    }

    public static string SETypeToString(SEKind SeType)
    {
        if (SeTypeNames.ContainsKey(SeType))
        {
            return SeTypeNames[SeType];
        }
        else
        {
            return SeType.ToString();
        }
    }

    public static AudioManager instance;

    [SerializeField] public List<AudioClip> bgms = new List<AudioClip>();
    [SerializeField] public List<AudioClip> ses = new List<AudioClip>();
    [SerializeField] private static float bgmVolume = 1.0f;
    [SerializeField] private static float seVolume = 1.0f;
    private static AudioSource bgmSource;
    private static AudioSource[] seSource = new AudioSource[(int)SEKind.E_SE_KIND_MAX];

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.loop = true;
        bgmSource.volume = bgmVolume;

        for (int i = 0; i < ses.Count; i++)
        {
            if (ses[i] == null) continue;
            seSource[i] = gameObject.AddComponent<AudioSource>();
            seSource[i].volume = seVolume;
            seSource[i].clip = ses[i];
        }
    }

    public static void PlayBGM(BGMKind index)
    {
        bgmSource.clip = instance.bgms[(int)index];
        bgmSource.Play();
    }

    public static void StopBGM()
    {
        bgmSource.Stop();
    }

    public static void PlaySE(SEKind index)
    {
        seSource[(int)index].Play();
    }

    public static void StopSE(SEKind index)
    {
        seSource[(int)index].Stop();
    }

    public static float BGMvolume
    {
        get { return bgmVolume; }
        set { bgmVolume = value; bgmSource.volume = bgmVolume; }
    }

    public static float SEvolume
    {
        get { return seVolume; }
        set { seVolume = value; for (int i = 0; i < instance.ses.Count; i++) { seSource[i].volume = seVolume; } }
    }
}