using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

public class StageDataToJson_araki : MonoBehaviour
{
	ObjectList m_list;
	[SerializeReference] string m_fileName;

	// Start is called before the first frame update
	void Start()
	{
		m_list = new ObjectList();
		m_list.m_objects = new List<ObjectData>();

        Debug.Log(m_fileName + "の生成を開始");

		//--- 子の情報をリストに格納
		for (int i = 0; i < this.transform.childCount; ++i)
		{
			// 子を取得
			Transform obj = this.transform.GetChild(i);

			// データ格納用変数
			ObjectData data = new ObjectData();
			
			//--- オブジェクトの子の情報を全て取得
			IEnumerable<Transform> children = obj.GetComponentsInChildren<Transform>(true);
			data.m_name = obj.name;
			data.m_pos = new float[children.Count() * 3];
			data.m_rot = new float[children.Count() * 3];
			int j = 0;
			foreach (Transform child in children)
			{
				//--- 座標
				data.m_pos[j * 3 + 0] = child.position.x;
				data.m_pos[j * 3 + 1] = child.position.y;
				data.m_pos[j * 3 + 2] = child.position.z;

				//--- 回転
				data.m_rot[j * 3 + 0] = child.rotation.x;
				data.m_rot[j * 3 + 1] = child.rotation.y;
				data.m_rot[j * 3 + 2] = child.rotation.z;

				j++;
			}
			m_list.m_objects.Add(data);	// リストに追加
		}

		// データをjsonに変換
		string json = JsonUtility.ToJson(m_list);

        Debug.Log(m_fileName + "の書き込み開始");

		// jsonファイルに書き込み(Resourcesファイルに格納)
#if true
		string filePath = Application.dataPath + "/Resources/" + m_fileName + ".json";
#else
		string filePath = Application.dataPath + "/Resources/StageData/" + m_fileName + ".json";
#endif
		StreamWriter sw;
		sw = new StreamWriter(filePath, false);
		sw.Write(json);
		sw.Flush();
		sw.Close();


        Debug.Log(m_fileName + "の生成を終了");
    }

	// Update is called once per frame
	void Update()
	{

	}

    public void Work()
    {
        m_list = new ObjectList();
        m_list.m_objects = new List<ObjectData>();

        Debug.Log(m_fileName + "の生成を開始");

        //--- 子の情報をリストに格納
        for (int i = 0; i < this.transform.childCount; ++i)
        {
            // 子を取得
            Transform obj = this.transform.GetChild(i);

            // データ格納用変数
            ObjectData data = new ObjectData();

            //--- オブジェクトの子の情報を全て取得
            IEnumerable<Transform> children = obj.GetComponentsInChildren<Transform>(true);
            data.m_name = obj.name;
            data.m_pos = new float[children.Count() * 3];
            data.m_rot = new float[children.Count() * 3];
            int j = 0;
            foreach (Transform child in children)
            {
                //--- 座標
                data.m_pos[j * 3 + 0] = child.position.x;
                data.m_pos[j * 3 + 1] = child.position.y;
                data.m_pos[j * 3 + 2] = child.position.z;

                //--- 回転
                data.m_rot[j * 3 + 0] = child.rotation.x;
                data.m_rot[j * 3 + 1] = child.rotation.y;
                data.m_rot[j * 3 + 2] = child.rotation.z;

                j++;
            }
            m_list.m_objects.Add(data); // リストに追加
        }

        // データをjsonに変換
        string json = JsonUtility.ToJson(m_list);

        Debug.Log(m_fileName + "の書き込み開始");

        // jsonファイルに書き込み(Resourcesファイルに格納)
#if true
        string filePath = Application.dataPath + "/Resources/" + m_fileName + ".json";
#else
		string filePath = Application.dataPath + "/Resources/StageData/" + m_fileName + ".json";
#endif
        StreamWriter sw;
        sw = new StreamWriter(filePath, false);
        sw.Write(json);
        sw.Flush();
        sw.Close();


        Debug.Log(m_fileName + "の生成を終了");
    }
}
