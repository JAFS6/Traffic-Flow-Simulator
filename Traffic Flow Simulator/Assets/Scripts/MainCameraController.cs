using UnityEngine;
using System.Collections;

public class MainCameraController : MonoBehaviour {

	public float vertical_velocity = 0.1f;
	public float movement_velocity = 0.1f;
	public float rotation_velocity = 0.01f;

	void Update () {
		this.transform.Translate (Input.GetAxis("Vertical") * Vector3.up * Time.deltaTime * vertical_velocity, this.transform);
		this.transform.Translate (Input.GetAxis("Horizontal") * Vector3.right * Time.deltaTime * movement_velocity, this.transform);
		this.transform.Translate (Input.GetAxis("Zoom") * Vector3.forward * Time.deltaTime * movement_velocity, this.transform);
		this.transform.Rotate (0, Input.GetAxis("RotHorizontal") * Time.deltaTime * rotation_velocity, 0, Space.World);
		this.transform.Rotate (Input.GetAxis("RotVertical") * Time.deltaTime * rotation_velocity, 0, 0, Space.Self);
	}
}
