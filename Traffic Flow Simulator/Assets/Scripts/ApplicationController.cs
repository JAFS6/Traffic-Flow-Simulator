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
using System.Xml;
using System.IO;
using System.Collections.Generic;

public struct EntryNodeInfo
{
	public string id;
	public TransportType tt; // Transport Type
	public float tbs; 		 // Time Between Spawns
}

public class ApplicationController : MonoBehaviour {
	
	public static string map_filename = "";
	
	// Variables de control para instanciar vehiculos
	private Dictionary<string, EntryNodeInfo> entryNodes;

	// Variables de control de las posiciones predeterminadas de la camara
	private GameObject main_camera;
	private float initial_camera_position_x = 0;
	private float initial_camera_position_y = 100f;
	private float initial_camera_position_z = 0;
	private Vector2[] node_positions;

	// Acciones a realizar cuando se inicia la aplicacion
	void Start () {
		
		// Obtener la referencia a la camara del simulador
		main_camera = GameObject.Find("Main Camera");

		// Crear mapa nuevo
		RoadMap.CreateNewMap(map_filename);

		// Cargar los datos del mapa
		MapLoader loader = new MapLoader();
		loader.LoadMap(map_filename);

		// Dibujar el mapa
		RoadMap.draw ();

		// Guardar las posiciones de los nodos para posicionar la camara
		saveNodePositions ();
		
		// Calcular posicion inicial de la camara
		calculateCameraInitialPosition();

		// Colocar la camara en la posicion inicial
		main_camera.GetComponent<MainCameraController> ().goTo (initial_camera_position_x,initial_camera_position_y,initial_camera_position_z);
		
		// Guardar los identificadores de los nodos de entrada al mapa
		saveEntryNodes ();
		
		// Instanciar vehiculos
		StartCoroutine(spawnVehicles ());
	}

	private void saveNodePositions () {
		List<string> node_IDs = RoadMap.getNodeIDs ();

		node_positions = new Vector2[node_IDs.Count];

		int i = 0;

		foreach (string ID in node_IDs) {
			Vector2 pos = RoadMap.getNodePosition (ID);

			node_positions[i] = new Vector2(pos.x,pos.y);
			i++;
		}
	}
	
	private void calculateCameraInitialPosition () {
		float max_x = node_positions[0].x;
		float min_x = node_positions[0].x;
		float max_z = node_positions[0].y;
		float min_z = node_positions[0].y;
		
		foreach (Vector2 node in node_positions) {
			
			if (node.x > max_x) {
				max_x = node.x;
			}
			else if (node.x < min_x) {
				min_x = node.x;
			}
			
			if (node.y > max_z) {
				max_z = node.y;
			}
			else if (node.y < min_z) {
				min_z = node.y;
			}
		}
		
		initial_camera_position_x = (max_x + min_x) / 2;
		initial_camera_position_z = (max_z + min_z) / 2;
	}
	
	private void saveEntryNodes () {
		entryNodes = new Dictionary<string, EntryNodeInfo>();
		
		List<string> node_IDs = RoadMap.getNodeIDs ();
		TransportType tt;
		
		foreach (string ID in node_IDs) {
		
			if (RoadMap.isEntryNode(ID, out tt)) {
				
				EntryNodeInfo new_entry = new EntryNodeInfo();
				new_entry.id = ID;
				new_entry.tt = tt;
				new_entry.tbs = Random.value * 3;
				
				entryNodes.Add(ID,new_entry);
			}
		}
	}

	private IEnumerator spawnVehicles () {
		// Cargar prefabs vehiculos
		GameObject sport_car_prefab = Resources.Load("Prefabs/Sport_Car", typeof(GameObject)) as GameObject;
		GameObject green_jeep_prefab = Resources.Load("Prefabs/GreenJeep", typeof(GameObject)) as GameObject;
		GameObject orange_jeep_prefab = Resources.Load("Prefabs/OrangeJeep", typeof(GameObject)) as GameObject;
		GameObject bus_prefab = Resources.Load("Prefabs/Bus", typeof(GameObject)) as GameObject;
		Vector2 dir_prefab = new Vector3 (0,1);
		
		int num_prefabs = 4;
		GameObject [] prefab = new GameObject[num_prefabs];
		prefab[0] = sport_car_prefab;
		prefab[1] = green_jeep_prefab;
		prefab[2] = orange_jeep_prefab;
		prefab[3] = bus_prefab;
		
		// Obtener los ids de los nodos
		List<string> node_IDs = RoadMap.getNodeIDs();
		
		GameObject vehicle = null;
		int random;
		
		while (true) {
		
			foreach (string id in node_IDs) {
				random = Random.Range(0,num_prefabs);
				vehicle = spawnVehicle (prefab[random], dir_prefab, id);
			}
			yield return new WaitForSeconds(5);
		}
	}

	/**
	 * @brief Instancia el vehiculo prefab en el nodo node_id
	 * @param[in] prefab El prefab a instanciar
	 * @param[in] prefab_orientation La orientacion del prefab
	 * @param[in] node_id El identificador del nodo limite donde se instanciara el vehiculo
	 * @return Devuelve una referencia al objeto instanciado
	 * @post Si node_id no existe o no es un nodo de tipo limite o no es un nodo de entrada de vehiculos
	 * no se instanciara el vehiculo
	 */
	private GameObject spawnVehicle (GameObject prefab, Vector2 prefab_orientation, string node_id) {

		NodeType node_type = RoadMap.getNodeType (node_id);
		TransportType valid_transports_types;

		if (RoadMap.existsNode(node_id) && node_type == NodeType.Limit && RoadMap.isEntryNode(node_id, out valid_transports_types)) {
		
			// Obtener el tipo de vehiculo
			TransportType vehicle_tt = prefab.GetComponent<VehicleController>().transport_type;
			
			// Comprobar si se puede instanciar el vehiculo debido a su tipo
			if (valid_transports_types == vehicle_tt || valid_transports_types == TransportType.PublicAndPrivate) {
			
				// Obtener los puntos de entrada a los carriles
				List<GameObject> startPoints = RoadMap.getLaneStartPoints(node_id);
				
				// Seleccionar el carril por el que entrara
				// Si hay varios del tipo requerido seleccionarlo aleatoriamente
				List<GameObject> candidates = new List<GameObject>();
				
				startPoints.ForEach(delegate(GameObject point) {
					
					if ((point.name == Constants.Lane_Name_Public && vehicle_tt == TransportType.Public) || 
					    (point.name == Constants.Lane_Name_Normal && vehicle_tt == TransportType.Private)) {
						candidates.Add (point);
					}
				});
				
				if (candidates.Count > 0) {
					Vector3 start_position = candidates[0].transform.position;
					Vector2 dir_road = RoadMap.entryOrientation(node_id);
					Vector3 dir_road3D = new Vector3(dir_road.x,0,dir_road.y);
					Vector3 pos = new Vector3 (start_position.x, Constants.road_thickness/2, start_position.z);
					GameObject vehicle = GameObject.Instantiate (prefab, pos, Quaternion.LookRotation(dir_road3D)) as GameObject;
					vehicle.tag = Constants.Tag_Vehicle;
					return vehicle;
				}
				else {
					return null;
				}
			}
			else {
				return null;
			}
		}
		return null;
	}
}
