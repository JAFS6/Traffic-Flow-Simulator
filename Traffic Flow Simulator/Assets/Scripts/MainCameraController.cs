using UnityEngine;
using System.Collections;

public class MainCameraController : MonoBehaviour {

	public float vertical_velocity = 4f;
	public float movement_velocity = 4f;
	public float rotation_velocity = 20f;

	void Update () {
		this.transform.Translate (Input.GetAxis("Vertical") * Vector3.up * Time.deltaTime * vertical_velocity, this.transform);
		this.transform.Translate (Input.GetAxis("Horizontal") * Vector3.right * Time.deltaTime * movement_velocity, this.transform);
		this.transform.Translate (Input.GetAxis("Zoom") * Vector3.forward * Time.deltaTime * movement_velocity, this.transform);
		this.transform.Rotate (0, Input.GetAxis("RotHorizontal") * Time.deltaTime * rotation_velocity, 0, Space.World);
		this.transform.Rotate (Input.GetAxis("RotVertical") * Time.deltaTime * rotation_velocity, 0, 0, Space.Self);
	}

	public void goTo (float x, float y, float z) {
		Vector3 v = new Vector3 (x,y,z);
		this.transform.position = v;
	}
}
