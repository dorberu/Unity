using UnityEngine;
using System.Collections;

public class Touch : MonoBehaviour {
	
	public AudioClip voice_01;
	public AudioClip voice_02;

	private Animator animator;
	private AudioSource univoice;

	private int motionIdol = Animator.StringToHash("Base Layer.Idol");

	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator> ();
		univoice = GetComponent<AudioSource> ();
	}
	
	// Update is called once per frame
	void Update () {
		animator.SetBool("Touch", false);
		animator.SetBool("TouchHead", false);

		// Check Animation Status
		if ( animator.GetCurrentAnimatorStateInfo(0).nameHash == motionIdol ) {
			animator.SetBool ("Motion_Idle", true);
		}
		else {
			animator.SetBool ("Motion_Idle", false);
		}

		Ray ray;
		RaycastHit hit;
		GameObject hitObject;
		if ( Input.GetMouseButtonDown(0) || Input.touchCount > 0 ) {
			if ( Input.touchCount > 0 ) {
				ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
			}
			else {
				ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			}

			if ( Physics.Raycast (ray, out hit, 100) ) {
				hitObject = hit.collider.gameObject;
				if ( hitObject.gameObject.tag == "Head" ) {
					animator.SetBool ("TouchHead", true);
					univoice.clip = voice_01;
					univoice.Play ();
					animator.SetBool ("Face_Happy", true);
					animator.SetBool ("Face_Angry", false);
				}
				else if ( hitObject.gameObject.tag == "Breast" ) {
					animator.SetBool ("Touch", true);
					univoice.clip = voice_02;
					univoice.Play ();
					animator.SetBool ("Face_Happy", false);
					animator.SetBool ("Face_Angry", true);
				}
			}
		}
	}
}
