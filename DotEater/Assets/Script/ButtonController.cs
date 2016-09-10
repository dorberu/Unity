using UnityEngine;
using System.Collections;

public class ButtonController : MonoBehaviour {

	public GameObject player;
	bool push = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 direction = Vector3.zero;
		if (gameObject.tag == "LeftButton") {
			direction += PushLeft ();
		}
		if (gameObject.tag == "RightButton") {
			direction += PushRight ();
		}
		if (gameObject.tag == "UpButton") {
			direction += PushUp ();
		}
		if (gameObject.tag == "DownButton") {
			direction += PushDown ();
		}
		this.player.GetComponent<PlayerController> ().Move (direction);
	}

	Vector3 PushLeft () {
		if (push || Input.GetKey (KeyCode.LeftArrow)) {
			return Vector3.left;
		}
		return Vector3.zero;
	}

	Vector3 PushRight () {
		if (push || Input.GetKey (KeyCode.RightArrow)) {
			return Vector3.right;
		}
		return Vector3.zero;
	}

	Vector3 PushUp () {
		if (push || Input.GetKey (KeyCode.UpArrow)) {
			return Vector3.forward;
		}
		return Vector3.zero;
	}

	Vector3 PushDown () {
		if (push || Input.GetKey (KeyCode.DownArrow)) {
			return Vector3.back;
		}
		return Vector3.zero;
	}

	public void StartPush() {
		push = true;
	}

	public void StopPush() {
		push = false;
	}
}
