using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class StageDirector : MonoBehaviour {

	public GameObject dotPreFab;

	// Use this for initialization
	void Start () {
		CreateDot (new Vector3 (-1.5f, 1.0f, 4.5f));
		CreateDot (new Vector3 (1.5f, 1.0f, 4.5f));
		CreateDot (new Vector3 (6.0f, 1.0f, 3.0f));
		CreateDot (new Vector3 (0.0f, 1.0f, -1.5f));
		CreateDot (new Vector3 (-6.0f, 1.0f, 0.0f));
	}
	
	// Update is called once per frame
	void Update () {
		if (GameObject.FindGameObjectsWithTag("Dot").Length == 0) {
			SceneManager.LoadScene ("Win");
		}
	}

	void CreateDot (Vector3 position) {
		GameObject dot = Instantiate (dotPreFab) as GameObject;
		dot.transform.position = position;
	}
}
