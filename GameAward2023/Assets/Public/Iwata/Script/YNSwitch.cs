using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class YNSwitch : MonoBehaviour
{
    static float ACTIVE_ALPHE = 255;
    static float ACTIVE_ALPHE_F = 100;

    [SerializeField] Image m_Yes;
    [SerializeField] Image m_No;

    [SerializeField] bool m_Flag;
    [SerializeField] string m_SceneName;
    [SerializeField] GameObject m_MainPose;
    

    private void Start()
    {
        m_Flag = true;
        m_Yes.color = new Color(1.0f, 0.0f, 0.0f);
        m_No.color = new Color(1.0f, 1.0f, 1.0f);
    }

    void Update()
    {
        int input = 0;
        input += AxisInput.GetAxisRawRepeat("Horizontal_L");
        input += AxisInput.GetAxisRawRepeat("Horizontal_PadX");
        if (input != 0)
        {
            m_Flag = !m_Flag;
        }

        if(PadInput.GetKeyDown(KeyCode.JoystickButton0))
        {
            if(m_Flag)
            {
                Fade.instance.FadeToScene(m_SceneName);
            }
            else
            {
                m_MainPose.SetActive(true);
                this.gameObject.SetActive(false);
            }
        }

        if (PadInput.GetKeyDown(KeyCode.JoystickButton1))
        {
            m_MainPose.SetActive(true);
            this.gameObject.SetActive(false);
        }
    }

    private void FixedUpdate()
    {
        if(m_Flag)
        {
            m_Yes.color = new Color(1.0f, 0.0f, 0.0f);
            m_No.color = new Color(1.0f, 1.0f, 1.0f);
        }
        else
        {
            m_Yes.color = new Color(1.0f, 1.0f, 1.0f);
            m_No.color = new Color(1.0f, 0.0f, 0.0f);
        }

    }
}
