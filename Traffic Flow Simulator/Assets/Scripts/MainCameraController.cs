using UnityEngine;
using System.Collections;

public class MainCameraController : MonoBehaviour {

	public float movement_velocity = 4f;
	public float movement_modifier = 3f;

	private float speed;
	
	void Start () {
		Screen.showCursor = false;
		speed = movement_velocity;
	}

	void Update () {
	
		if (Input.GetKeyDown(KeyCode.LeftShift)) {
			speed = movement_velocity * movement_modifier;
		}
		
		if (Input.GetKeyDown(KeyCode.LeftControl)) {
			speed = movement_velocity / movement_modifier;
		}
		
		if (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.LeftControl)) {
			speed = movement_velocity;
		}
	
		this.transform.Translate (Input.GetAxis("Horizontal") * Vector3.right * Time.deltaTime * speed, this.transform);
		this.transform.Translate (Input.GetAxis("Forward") * Vector3.forward * Time.deltaTime * speed, this.transform);
	}

	public void goTo (float x, float y, float z) {
		Vector3 v = new Vector3 (x,y,z);
		this.transform.position = v;
	}
}
