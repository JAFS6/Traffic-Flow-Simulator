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

public class MainCameraController : MonoBehaviour
{
	public float movement_velocity = 4f;
	public float movement_modifier = 3f;

	private float speed;
	private float max_x,min_x,max_z,min_z;
	private float min_y = 0.5f;
	
	public void Start ()
	{
		Cursor.visible = true;
		speed = movement_velocity;
	}

	public void Update ()
	{
		if (!SimulationUIController.is_paused)
		{
			if (Input.GetMouseButtonDown(1))
			{
				Cursor.visible = false;
			}
			
			if (Input.GetMouseButtonUp(1))
			{
				Cursor.visible = true;
			}
		
			if (Input.GetKeyDown(KeyCode.LeftShift))
			{
				speed = movement_velocity * movement_modifier;
			}
			
			if (Input.GetKeyDown(KeyCode.LeftControl))
			{
				speed = movement_velocity / movement_modifier;
			}
			
			if (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.LeftControl))
			{
				speed = movement_velocity;
			}
			
			this.transform.Translate (Input.GetAxis("Horizontal") * Vector3.right * Time.deltaTime * speed, this.transform);
			this.transform.Translate (Input.GetAxis("Forward" ) * Vector3.forward * Time.deltaTime * speed, this.transform);
			
			Vector3 new_pos = this.transform.position;
			
			// If the new position is out of the limits, clamp the position to the limit
			
			if (new_pos.x > max_x)
			{
				new_pos.x = max_x;
			}
			else if (new_pos.x < min_x)
			{
				new_pos.x = min_x;
			}
			
			if (new_pos.z > max_z)
			{
				new_pos.z = max_z;
			}
			else if (new_pos.z < min_z)
			{
				new_pos.z = min_z;
			}
			
			if (new_pos.y < min_y)
			{
				new_pos.y = min_y;
			}
			
			this.transform.position = new_pos;
		}
	}

	public void goTo (Vector3 pos, Vector3 dir)
	{
		Vector3 v = new Vector3 (pos.x,pos.y,pos.z);
		this.transform.position = v;
		this.transform.rotation = Quaternion.LookRotation(dir,Vector3.up);
	}
	
	public void setLimits (float max_x,float min_x,float max_z,float min_z)
	{
		this.max_x = max_x;
		this.min_x = min_x;
		this.max_z = max_z;
		this.min_z = min_z;
	}
}
