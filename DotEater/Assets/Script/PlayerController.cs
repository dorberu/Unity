using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerController : MonoBehaviour {

	public float moveSpeed = 5.0f;
	public float rotationSpeed = 360.0f;
	private Vector3 direction;

	CharacterController characterController;
	Animator animator;

	// Use this for initialization
	void Start () {
		characterController = GetComponent<CharacterController> ();
		animator = GetComponentInChildren<Animator> ();
		direction = Vector3.zero;
	}
	
	// Update is called once per frame
	void Update () {
		if (direction.sqrMagnitude > 0.01f) {
			Vector3 forward = Vector3.Slerp (
				transform.forward,
				direction,
				rotationSpeed * Time.deltaTime / Vector3.Angle(transform.forward, direction)
			);
			transform.LookAt (transform.position + forward);
		}
		characterController.Move (direction * moveSpeed * Time.deltaTime);
		animator.SetFloat ("Speed", characterController.velocity.magnitude);
		direction = Vector3.zero;
	}

	void OnTriggerEnter (Collider other) {
		if (other.tag == "Dot") {
			Destroy (other.gameObject);
		} else if (other.tag == "Enemy") {
			SceneManager.LoadScene ("Lose");
		}
	}

	public void Move (Vector3 direction) {
		this.direction += direction;
	}
}
