using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class ObjectData
{
	public string m_name;
	public float[] m_pos;
	public float[] m_rot;
}

[System.Serializable]
public class JunkData
{
	public string m_name;
	public float[] m_pos;
	public float[] m_rot;
	public float[] m_params;
}

[System.Serializable]
public class ObjectList
{
	public List<ObjectData> m_objects;
}

[System.Serializable]
public class JunkList
{
	public List<JunkData> m_junks;
}

public static class LoadStageData_araki
{
	/// <summary>
	/// ステージ上にオブジェクトを設置
	/// </summary>
	public static void SettingStageObjects(string fileName)
	{
		//--- jsonファイルの読み込み
		ObjectList list;
		LoadJsonFile(fileName, out list);

		// ガラクタを保持する親オブジェクト
		GameObject stageObjectPparent = GameObject.Find("StageObject");

		//--- 読み込んだデータを基に処理
		foreach (ObjectData obj in list.m_objects)
		{
			//--- データを作成
			Object objData = Resources.Load<Object>("Prefabs/" + obj.m_name);
			GameObject prefab = (GameObject)objData;

			//--- オブジェクトの子の情報を全て取得
			IEnumerable<Transform> children = prefab.GetComponentsInChildren<Transform>(true);
			int j = 0;
            Debug.Log(prefab.transform.name);
			foreach (Transform child in children)
			{
				//--- 座標
				child.position = new Vector3(
					obj.m_pos[j * 3 + 0], obj.m_pos[j * 3 + 1], obj.m_pos[j * 3 + 2]);

				//--- 回転
				child.localEulerAngles = new Vector3(
					obj.m_rot[j * 3 + 0], obj.m_rot[j * 3 + 1], obj.m_rot[j * 3 + 2]);

				j++;
			}

			//--- オブジェクトを生成
			Transform prefabData = prefab.transform;
			GameObject stageObject = (GameObject)GameObject.Instantiate(
				objData, prefabData.position, prefabData.localRotation);

			// 親オブジェクトが無い場合は処理しない
			if (stageObjectPparent == null) continue;

			// 親をJankに設定
			stageObject.transform.SetParent(stageObjectPparent.transform);

			stageObject.name = prefab.name;	// 名前をプレハブと一致させる
		}
	}

	/// <summary>
	/// 読み込んだガラクタを設置
	/// </summary>
	public static void SettingJunks(string fileName)
	{
        //--- jsonファイルの読み込み
        ObjectList list;
        LoadJsonFile(fileName, out list);

        // ガラクタを保持する親オブジェクト
        GameObject stageObjectPparent = GameObject.Find("Jank");

        //--- 読み込んだデータを基に処理
        foreach (ObjectData obj in list.m_objects)
        {
            //--- データを作成
            Object objData = Resources.Load<Object>("Prefabs/" + obj.m_name);
            GameObject prefab = (GameObject)objData;

            //--- オブジェクトの子の情報を全て取得
            IEnumerable<Transform> children = prefab.GetComponentsInChildren<Transform>(true);
            int j = 0;
            foreach (Transform child in children)
            {
                //--- 座標
                child.position = new Vector3(
                    obj.m_pos[j * 3 + 0], obj.m_pos[j * 3 + 1], obj.m_pos[j * 3 + 2]);

                //--- 回転
                child.localEulerAngles = new Vector3(
                    obj.m_rot[j * 3 + 0], obj.m_rot[j * 3 + 1], obj.m_rot[j * 3 + 2]);

                j++;
            }

            //--- オブジェクトを生成
            Transform prefabData = prefab.transform;
            GameObject stageObject = (GameObject)GameObject.Instantiate(
                objData, prefabData.position, prefabData.localRotation);

            // 親オブジェクトが無い場合は処理しない
            if (stageObjectPparent == null) continue;

            // 親をJankに設定
            stageObject.transform.SetParent(stageObjectPparent.transform);

            stageObject.name = prefab.name; // 名前をプレハブと一致させる
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
}