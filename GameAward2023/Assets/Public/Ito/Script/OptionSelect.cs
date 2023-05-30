using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class OptionSelect : MonoBehaviour
{
    private GameObject BGMImage1;
    private GameObject BGMImage2;    
    private GameObject SEImage1;
    private GameObject SEImage2;    
    private GameObject ReadMeImage1;
    private GameObject ReadMeImage2;

    private bool activeOption;
    
    [SerializeReference] public GameObject titleScreen;
    [SerializeReference] public GameObject OptionScreen;
    [SerializeReference] GameObject titleControle;

    [SerializeReference] public Slider bgmSlider;         // Sliderを格納する変数
    [SerializeReference] public Slider SESlider;          // SEを格納する変数    

    public int SelectOptionNum; 

    // Start is called before the first frame update
    void Start()
    {
        BGMImage1 = GameObject.Find("BGMImage1");       //光っている
        BGMImage2 = GameObject.Find("BGMImage2");       //光っていない
        SEImage1 = GameObject.Find("SEImage1");         //光っていない
        SEImage2 = GameObject.Find("SEImage2");         //光っている
        ReadMeImage1 = GameObject.Find("ReadMeImage1"); //光っていない
        ReadMeImage2 = GameObject.Find("ReadMeImage2"); //光っている               

        //音量スライダーの限界値初期化
        bgmSlider.maxValue = 1.0f;
        bgmSlider.minValue = 0.0f;
        SESlider.maxValue = 1.0f;
        SESlider.minValue = 0.0f;

        activeOption = true;

        SelectOptionNum = 0;
    }

    // Update is called once per frame
    public void Update()
    {    
        //選択
        SelectOptionNum -= AxisInput.GetAxisRawRepeat("Vertical_PadX");

        SelectOptionNum += 3;
        SelectOptionNum %= 3;

        switch (SelectOptionNum)
        {
            case 0:
                //音量がひかっている
                ChangeActive();               

                bgmSlider.value += AxisInput.GetAxisRawRepeat("Horizontal_PadX") * 0.2f;
                //AudioManager.BGMvolume = bgmSlider.value;
                break;

            case 1:
                //効果音が光っている
                ChangeActive();

                SESlider.value += AxisInput.GetAxisRawRepeat("Horizontal_PadX") * 0.2f;
                //AudioManager.SEvolume = SESlider.value;
                break;

            case 2:
                //操作説明が光っている
                ChangeActive();
               
                //if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Joystick1Button0))
                //{

                //}
                break;              
        }

        if (PadInput.GetKeyDown(KeyCode.JoystickButton1) && activeOption && Pose.activePoseOption)
        {
            Pose.activePoseOption = false;
            Pose.activePose = true;
            OptionScreen.SetActive(false);
        }
        else if(PadInput.GetKeyDown(KeyCode.JoystickButton1) && activeOption)
        {
            titleControle.SetActive(true);
            titleScreen.SetActive(true);
            OptionScreen.SetActive(false);
        }      
    }

    /// <summary>
    /// /// イメージアクティブ・非アクティブ変更関数
    /// </summary>
    public void ChangeActive()
    {
        switch(SelectOptionNum)
        {
            case 0:
                BGMImage1.SetActive(true);
                BGMImage2.SetActive(false);
                SEImage1.SetActive(true);
                SEImage2.SetActive(false);
                ReadMeImage1.SetActive(true);
                ReadMeImage2.SetActive(false);
                break;

            case 1:
                BGMImage1.SetActive(false);
                BGMImage2.SetActive(true);
                SEImage1.SetActive(false);
                SEImage2.SetActive(true);
                ReadMeImage1.SetActive(true);
                ReadMeImage2.SetActive(false);
                break;

            case 2:
                BGMImage1.SetActive(false);
                BGMImage2.SetActive(true);
                SEImage1.SetActive(true);
                SEImage2.SetActive(false);
                ReadMeImage1.SetActive(false);
                ReadMeImage2.SetActive(true);
                break;
        }
    }
}
