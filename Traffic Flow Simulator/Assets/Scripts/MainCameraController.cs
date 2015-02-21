/*
	Copyright 2014-2015 Juan Antonio Fajardo Serrano

	Licensed under the Apache License, Version 2.0 (the "License");
	you may not use this file except in compliance with the License.
	You may obtain a copy of the License at

		http://www.apache.org/licenses/LICENSE-2.0

	Unless required by applicable law or agreed to in writing, software
	distributed under the License is distributed on an "AS IS" BASIS,
	WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	See the License for the specific language governing permissions and
	limitations under the License.
*/
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
		if (!SimulationUIController.is_paused) {
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
	}

	public void goTo (float x, float y, float z) {
		Vector3 v = new Vector3 (x,y,z);
		this.transform.position = v;
	}
}
