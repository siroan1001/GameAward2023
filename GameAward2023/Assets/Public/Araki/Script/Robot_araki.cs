using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robot_araki : MonoBehaviour
{
	Vector3 forwardDirect;
	float rayCastDistance = 5.0f;
	[SerializeReference] float moveSpeed = 0.05f;
	Transform lastHit;

	private void Start()
	{
		// 何がぶつかってきても動じないようにする
		GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
		forwardDirect = transform.forward * moveSpeed;  // 最初は前に進むように設定
		lastHit = null;
	}

	private void Update()
	{
		//--- パネルを踏んだ時の処理
		Ray ray = new Ray(transform.position + transform.up.normalized * 3.0f, -transform.up);
		RaycastHit[] hits = Physics.RaycastAll(ray);
		for(int i = 0; i < hits.Length; i++)
		{
			Panel_araki panel = hits[i].transform.gameObject.GetComponent<Panel_araki>();
			if (panel == null) continue;

			//--- 前に踏んだパネルは処理しない
			if (lastHit == hits[i].transform) return;
			lastHit = hits[i].transform;

			// パネルによって進行方向を更新
			Vector3 vPanel = panel.GetForwardDirect() * moveSpeed;
			float dot = Vector3.Dot(vPanel.normalized, forwardDirect.normalized);

			//--- 新しいベクトルの方を向く
			float rotY = transform.rotation.y;
			transform.Rotate(0.0f, rotY - dot * 180.0f, 0.0f);

			forwardDirect = vPanel; // ベクトルを更新

			break;
		}
	}

	private void FixedUpdate()
	{
		// 移動処理(等速直線運動)
		transform.position += forwardDirect;
	}
}