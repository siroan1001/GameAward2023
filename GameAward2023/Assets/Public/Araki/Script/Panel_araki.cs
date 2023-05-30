using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Panel_araki : MonoBehaviour
{
	// Start is called before the first frame update
	void Start()
	{
		// ステージ作成時にオブジェクトが見えないと分かりにくい為、
		// ゲーム起動時に見えないように設定する
		GetComponent<MeshRenderer>().enabled = false;
	}

	// Update is called once per frame
	void Update()
	{
	}

	public Vector3 GetForwardDirect()
	{
		return -transform.forward;
	}
}