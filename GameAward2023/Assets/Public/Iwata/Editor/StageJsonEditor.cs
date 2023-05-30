#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(StageDataToJson_araki))]
public class StageJsonEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("データ作成"))
        {
            // ボタンがクリックされた時の処理
            var hoge = target as StageDataToJson_araki;
            hoge.Work();
        }
    }
}

#endif