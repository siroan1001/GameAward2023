using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class PrefabData_Json
{
    public string m_prefabName;
    public string m_junkName;
    public string m_categoriz;
    public string m_property;
    public string m_explnation;
}

[System.Serializable]
public class PrefabList
{
    public List<PrefabData_Json> m_prefabs;
}

public class JunkData_Json
{
    public string m_junkName;
    public string m_categoriz;
    public string m_property;
    public string m_explnation;
}

public class ReadJson : MonoBehaviour
{
    Dictionary<string, JunkData_Json> m_junkData;
    public Text textComponent;

    private void Start()
    {
        m_junkData = new Dictionary<string, JunkData_Json>();
        SettingPrefab("Prefab");
    }

    void SettingPrefab(string fileName)
    {
        //--- jsonファイルの読み込み
        PrefabList list;
        LoadJsonFile(fileName, out list);
        //--- 読み込んだデータを基に処理
        foreach (PrefabData_Json prefab in list.m_prefabs)
        {
            // ---データ作成
            JunkData_Json junk = new JunkData_Json();
            junk.m_junkName   = prefab.m_junkName;
            junk.m_categoriz  = prefab.m_categoriz;
            junk.m_property   = prefab.m_property;
            junk.m_explnation = prefab.m_explnation;
            m_junkData.Add(prefab.m_prefabName, junk);
        }
    }

    /// <summary>
	/// jsonファイルの読み込み
	/// </summary>
	static void LoadJsonFile<T>(string fileName, out T list)
    {
        //--- jsonファイルの読み込み
        string inputText = Resources.Load<TextAsset>(fileName).ToString();
        list = JsonUtility.FromJson<T>(inputText);  // 読み込んだデータをリスト化
    }

    /// <summary>
    /// json内のテキスト表示
    /// </summary>
    public void DisplayText(string prefabName)
    {
        JunkData_Json junk = new JunkData_Json();
        string matchName = null;
        List<string> keys = new List<string>(m_junkData.Keys);
        foreach (string pfname in keys)
        {
            Debug.Log(pfname);
            if (!prefabName.Contains(pfname)) continue;
            matchName = pfname;
        }

        junk = m_junkData[matchName];

        textComponent.text  = junk.m_junkName   + "\n";
        textComponent.text += junk.m_categoriz  + "\n";
        textComponent.text += junk.m_property   + "\n--------------------------\n";
        textComponent.text += junk.m_explnation;
    }

    public void ClearText()
    {
       textComponent.text = "";
    }
}
