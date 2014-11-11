using UnityEngine;
using System.Collections;

public class MainCameraController : MonoBehaviour {

	public float vertical_velocity = 0.1f;
	public float movement_velocity = 0.1f;
	public float rotation_velocity = 0.01f;

	Event e;
	bool key_pressed_status = false;
	KeyCode key_pressed;

	void OnGUI () {
		e = Event.current;

		if (e.type.Equals (EventType.keyDown) && !key_pressed_status) {
			key_pressed_status = true;
			key_pressed = e.keyCode;
		}

		if (e.type.Equals (EventType.keyUp)) {
			key_pressed_status = false;
		}
	}

	void Update () {

		if (key_pressed_status == true) {

			switch (key_pressed) {
				case KeyCode.X:
					this.transform.Translate(Vector3.up * Time.deltaTime * vertical_velocity, this.transform);
					break;
				case KeyCode.Z:
					this.transform.Translate(Vector3.down * Time.deltaTime * vertical_velocity, this.transform);
					break;
				case KeyCode.R:
					this.transform.Rotate(Time.deltaTime * -rotation_velocity, 0, 0, Space.Self);
					break;
				case KeyCode.F:
					this.transform.Rotate(Time.deltaTime * rotation_velocity, 0, 0, Space.Self);
					break;
				case KeyCode.Q:
					this.transform.Rotate(0, Time.deltaTime * -rotation_velocity, 0, Space.World);
					break;
				case KeyCode.E:
					this.transform.Rotate(0, Time.deltaTime * rotation_velocity, 0, Space.World);
					break;
				case KeyCode.W:
					this.transform.Translate(Vector3.forward * Time.deltaTime * movement_velocity, this.transform);
					break;
				case KeyCode.S:
					this.transform.Translate(Vector3.back * Time.deltaTime * movement_velocity, this.transform);
					break;
				case KeyCode.A:
					this.transform.Translate(Vector3.left * Time.deltaTime * movement_velocity, this.transform);
					break;
				case KeyCode.D:
					this.transform.Translate(Vector3.right * Time.deltaTime * movement_velocity, this.transform);
					break;
			}
		}
	}
}
