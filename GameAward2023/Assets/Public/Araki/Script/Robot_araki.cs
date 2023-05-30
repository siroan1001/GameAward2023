using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robot_araki : MonoBehaviour
{
	Vector3 forwardDirect;
	float rayCastDistance = 1.0f;
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
		RaycastHit hit;
		if (Physics.Raycast(transform.position, -transform.up, out hit, rayCastDistance))
		{
			//--- 前に踏んだパネルは処理しない
			if (lastHit == hit.transform) return;
			lastHit = hit.transform;

			Panel_araki panel = hit.collider.GetComponent<Panel_araki>();
			if (panel == null) return;

			// パネルによって進行方向を更新
			Vector3 vPanel = panel.GetForwardDirect() * moveSpeed;
			float dot = Vector3.Dot(vPanel.normalized, forwardDirect.normalized);

			//--- 新しいベクトルの方を向く
			float rotY = transform.rotation.y;
			transform.Rotate(0.0f, rotY - dot * 180.0f, 0.0f);

			forwardDirect = vPanel;	// ベクトルを更新
		}
	}

	private void FixedUpdate()
	{
		// 移動処理(等速直線運動)
		transform.position += forwardDirect;
	}
}