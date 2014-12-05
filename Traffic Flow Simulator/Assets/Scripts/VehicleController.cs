using UnityEngine;
using System.Collections;

public enum TurnSide : byte {Left, Right};

public class VehicleController : MonoBehaviour {

	private float current_speed; 	// Velocidad actual en metros por segundo

	private const float max_speed = 10f;		// Maxima velocidad que alcanzara el vehiculo
	private const float acceleration = 0.1f;

	// Sensores (raycasting)
	private const float front_sensor_y = 0.5f; // Altura de los sensores frontales
	private const float sensor_length = 10f; // Alcance del sensor

	private Vector3 front_ray_pos;
	private Vector3 left_ray_pos;
	private Vector3 right_ray_pos;
	private Vector3 front_ray_dir;
	private Vector3 left_ray_dir;
	private Vector3 right_ray_dir;
	private RaycastHit front_ray_hit;
	private RaycastHit left_ray_hit;
	private RaycastHit right_ray_hit;
	
	/**
	 * @brief Establece la velocidad del vehiculo
	 * @param[in] v La velocidad del vehiculo en metros por segundo
	 */
	public void setSpeed (float v) {
		this.current_speed = v;
	}

	// Update is called once per frame
	void Update () {
		Debug.DrawLine(this.transform.position,this.transform.position + this.transform.forward * 6,Color.magenta);

		// Raycasting

		Vector3 front_ray_pos = new Vector3 (this.transform.position.x + (this.transform.forward.x * 2) ,this.transform.position.y + front_sensor_y,this.transform.position.z + (this.transform.forward.z * 2));
		Vector3 front_ray_dir = new Vector3 ();
		front_ray_dir = Vector3.Normalize ((this.transform.forward * 5) - this.transform.up);
		
		Vector3 left_ray_pos = new Vector3 (this.transform.position.x + (this.transform.forward.x * 2),this.transform.position.y + front_sensor_y,this.transform.position.z + (this.transform.forward.z * 2));
		left_ray_pos = left_ray_pos - this.transform.right * 0.8f;
		Vector3 left_ray_dir = new Vector3 ();
		left_ray_dir = Vector3.Normalize ((this.transform.forward * 2) - this.transform.up);
		left_ray_dir = Vector3.Normalize (left_ray_dir - this.transform.right/2);
		
		Vector3 right_ray_pos = new Vector3 (this.transform.position.x + (this.transform.forward.x * 2),this.transform.position.y + front_sensor_y,this.transform.position.z + (this.transform.forward.z * 2));
		right_ray_pos = right_ray_pos + this.transform.right * 0.8f;
		Vector3 right_ray_dir = new Vector3 ();
		right_ray_dir = Vector3.Normalize ((this.transform.forward * 2) - this.transform.up);
		right_ray_dir = Vector3.Normalize (right_ray_dir +  this.transform.right/2);

		if (Physics.Raycast(front_ray_pos,front_ray_dir, out front_ray_hit,sensor_length)) {
			Debug.DrawLine(front_ray_pos,front_ray_hit.point,Color.white);
		}

		if (Physics.Raycast(left_ray_pos,left_ray_dir, out left_ray_hit,sensor_length)) {
			Debug.DrawLine(left_ray_pos,left_ray_hit.point,Color.red);

			if (left_ray_hit.transform.name == RoadMap.hard_shoulder_line_name || left_ray_hit.transform.name == RoadMap.normal_lane_line_name) {
				Turn (TurnSide.Right, 1f);
			}
		}

		if (Physics.Raycast(right_ray_pos,right_ray_dir, out right_ray_hit,sensor_length)) {
			Debug.DrawLine(right_ray_pos,right_ray_hit.point,Color.green);

			if (right_ray_hit.transform.name == RoadMap.hard_shoulder_line_name || right_ray_hit.transform.name == RoadMap.public_transport_lane_line_name) {
				Turn (TurnSide.Left, 1f);
			}
			else if (right_ray_hit.transform.name == RoadMap.center_line_name) {
				Turn (TurnSide.Right, 10f);
			}
		}

		// Increase speed
		if (this.current_speed < max_speed) {
			this.current_speed += acceleration;
		}

		// Movement

		Vector3 position = this.transform.position;
		position += this.transform.forward * this.current_speed * Time.deltaTime;
		this.transform.position = position;
	}

	private void Turn (TurnSide t, float degrees) {

		Quaternion rotation = Quaternion.AngleAxis(0f,new Vector3(0,1,0));

		if (t == TurnSide.Left) {
			rotation = Quaternion.AngleAxis(-degrees,new Vector3(0,1,0));
		}
		else if (t == TurnSide.Right) {
			rotation = Quaternion.AngleAxis(degrees,new Vector3(0,1,0));
		}

		Vector3 new_rotation_v = rotation * this.transform.forward;

		Quaternion new_rotation_q = Quaternion.LookRotation (new_rotation_v);

		this.transform.rotation = new_rotation_q;
	}
}
