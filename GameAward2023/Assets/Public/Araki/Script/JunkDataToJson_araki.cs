using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

public class JunkDataToJson_araki : MonoBehaviour
{
	JunkList m_list;
	[SerializeReference] string m_fileName;

	// Start is called before the first frame update
	void Start()
	{
		m_list = new JunkList();
		m_list.m_junks = new List<JunkData>();

		//--- 子の情報をリストに格納
		for (int i = 0; i < this.transform.childCount; ++i)
		{
			// 子を取得
			Transform child = this.transform.GetChild(i);

			//--- データを格納
			JunkData data = new JunkData();
			data.m_pos = new float[3];
			data.m_name = child.name;
			data.m_pos[0] = child.position.x;
			data.m_pos[1] = child.position.y;
			data.m_pos[2] = child.position.z;
			data.m_rot = new float[3];
			data.m_rot[0] = child.localRotation.x;
			data.m_rot[1] = child.localRotation.y;
			data.m_rot[2] = child.localRotation.z;

			// パラメータの取得
			List<float> param = child.GetComponent<JankBase_iwata>().GetParam();
			data.m_params = new float[param.Count];
			for (int j = 0; j < param.Count; j++)
				data.m_params[j] = param[j];

			m_list.m_junks.Add(data); // リストに追加
		}

		StreamWriter sw;

		// データをjsonに変換
		string json = JsonUtility.ToJson(m_list);

		// jsonファイルに書き込み(Resourcesファイルに格納)
#if true
		string filePath = Application.dataPath + "/Resources/" + m_fileName + ".json";
#else
		string filePath = Application.dataPath + "/Resources/StageData/" + m_fileName + ".json";
#endif
		sw = new StreamWriter(filePath, false);
		sw.Write(json);
		sw.Flush();
		sw.Close();
	}

	// Update is called once per frame
	void Update()
	{

	}
}