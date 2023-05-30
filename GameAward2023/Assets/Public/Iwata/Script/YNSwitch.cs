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

    bool m_Flag;

    private void Start()
    {
        m_Flag = true;
    }
}
