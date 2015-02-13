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
using System.Collections.Generic;

public class VehicleController : MonoBehaviour {

	public enum TurnSide : byte {Left, Right};
	public enum VehicleType : byte {Car, Bus};
	
	// Variables dependientes de cada vehiculo
	public VehicleType vehicle_type;	// Tipo de vehiculo
	public TransportType transport_type;// Tipo de transporte
	public float max_speed = 10f;		// Velocidad maxima que puede alcanzar el vehiculo
	public float acceleration = 0.1f;	// Aceleracion del vehiculo

	// Variables de control del vehiculo
	private float current_speed; 				// Velocidad actual en metros por segundo
	public string current_location; 			// Identificador del nodo o arco en el que se encuentra
	private bool intersection_detected = false;	// Indicador de si acaba de encontrarse con una interseccion
	private bool edge_detected = false;			// Indicador de si acaba de encontrarse con un arco
	private bool on_intersection = false; 		// Indica si se encuentra sobre una interseccion
	private DirectionType entry_orientation;

	// Sensores (raycasting)
	private const float front_sensor_y = 0.5f; // Altura de los sensores frontales
	private const float sensor_length = 10f; // Alcance del sensor

	private Vector3 front_ray_pos;
	private Vector3 left_ray_pos;
	private Vector3 right_ray_pos;
	private Vector3 down_ray_pos;
	private Vector3 front_ray_dir;
	private Vector3 left_ray_dir;
	private Vector3 right_ray_dir;
	private Vector3 down_ray_dir;
	private RaycastHit front_ray_hit;
	private RaycastHit left_ray_hit;
	private RaycastHit right_ray_hit;
	private RaycastHit down_ray_hit;
	
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
		
		// Down ray
		Vector3 down_ray_pos = new Vector3 (this.transform.position.x,
		                                     this.transform.position.y + 1f,
		                                     this.transform.position.z);
		Vector3 down_ray_dir = new Vector3 ();
		down_ray_dir = Vector3.Normalize (- this.transform.up);

		// Front ray check
		if (Physics.Raycast(front_ray_pos,front_ray_dir, out front_ray_hit,sensor_length)) {
			Debug.DrawLine(front_ray_pos,front_ray_hit.point,Color.white);
			
			switch (front_ray_hit.transform.tag) {
			
				case Constants.Tag_Node_Limit:
					Destroy(this.gameObject);
					break;
					
				case Constants.Tag_Vehicle:
					current_speed = 0f;
					break;
					
				case Constants.Tag_Node_Intersection:
				
					if (!intersection_detected && !on_intersection) {
						intersection_detected = true;
						edge_detected = false;
					}
					break;
					
				case Constants.Tag_Edge:
					
					if (!edge_detected && on_intersection) {
						edge_detected = true;
						intersection_detected = false;
					}
					break;
			}
		}
		// Left ray check
		if (Physics.Raycast(left_ray_pos,left_ray_dir, out left_ray_hit,sensor_length)) {
			Debug.DrawLine(left_ray_pos,left_ray_hit.point,Color.red);

			switch (left_ray_hit.transform.name) {

				case Constants.Line_Name_Hard_Shoulder:
					Turn (TurnSide.Right, 1f);
					break;

				case Constants.Line_Name_Normal_Lane:
					
					if (transport_type == TransportType.Public) {
						Turn (TurnSide.Right, 1f);
					}
					else {
						Turn (TurnSide.Right, 1f);
					}
					break;

				case Constants.Line_Name_Center:
					Turn (TurnSide.Right, 1f);
					break;

				case Constants.Line_Name_Public_Transport_Lane:
					
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
				
				case Constants.Line_Name_Hard_Shoulder:
					Turn (TurnSide.Left, 1f);
					break;
					
				case Constants.Line_Name_Normal_Lane:
					
					if (transport_type == TransportType.Public) {
						Turn (TurnSide.Right, 1f);
					}
					else {
						Turn (TurnSide.Left, 1f);
					}
					break;
					
				case Constants.Line_Name_Public_Transport_Lane:
					
					if (transport_type == TransportType.Public) {
						Turn (TurnSide.Right, 1f);
					}
					else {
						Turn (TurnSide.Left, 1f);
					}
					break;
				
				case Constants.Line_Name_Center:
					Turn (TurnSide.Right, 1f);
					break;
			} // End switch (right_ray_hit.transform.name)
		}
		
		// Down ray check
		if (Physics.Raycast(down_ray_pos,down_ray_dir, out down_ray_hit,sensor_length)) {
			Debug.DrawLine(down_ray_pos,down_ray_hit.point,Color.black);
			
			switch (down_ray_hit.transform.tag) {
			
				case Constants.Tag_Node_Intersection:
				
					if (intersection_detected && !on_intersection) {
						on_intersection = true;
						// Obtener lista de los arcos de salida
						List<string> exits_edges = RoadMap.exitPaths(front_ray_hit.transform.name, current_location, transport_type);
						
						if (exits_edges.Count <= 0) {
							Debug.LogError("Error: No exit path found.");
							Debug.Log("No exit path found.");
						}
						else {
							// Actualizar posicion actual
							current_location = down_ray_hit.transform.name;
							// Elegir arco aleatoriamente
							string selected_edge = exits_edges[Random.Range(0,exits_edges.Count)];
							// Elegir punto de entrada al carril
							Vector2 entry_point = getNearestLaneStartPoint (selected_edge, out entry_orientation);
							// Girar hacia el punto
							this.transform.rotation = Quaternion.LookRotation(new Vector3(entry_point.x - this.transform.position.x,
							                                                              this.transform.position.y,
							                                                              entry_point.y - this.transform.position.z));
                    	}
					}
					break;
				
				case Constants.Tag_Edge:
					
					if (edge_detected && on_intersection) { // Si estaba sobre una interseccion y acaba de llegar al arco
						on_intersection = false;
						current_location = down_ray_hit.transform.name;
						// TODO Girar el vehiculo para ponerlo en la linea del arco
						Vector2 edge_dir = RoadMap.getEdgeDirection(current_location, entry_orientation);
						this.transform.rotation = Quaternion.LookRotation(new Vector3(edge_dir.x, this.transform.position.y, edge_dir.y));
					}
					break;
			} // End switch (down_ray_hit.transform.name)
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
	 * @param[out] d Direccion respecto al arco: Source_Destination si el punto de entrada 
	 * pertenece al LaneStartPointGroup fuente o Destination_Source si pertenece al de destino
	 * @return La posicion del punto mas cercano
	 */
	private Vector2 getNearestLaneStartPoint (string edge_id, out DirectionType d) {
	
		GameObject edge = GameObject.Find (edge_id);
		bool first = true;
		float best_distance = 0f;
		float distance = 0f;
		Vector2 position = new Vector2();
		d = DirectionType.Source_Destination;
		
		foreach (Transform lane_start_group in edge.transform) {
			
			foreach (Transform child in lane_start_group.transform) {
			
				if (child.tag == Constants.Tag_Lane_Start_Point) {
				
					Vector3 child_position = new Vector3(child.transform.position.x, this.transform.position.y, child.transform.position.z);
				
					if (first) {
						first = false;
						position.x = child.transform.position.x;
						position.y = child.transform.position.z;
						best_distance = MyMathClass.Distance(this.transform.position, child_position);
						
						if (lane_start_group.name == Constants.Name_Source_Start_Points) {
							d = DirectionType.Source_Destination;
						}
						else {
							d = DirectionType.Destination_Source;
						}
					}
					else {
						distance = MyMathClass.Distance(this.transform.position, child_position);
						
						if (distance < best_distance) {
							best_distance = distance;
							position.x = child.transform.position.x;
							position.y = child.transform.position.z;
							
							if (lane_start_group.name == Constants.Name_Source_Start_Points) {
								d = DirectionType.Source_Destination;
							}
							else {
								d = DirectionType.Destination_Source;
							}
						}
					}
				}
			}
		}
		return position;
	}
}
