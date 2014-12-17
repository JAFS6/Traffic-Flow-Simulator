using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum TurnSide : byte {Left, Right};
public enum TransportType: byte {Public, Private, PublicAndPrivate, Unknown};
public enum VehicleType : byte {Car, Bus};

public class VehicleController : MonoBehaviour {

	// Variables dependientes de cada vehiculo
	public VehicleType vehicle_type;	// Tipo de vehiculo
	public TransportType transport_type;// Tipo de transporte
	public float max_speed = 10f;		// Velocidad maxima que puede alcanzar el vehiculo
	public float acceleration = 0.1f;	// Aceleracion del vehiculo

	// Variables de control del vehiculo
	private float current_speed; 				// Velocidad actual en metros por segundo
	private string current_location; 			// Identificador del nodo o arco en el que se encuentra
	private bool intersection_first_encounter;	// Indicador de si acaba de encontrarse con una interseccion

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
	
	void Update () {
		Debug.DrawLine(this.transform.position,this.transform.position + this.transform.forward * 6,Color.magenta);

		// Raycasting
			// Front ray
		Vector3 front_ray_pos = new Vector3 (this.transform.position.x + (this.transform.forward.x * 2),
											 this.transform.position.y + front_sensor_y,
											 this.transform.position.z + (this.transform.forward.z * 2));
		Vector3 front_ray_dir = new Vector3 ();
		front_ray_dir = Vector3.Normalize ((this.transform.forward * 5) - this.transform.up);
		
			// Left ray
		Vector3 left_ray_pos = new Vector3 (this.transform.position.x + (this.transform.forward.x * 2),
											this.transform.position.y + front_sensor_y,
											this.transform.position.z + (this.transform.forward.z * 2));
		left_ray_pos = left_ray_pos - this.transform.right * 0.8f;
		Vector3 left_ray_dir = new Vector3 ();
		left_ray_dir = Vector3.Normalize ((this.transform.forward * 2) - this.transform.up);
		left_ray_dir = Vector3.Normalize (left_ray_dir - this.transform.right/2);
		
			// Right ray
		Vector3 right_ray_pos = new Vector3 (this.transform.position.x + (this.transform.forward.x * 2),
											 this.transform.position.y + front_sensor_y,
											 this.transform.position.z + (this.transform.forward.z * 2));
		right_ray_pos = right_ray_pos + this.transform.right * 0.8f;
		Vector3 right_ray_dir = new Vector3 ();
		right_ray_dir = Vector3.Normalize ((this.transform.forward * 2) - this.transform.up);
		right_ray_dir = Vector3.Normalize (right_ray_dir +  this.transform.right/2);

		// Front ray check
		if (Physics.Raycast(front_ray_pos,front_ray_dir, out front_ray_hit,sensor_length)) {
			Debug.DrawLine(front_ray_pos,front_ray_hit.point,Color.white);
			
			switch (front_ray_hit.transform.tag) {
			
				case RoadMap.limit_node_tag:
					Destroy(this.gameObject);
					break;
					
				case "Vehicle":
					current_speed = 0f;
					break;
					
				case RoadMap.edge_tag:
					if (!intersection_first_encounter) { // Acaba de llegar al arco
						current_location = front_ray_hit.transform.name;
						intersection_first_encounter = true;
						// TODO Girar el vehiculo para ponerlo en la linea del arco
						Vector2 entry_point = RoadMap.getEdgePosition(current_location);
						this.transform.rotation = Quaternion.LookRotation(new Vector3(entry_point.x - this.transform.position.x,
					                                                                  this.transform.position.y,
					                                               					  entry_point.y - this.transform.position.z));
					}
					break;
					
				case RoadMap.intersection_node_tag:
					if (intersection_first_encounter) {
						intersection_first_encounter = false;
						// Obtener lista de los arcos de salida
						List<string> exits_edges = RoadMap.exitPaths(front_ray_hit.transform.name, current_location, transport_type);
						// Actualizar posicion actual
						current_location = front_ray_hit.transform.name;
						// Elegir arco aleatoriamente
						string selected_edge = exits_edges[Random.Range(0,exits_edges.Count)];
						// Elegir punto de entrada al carril
						Vector2 entry_point = getNearestLaneStartPoint (selected_edge);
						// Girar hacia el punto
						this.transform.rotation = Quaternion.LookRotation(new Vector3(entry_point.x - this.transform.position.x,
						                                                              this.transform.position.y,
						                                                              entry_point.y - this.transform.position.z));
					}
					break;
			}
		}
		// Left ray check
		if (Physics.Raycast(left_ray_pos,left_ray_dir, out left_ray_hit,sensor_length)) {
			Debug.DrawLine(left_ray_pos,left_ray_hit.point,Color.red);

			switch (left_ray_hit.transform.name) {

				case RoadMap.hard_shoulder_line_name:
					Turn (TurnSide.Right, 1f);
					break;

				case RoadMap.normal_lane_line_name:
					
					if (transport_type == TransportType.Public) {
						Turn (TurnSide.Right, 1f);
					}
					else {
						Turn (TurnSide.Right, 1f);
					}
					
					break;

				case RoadMap.center_line_name:

					if (transport_type == TransportType.Public) {
						Turn (TurnSide.Right, 1f);
					}
					else {
						Turn (TurnSide.Right, 1f);
					}
					
					break;

				case RoadMap.public_transport_lane_line_name:
					
					if (transport_type == TransportType.Public) {
						Turn (TurnSide.Right, 1f);
					}
					else {
						Turn (TurnSide.Left, 1f);
					}

					break;
			} // End switch (left_ray_hit.transform.name)
		}
		// Right ray check
		if (Physics.Raycast(right_ray_pos,right_ray_dir, out right_ray_hit,sensor_length)) {
			Debug.DrawLine(right_ray_pos,right_ray_hit.point,Color.green);

			switch (right_ray_hit.transform.name) {
				
				case RoadMap.hard_shoulder_line_name:
					Turn (TurnSide.Left, 1f);
					break;
					
				case RoadMap.normal_lane_line_name:
					
					if (transport_type == TransportType.Public) {
						Turn (TurnSide.Right, 1f);
					}
					else {
						Turn (TurnSide.Right, 1f);
					}
					
					break;
					
				case RoadMap.public_transport_lane_line_name:
					
					if (transport_type == TransportType.Public) {
						Turn (TurnSide.Right, 1f);
					}
					else {
						Turn (TurnSide.Left, 1f);
					}
					
					break;
			} // End switch (right_ray_hit.transform.name)
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

	/**
	 * @brief Gira el vehiculo degrees grados hacia el lado t
	 * @param[in] t El lado hacia el que girara el vehiculo
	 * @param[in] degrees Los grados que girara el vehiculo
	 */
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
	
	/**
	 * @brief Obtiene las coordenadas en el plano 2D del punto de entrada al carril mas cercano
	 * al vehiculo del arco seleccionado y que se corresponda con su tipo de vehiculo
	 * @param[in] edge_id Arco seleccionado
	 */
	private Vector2 getNearestLaneStartPoint (string edge_id) {
	
		GameObject edge = GameObject.Find (edge_id);
		bool first = true;
		float best_distance = 0f;
		float distance = 0f;
		Vector2 position = new Vector2();
		
		foreach (Transform child in edge.transform) {
			
			if (child.tag == RoadMap.lane_start_point_tag) {
			
				Vector3 child_position = new Vector3(child.transform.position.x, this.transform.position.y, child.transform.position.z);
			
				if (first) {
					first = false;
					position.x = child.transform.position.x;
					position.y = child.transform.position.z;
					best_distance = MyMathClass.Distance(this.transform.position, child_position);
				}
				else {
					distance = MyMathClass.Distance(this.transform.position, child_position);
					
					if (distance < best_distance) {
						best_distance = distance;
						position.x = child.transform.position.x;
						position.y = child.transform.position.z;
					}
				}
			}
		}
		return position;
	}
}
