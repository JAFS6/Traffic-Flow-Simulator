using UnityEngine;
using System.Collections;

public class MainCameraController : MonoBehaviour {

	public float movement_velocity = 4f;
	
	void Start () {
		Screen.showCursor = false;
	}

	void Update () {
		this.transform.Translate (Input.GetAxis("Horizontal") * Vector3.right * Time.deltaTime * movement_velocity, this.transform);
		this.transform.Translate (Input.GetAxis("Forward") * Vector3.forward * Time.deltaTime * movement_velocity, this.transform);
	}

	public void goTo (float x, float y, float z) {
		Vector3 v = new Vector3 (x,y,z);
		this.transform.position = v;
	}
}
