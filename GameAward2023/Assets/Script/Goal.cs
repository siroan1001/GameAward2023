using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Goal : MonoBehaviour
{
	private void OnCollisionEnter(Collision collision)
	{
        Debug.Log("ぶつかった" + collision.transform.name);

		if (collision.transform.name.Contains("Core_Child"))
        {
            if (!collision.transform.parent.GetComponent<Core_Playing>().Life) return;

            Debug.Log("ゴールした");
            SceneManager.LoadScene("GameScene_v2.0");
        }

	}
}